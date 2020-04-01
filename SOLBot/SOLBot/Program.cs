using System;

using SOLBot.Configuration;
using SOLBot.Bot;
using SOLBot.Services;

namespace SOLBot
{
    class Program
    {
        static void Main(string[] args)
        {
            BotConfiguration confiuration = new BotConfiguration();

            TravellerBot bot = new TravellerBot();
            bot.StartServer(new TravellerService()).GetAwaiter().GetResult();

            Console.ReadKey();
        }
    }
}
