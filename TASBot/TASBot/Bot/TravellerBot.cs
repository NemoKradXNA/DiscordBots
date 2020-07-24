using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using TASLibBot.BaseClasses;

namespace TASLibBot.Bot
{
    public class TravellerBot : BotBase
    {
        public TravellerBot() : base()
        {
            Logger(new LogMessage(LogSeverity.Info, "TASLibBot", $"Version: { Assembly.GetExecutingAssembly().GetName().Version }"));
        }
    }
}
