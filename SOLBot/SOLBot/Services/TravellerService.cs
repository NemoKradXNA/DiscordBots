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

//using SOLBot.DataModels;
using SOLBot.Enums;

using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;

namespace SOLBot.Services
{
    public class TravellerService
    {
        public Dictionary<string, string> TravellerMapAPI = new Dictionary<string, string>
        {
            { "Universe","https://travellermap.com/data" },
            { "Sector Survey", "https://travellermap.com/data/sector" },
            { "Sector Tab", "https://travellermap.com/data/sector/tab" },
        };

        public Random rnd = new Random(DateTime.Now.Millisecond);

        public TravellerService() { }

        public int GetDiceRoll(DiceTypeEnum dieType, int numberOfDice = 1)
        {
            int retVal = 0;

            int min = 1;
            int max = 0;

            switch (dieType)
            {
                case DiceTypeEnum.Coin:
                    max = 2;
                    break;
                case DiceTypeEnum.D4:
                    max = 4;
                    break;
                case DiceTypeEnum.D6:
                    max = 6;
                    break;
                case DiceTypeEnum.D8:
                    max = 8;
                    break;
                case DiceTypeEnum.D10:
                    max = 10;
                    break;
                case DiceTypeEnum.D12:
                    max = 12;
                    break;
                case DiceTypeEnum.D20:
                    max = 20;
                    break;
                case DiceTypeEnum.D100:
                    max = 100;
                    break;
            }

            for (int d = 0; d < numberOfDice; d++)
                retVal += rnd.Next(min, max + 1);

            return retVal;
        }
    }
}
