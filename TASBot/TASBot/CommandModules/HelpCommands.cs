using System.Linq;
using System.Collections.Generic;

using System.Threading.Tasks;
using Discord;
using Discord.Commands;

using TASLibBot.BaseClasses;
using TASLibBot.Configuration;

namespace TASLibBot.CommandModules
{
    public class HelpCommands : CommandModuleBase
    {
        public HelpCommands() : base() { defaultColor = Color.Green; }

        [Command("help")]
        [Alias(new string[] { "h", "hlp", "info", "information", "?" })]
        [Summary("This command wiil give you\nhelpful information.")]
        public async Task Help(string hlp = null)
        {
            if (hlp == "?")
            {
                await SendHelpMessage($"Usage: {BotConfiguration.thisBotConfig.CommandIndicator}help", "This command wiil give you helpful information.", $"{BotConfiguration.thisBotConfig.CommandIndicator}help");
                return;
            }

            string txt = "There are lots of things you can do, and I will list them all here one day, honest...";

            EmbedBuilder msg = GetMsg(defaultColor, $"Help", txt);

            msg.AddField($"{BotConfiguration.thisBotConfig.CommandIndicator}commands", "You can get a list of commands that can be made by you.", true);
            msg.AddField($"{BotConfiguration.thisBotConfig.CommandIndicator}travellermap", "You can access view the Traveller Map API commands.", true);
            msg.AddField($"{BotConfiguration.thisBotConfig.CommandIndicator}rpg-commands", "This command lists ALL RPG related commands.", true);

            await SendMessageAsync(msg);
        }

        [Command("commands")]
        [Summary("This command wiil give a  list of ALL available commands.")]
        [Alias(new string[] { "cmds" })]
        public async Task ListAllCommands(string hlp = null)
        {
            if (hlp == "?")
            {
                await SendHelpMessage($"Usage: {BotConfiguration.thisBotConfig.CommandIndicator}commands", "This command wiil give a  list of ALL available commands.", $"{BotConfiguration.thisBotConfig.CommandIndicator}commands");
                return;
            }

            string txt = "Here are all the commands I can find...";

            EmbedBuilder msg = GetMsg(defaultColor, $"Listing All Commands:-", txt);


            List<CommandInfo> commands = commandsService.Commands.ToList();

            foreach (CommandInfo command in commands)
            {
                string aliases = "";
                foreach (string alias in command.Aliases)
                    aliases += $"{BotConfiguration.thisBotConfig.CommandIndicator}{alias}, ";

                if (aliases.Length > 0)
                    aliases = aliases.Substring(0, aliases.Length - 2);

                RequireUserPermissionAttribute ruap = (RequireUserPermissionAttribute)command.Preconditions.SingleOrDefault(a => a is RequireUserPermissionAttribute);

                if(HideIfNotAdmin(ruap, Context.User as IGuildUser))
                    continue;

                msg.AddField($"{BotConfiguration.thisBotConfig.CommandIndicator}{command.Name}", $"Summary:\n{command.Summary.Replace("[~]", $"{BotConfiguration.thisBotConfig.CommandIndicator}")}\n\nAliases:\n{aliases}", true);
            }

            await SendMessageAsync(msg);
        }

    }
}
