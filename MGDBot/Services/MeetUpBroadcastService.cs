using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using Newtonsoft.Json.Serialization;

using System.Net;

using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

using MGDBot.DataClasses;

using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;

using MGDBot.DataClasses.MeetUp;
using MGDBot.Enums;

namespace MGDBot
{
    
    public class MeetUpBroadcastService
    {
        public MeetUpBroadcastService() { }

        List<BroadcastMeep> broadcasts = new List<BroadcastMeep>();

        public List<BroadcastMeep> GetBroadcastsForChannel(IChannel channel, string meeup = "")
        {
            List<BroadcastMeep> meeps = broadcasts.Where(m => m.Channel == channel).ToList();

            if(!string.IsNullOrEmpty(meeup))
                meeps = meeps.Where(m => m.MeetUp.Name == meeup).ToList();


            return meeps;
        }

        public void SetBroadcast(KnownMeetUp meeup, ISocketMessageChannel channel, TimeSpan frequency, MeetUpBroadCastTypeEnum type, out string msg)
        {
            msg = string.Empty;

            BroadcastMeep mub = broadcasts.SingleOrDefault(b => b.MeetUp.Name == meeup.Name  && b.Channel == channel);

            if (mub == null)
            {
                broadcasts.Add(new BroadcastMeep() { Channel = channel, MeetUp = meeup, Duration = frequency, BroadcastType = type });

                (new Thread(new ThreadStart(broadcasts[broadcasts.Count - 1].Broadcast))).Start();
            }
            else
                msg = $"A broadcast for {meeup.Name} on {channel.Name} has already been set.";
        }

        public void StopBroadcast(KnownMeetUp meeup, ISocketMessageChannel channel, out string msg)
        {
            msg = string.Empty;

            BroadcastMeep mub = broadcasts.SingleOrDefault(b => b.MeetUp.Name == meeup.Name && b.Channel == channel);

            if (mub == null)
                msg = $"A broadcast for {meeup.Name} on {channel.Name} has not been set.";
            else
            {
                mub.Live = false;
                msg = $"The {mub.BroadcastType} broadcast for {meeup.Name} on {channel.Name} has been stopped.";
                broadcasts.Remove(mub);
            }
        }
    }
}
