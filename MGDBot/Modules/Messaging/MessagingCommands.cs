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
    public class MessagingCommands : CommandModuleBase
    {

        [Command("purge", RunMode = RunMode.Async)]
        [Summary("Deletes the specified amount of messages, default is 100, does not purge pinned messages")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task PurgeChat(uint amount = 100)
        {
            if (BotConfiguration.thisBotConfig.Owners.Contains(Context.User.Id) || BotConfiguration.thisBotConfig.Moderators.Contains(Context.User.Id))
            {
                var messages = await this.Context.Channel.GetMessagesAsync((int)amount + 1).Flatten();
                messages = messages.Where(m => !m.IsPinned);


                await this.Context.Channel.DeleteMessagesAsync(messages);
            }
            else
            {
                await Context.Channel.SendMessageAsync("", false, GetWarningMsg("You can't do this"));
            }
        }
        
    }
}
