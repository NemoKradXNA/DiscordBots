using System;
using System.Collections.Generic;
using TASBot.Enums;
using TASBot.Interfaces;

namespace TASBot.Services
{
    public class TravellerService : ITravellerMapAPIService, IRPGService
    {
        public Dictionary<TravellerAPIMapEnums, string> TravellerMapAPI
        {
            get
            {
                return new Dictionary<TravellerAPIMapEnums, string>
                {
                    { TravellerAPIMapEnums.Universe,"https://travellermap.com/data" },
                    { TravellerAPIMapEnums.Sector, "https://travellermap.com/data/sector" },
                    { TravellerAPIMapEnums.Sector_Image, "https://travellermap.com/data/sector/image" },
                    { TravellerAPIMapEnums.Sector_Booklet, "https://travellermap.com/data/sector/booklet" },
                    { TravellerAPIMapEnums.Subsector, "https://travellermap.com/data/sector/subsector" },
                    { TravellerAPIMapEnums.Quadrant_Image,"https://travellermap.com/data/sector/quadrant/image"},
                    { TravellerAPIMapEnums.Subsector_Image, "https://travellermap.com/data/sector/subsector/image" },
                    { TravellerAPIMapEnums.World_Data,"https://travellermap.com/data/sector/hex" },
                    { TravellerAPIMapEnums.Jump_Range,"https://travellermap.com/data/sector/hex/jump/range" },
                    { TravellerAPIMapEnums.Jump_Image, "https://travellermap.com/data/sector/hex/jump/range/image" },
                    { TravellerAPIMapEnums.World_Sheet, "https://travellermap.com/data/sector/hex/sheet" },
                };
            }
        }

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
                    min = 0;
                    max = 1;
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
                retVal += rnd.Next(min, max+1);

            return retVal;
        }
    }
}
