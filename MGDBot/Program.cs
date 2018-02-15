using System;


using MGDBot.Bots;

namespace MGDBot
{
    class Program
    {
        

        static void Main(string[] args)
        {
            BotConfiguration confiuration = new BotConfiguration();

            Bots.MGDBot bot = new Bots.MGDBot();

            bot.StartServer().GetAwaiter().GetResult();

            Console.ReadKey();

        }
    }
}
