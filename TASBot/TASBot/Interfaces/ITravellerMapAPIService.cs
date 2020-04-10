using System;
using System.Collections.Generic;
using System.Text;

using TASBot.Enums;

namespace TASBot.Interfaces
{
    public interface ITravellerMapAPIService
    {
        Dictionary<TravellerAPIMapEnums, string> TravellerMapAPI { get; }
    }
}
