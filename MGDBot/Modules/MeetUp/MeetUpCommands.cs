using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
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

namespace MGDBot.Modules
{
    public class MeetUpCommands : CommandModuleBase
    {
        new protected IConfigurationRoot config = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory).AddJsonFile("Configuration/meetup.json").Build();

        /// <summary>
        /// API: https://www.meetup.com/meetup_api/
        /// My API Key: 19617d4825531d2320501a252f773e41
        /// API Builder: https://secure.meetup.com/meetup_api/console/?path=/:urlname/events
        /// </summary>
        List<KnownMeetUp> KnownMeetups
        {
            get
            {
                List<KnownMeetUp> mups = new List<KnownMeetUp>();

                string meetupName = config["meetups:0:name"];

                for(int m = 1;meetupName != null;m++)
                {
                    mups.Add(new KnownMeetUp()
                    {
                        Name = meetupName,
                        Link = config[$"meetups:{m-1}:link"],
                        URL = config[$"meetups:{m-1}:url"],
                        AliasMap = new List<string>(),
                    });

                    string alias = config[$"meetups:{m-1}:aliasmap:0"];
                    for (int a = 1; alias != null; a++)
                    {
                        mups[m-1].AliasMap.Add(alias);
                        alias = config[$"meetups:{m-1}:aliasmap:{a}"];
                    }

                    meetupName = config[$"meetups:{m}:name"];
                }

                return mups;
            }
        }

        [Command("mub")]
        public async Task BroadcastMUP(string meetup, string duration)
        {
            EmbedBuilder b = null;
            KnownMeetUp thisMeetup = GetThisMeetUp(meetup);

            if (thisMeetup != null)
            {
                MeetUpBroadcastService broadcastService = (MeetUpBroadcastService)Services.GetService(typeof(MeetUpBroadcastService));
                string state;
                TimeSpan ts;
                MeetUpBroadCastTypeEnum type = MeetUpBroadCastTypeEnum.Reoccuring;

                if (duration.ToLower().Substring(0, 6) == "before") // !mub name before02:00:00:00.000 in dd:hh:mm:ss.fff
                {
                    type = MeetUpBroadCastTypeEnum.TimeSpanBefore;
                    ts = TimeSpan.Parse(duration.Substring(6)); // How long before.
                }
                else if (duration.ToLower().Substring(0, 7) == "weekly:") // !mub name weekly:Monday
                {
                    type = MeetUpBroadCastTypeEnum.Weekly;
                    // 0 = sun - 6 sat
                    int td = (int)DateTime.Now.DayOfWeek;
                    int target = (int)(DayOfWeek)Enum.Parse(typeof(DayOfWeek), duration.Substring(7));
                    int dayDelta = Math.Abs(target - td);
                    ts = DateTime.Now.AddDays(dayDelta) - DateTime.Now; // Next Day to broadcast on from now
                }
                else if (duration.ToLower().Substring(0, 7) == "monthly") // !mub name monthly always first of the month.
                {
                    type = MeetUpBroadCastTypeEnum.Monthly;
                    ts = TimeSpan.Zero; // It's the 1st of every month
                }
                else
                    ts = TimeSpan.Parse(duration);                

                broadcastService.SetBroadcast(thisMeetup, Context.Channel, ts, type, out state);

                if (!string.IsNullOrEmpty(state))
                {
                    b = GetErrorMsg(state);
                }
                else
                    b = GetMsg("Broadcast Set",$"{type} Meet Up Broadcast set.");
            }
            else
                b = GetErrorMsg($"{meetup} is not a known meet up.");

            await Context.Channel.SendMessageAsync("", false, b);
        }
        
        [Command("listmeetup")]
        [Alias(new string[] { "lmu", "lstmu", "lmup" })]
        [Summary("Lists all known meet ups")]
        public async Task ListMeetUps()
        {
            EmbedBuilder b = new EmbedBuilder();

            b.WithTitle($"Meetup List");
            b.WithDescription($"All possible meetups you can view");
            b.WithColor(new Color(0, 170, 255));

            foreach (KnownMeetUp mup in KnownMeetups)
            {
                if (mup.Link.Contains("www.meetup.com"))
                    b.AddInlineField(mup.Name, $"MeetUp: {mup.Link}");
                else if (mup.Link.Contains("www.facebook.com"))
                    b.AddInlineField(mup.Name, $"FaceBook: {mup.Link}");
                else
                    b.AddInlineField(mup.Name, "No link...");
            }

            await Context.Channel.SendMessageAsync("", false, b);
        }

        [Command("meetup")]
        [Alias(new string[] { "mu", "mup" })]
        [Summary("Gets latest meetup")]
        public async Task meetupCommand(string meetup)
        {
            KnownMeetUp thisMup = GetThisMeetUp(meetup);

            EmbedBuilder b = new EmbedBuilder();


            if (thisMup != null)
            {

                b.WithTitle($"Meetup {thisMup.Name}");
                b.WithDescription($"The next meetup for {thisMup.Name}");
                b.WithColor(new Color(0, 170, 255));

                if (!string.IsNullOrEmpty(thisMup.URL))
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
                                MeetUpMeet latest = data.OrderBy(m => m.local_date).OrderBy(m => m.local_time).ToList()[0];
                                b.AddInlineField("Name:", $"{latest.name}");
                                b.AddInlineField("Date:", $"{latest.local_date.ToShortDateString()} {latest.local_time}");
                                b.AddInlineField("Venue:", $"{latest.venue.name}");
                                b.AddInlineField("Address:", $"{latest.venue.address_1}, {latest.venue.city}, {latest.venue.localized_country_name}");
                            }
                            else
                            {
                                b.WithFooter("There are no Upcoming Meetups...");
                            }
                        }
                    }

                    b.WithUrl(thisMup.Link);
                }
                else
                {
                    b.WithDescription($"{thisMup.Name} does not use MeetUp, click the name to find out more...");
                    b.WithUrl(thisMup.Link);
                }
            }
            else
            {
                b.WithTitle($"Meetup not found");
                b.WithDescription($"The next meetup for [{meetup}] does not exist, try listing what meetups we can view...");
                b.WithColor(new Color(0, 170, 255));
            }

            await Context.Channel.SendMessageAsync("", false, b);
        }

        KnownMeetUp GetThisMeetUp(string meetup)
        {
            return KnownMeetups.SingleOrDefault(m => m.AliasMap.Contains(meetup));
        }
    }
}
