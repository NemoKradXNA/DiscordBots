using System;
using System.Collections.Generic;
using System.Text;

using TASLibBot.Enums;

namespace TASLibBot.Interfaces
{
    public interface ITravellerMapAPIService
    {
        Dictionary<TravellerAPIMapEnums, string> TravellerMapAPI { get; }
    }
}
