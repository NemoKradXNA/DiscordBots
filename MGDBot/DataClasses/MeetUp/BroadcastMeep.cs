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

using MGDBot.Enums;

namespace MGDBot.DataClasses.MeetUp
{
    public class BroadcastMeep
    {
        public KnownMeetUp MeetUp { get; set; }
        public ISocketMessageChannel Channel { get; set; }
        public TimeSpan Duration { get; set; }
        public bool Live { get; set; }

        // Used for TimeSpanBefore
        public bool GetNext { get; set; }

        // Used for Weekly/Monthly
        public DateTime LastUpdate { get; set; }
        public DateTime CreationDate { get; set; }
        

        public MeetUpBroadCastTypeEnum BroadcastType { get; set; }

        public BroadcastMeep()
        {
            BroadcastType = MeetUpBroadCastTypeEnum.Reoccuring;
            CreationDate = DateTime.Now;
            LastUpdate = CreationDate;
        }

        public void Broadcast()
        {
            Live = true;
            while (Live)
            {
                MeetUpMeet latest = GetNextMeetup(MeetUp);

                switch (BroadcastType)
                {
                    case MeetUpBroadCastTypeEnum.Reoccuring:
                        Reoccuring();
                        break;
                    case MeetUpBroadCastTypeEnum.TimeSpanBefore:
                        if (DateTime.Now > latest.local_date) // last one must have passed now, get latest again.
                            GetNext = false;

                        if (GetNext)
                            latest = GetNextMeetup(MeetUp, 1);

                        TimeSpanBefore(latest);
                        break;
                    case MeetUpBroadCastTypeEnum.Weekly:
                        Weekly();
                        break;
                    case MeetUpBroadCastTypeEnum.Monthly:
                        Monthly();
                        break;
                }

                EmbedBuilder msg = new EmbedBuilder();

                msg.WithTitle($"Meetup {MeetUp.Name}");
                msg.WithDescription($"The next meetup for {MeetUp.Name}");
                msg.WithColor(Color.DarkPurple);

                if (latest != null)
                {


                    msg.AddInlineField("Name:", $"{latest.name}");
                    msg.AddInlineField("Date:", $"{latest.local_date.ToShortDateString()} {latest.local_time}");
                    msg.AddInlineField("Venue:", $"{latest.venue.name}");
                    msg.AddInlineField("Address:", $"{latest.venue.address_1}, {latest.venue.city}, {latest.venue.localized_country_name}");
                }
                else
                {
                    msg.WithDescription($"{MeetUp.Name} does not use MeetUp, click the name to find out more...");
                    msg.WithUrl(MeetUp.Link);
                }

                Channel.SendMessageAsync("", false, msg);
            }
        }

        protected void Reoccuring()
        {
            Thread.Sleep((int)Duration.TotalMilliseconds);
        }

        protected void TimeSpanBefore(MeetUpMeet latest)
        {
            TimeSpan threadWait = new TimeSpan(0, 3, 0, 0, 0); // Need this to be in a config file?

            while (DateTime.Now.Add(Duration) < latest.local_date)
            {
                Thread.Sleep((int)threadWait.TotalMilliseconds);
            }
                        
            GetNext = true;
        }

        protected void Weekly()
        {
            TimeSpan threadWait = LastUpdate.Add(Duration) - LastUpdate;            

            Thread.Sleep((int)threadWait.TotalMilliseconds);

            Duration = new TimeSpan(7, 0, 0, 0, 0); // Add Seven Days, reached target day.
            LastUpdate = DateTime.Now;
        }

        protected void Monthly()
        {
            TimeSpan threadWait = new TimeSpan(7, 0, 0, 0, 0); // Need this to be in a config file?

            while (DateTime.Now.Day != 1)
            {
                Thread.Sleep((int)threadWait.TotalMilliseconds);
            }
        }

        protected MeetUpMeet GetNextMeetup(KnownMeetUp thisMup, int offset = 0)
        {
            HttpWebRequest webClient = (HttpWebRequest)WebRequest.Create(thisMup.URL);
            using (HttpWebResponse response = (HttpWebResponse)webClient.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {

                    List<MeetUpMeet> data = Newtonsoft.Json.JsonConvert.DeserializeObject<MeetUpMeet[]>(reader.ReadToEnd()).ToList();

                    if (data != null && data.Count > 0)
                    {
                        // Get Latest Meetup.
                        MeetUpMeet latest = data.OrderBy(m => m.local_date).OrderBy(m => m.local_time).ToList()[offset];
                        return latest;
                    }

                }
            }

            return null;
        }
    }
}
