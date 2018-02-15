using System;
using System.Collections.Generic;
using System.Text;

namespace MGDBot.DataClasses
{
    public class MeetUpVenue
    {
        public long id { get; set; }
        public string name { get; set; }
        public string address_1 { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string localized_country_name { get; set; }
    }
}
