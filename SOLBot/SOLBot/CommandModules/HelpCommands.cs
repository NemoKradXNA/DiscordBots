using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Net;

using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

//using MGDBot.DataClasses;

using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;


using SOLBot.BaseClasses;
using SOLBot.Configuration;

namespace SOLBot.CommandModules
{
    public class HelpCommands : CommandModuleBase
    {
        public HelpCommands() : base() { defaultColor = Color.Green; }

        [Command("help")]
        [Alias(new string[] { "h", "hlp", "info" })]
        [Summary("This command wiil give you\nhelpful information.")]
        public async Task Help()
        {
            string hlp = "There are lots of things you can do, and I will list them all here one day, honest...";

            EmbedBuilder hlpMag = GetMsg(defaultColor, $"Help", hlp);

            hlpMag.AddField("Commands", "You can get a list of commands that can be made by you by using\n~commands", true);
            hlpMag.AddField("Traveller Map", "You can access view the Traveller Map API commands wy using\n~travellermap", true);

            await Context.Channel.SendMessageAsync("", false, hlpMag.Build());
        }

        [Command("commands")]
        [Summary("This command wiil give a  list of ALL available commands.")]
        [Alias(new string[] { "cmds" })]
        public async Task ListAllCommands()
        {
            string hlp = "Here are all the commands I can find...";

            EmbedBuilder hlpMag = GetMsg(defaultColor, $"Listing All Commands:-", hlp);


            List<CommandInfo> commands = this.commandsService.Commands.ToList();

            foreach (CommandInfo command in commands)
            {
                string aliases = "";
                foreach (string alias in command.Aliases)
                    aliases += $"{BotConfiguration.thisBotConfig.CommandIndicator}{alias}, ";

                if (aliases.Length > 0)
                    aliases = aliases.Substring(0, aliases.Length - 2);

                hlpMag.AddField($"{command.Name}", $"Summary:\n{command.Summary}\n\nAliases:\n{aliases}", true);
            }

            await Context.Channel.SendMessageAsync("", false, hlpMag.Build());
        }

    }
}
