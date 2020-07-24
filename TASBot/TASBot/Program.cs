using System;

using TASLibBot.Configuration;
using TASLibBot.Bot;
using TASLibBot.Services;

namespace TASLibBot
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
