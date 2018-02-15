using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;

using Discord;

namespace MGDBot
{
    public class BotConfiguration
    {
        public static BotConfiguration thisBotConfig;

        protected IConfigurationRoot config;

        public LogSeverity LogSeverity { get { return (LogSeverity)Enum.Parse(typeof(LogSeverity), config["loggingLevel"]); } }
        public string Token { get { return config["token"]; } }
        public List<ulong> Owners
        {
            get
            {
                List<ulong> ret = new List<ulong>();

                string owner = config["owners:0"];
                for (int o = 1; owner != null; o++)
                {
                    ret.Add(ulong.Parse(owner));
                    owner = config[$"owners:{o}"];
                }

                return ret;
            }
        }

        public List<ulong> Moderators
        {
            get
            {
                List<ulong> ret = new List<ulong>();

                string moderator = config["moderators:0"];
                for (int o = 1; moderator != null; o++)
                {
                    ret.Add(ulong.Parse(moderator));
                    moderator = config[$"moderators:{o}"];
                }

                return ret;
            }
        }

        public BotConfiguration()
        {
            thisBotConfig = this;

            Log($"Loading Configuration...");

            config = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory).AddJsonFile("config.json").Build();


            Log($"Total Owners: {Owners.Count}");
            Log($"Total Moerators: {Moderators.Count}");

        }

        public void Log(string message, LogSeverity Severity = LogSeverity.Info)
        {
            switch (Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            Console.WriteLine($"{DateTime.Now,-19} [{Severity,-8}] BotConfiguration: {message}");
            Console.ResetColor();
        }
    }
}
