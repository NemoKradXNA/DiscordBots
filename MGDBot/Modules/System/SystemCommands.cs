using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json.Serialization;

using System.Net;

using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

using MGDBot.DataClasses;

using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;

namespace MGDBot.Modules
{
    public class SystemCommands : CommandModuleBase
    {
        //https://anidiotsguide_old.gitbooks.io/discord-js-bot-guide/content/examples/using-embeds-in-messages.html
        [Command("die")]
        [Summary("Attempts to kill the bot.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Die()
        {
            if (BotConfiguration.thisBotConfig.Owners.Contains(Context.User.Id))
            {
                EmbedBuilder b = new EmbedBuilder();
                b.WithColor(new Color(255, 0, 0));

                b.WithDescription($"{Context.User.Username} Shutting Down");

                await Context.Channel.SendMessageAsync("", false, b);
            }
            else
                await Task.CompletedTask;
        }
    }
}
