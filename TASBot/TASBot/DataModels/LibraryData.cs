using System;
using System.Collections.Generic;
using System.Text;

namespace TASLibBot.DataModels
{
    public class LibraryData
    {
        public List<LibraryEntry> Entries { get; set; }
    }

    public class LibraryEntry
    {
        public string Name { get; set; }
        public string Data { get; set; }
    }
}
