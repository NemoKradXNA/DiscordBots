using System;
using System.Collections.Generic;
using System.Text;

namespace MGDBot
{
    public class CommandListItem
    {
        public bool isAdmin { get; set; }
        public string command { get; set; }
        public string aliasmap { get; set; }
        public string summary { get; set; }
    }
}
