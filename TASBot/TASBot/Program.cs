using System;

using TASBot.Configuration;
using TASBot.Bot;
using TASBot.Services;

namespace TASBot
{
    class Program
    {
        static void Main(string[] args)
        {
            BotConfiguration confiuration = new BotConfiguration();

            TravellerBot bot = new TravellerBot();
            bot.StartServer(typeof(TravellerService)).GetAwaiter().GetResult();

            Console.ReadKey();
        }
    }
}
