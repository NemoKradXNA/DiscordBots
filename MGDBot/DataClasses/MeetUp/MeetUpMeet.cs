using System;
using System.Collections.Generic;
using System.Text;

namespace MGDBot.DataClasses
{
    public class MeetUpMeet
    {
        public string id { get; set; }
        public string name { get; set; }
        public string status { get; set; }
        public TimeSpan local_time { get; set; }
        public DateTime local_date { get; set; }
        public MeetUpVenue venue { get; set; }
    }
}
