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
        
    }
}
