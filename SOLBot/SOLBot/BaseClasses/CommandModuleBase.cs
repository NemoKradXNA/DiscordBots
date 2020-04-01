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

//using RoboMudBot.DataClasses;

using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;



namespace SOLBot.BaseClasses
{
    public class CommandModuleBase : ModuleBase<SocketCommandContext>
    {
        protected IServiceProvider Services { get { return BotBase.services; } }
        protected IConfigurationRoot config;
        protected Color defaultColor = new Color(0, 170, 255);

        private CommandService _commandsService = null;
        protected CommandService commandsService
        {
            get
            {
                if (_commandsService == null)
                {
                    _commandsService = new CommandService();
                    _commandsService.AddModulesAsync(Assembly.GetEntryAssembly(), Services);
                }
                return _commandsService;
            }
        }

        public CommandModuleBase()
        {
            
        }

        protected EmbedBuilder GetWarningMsg(string text, IGuildUser author = null)
        {
            EmbedBuilder b = new EmbedBuilder();
            b.WithColor(Color.Orange);
            b.WithDescription(text);
            b.WithTitle("Warning");
            if (author != null)
                b.WithAuthor(author);
            return b;
        }

        protected EmbedBuilder GetErrorMsg(string text, SocketUser author = null)
        {
            EmbedBuilder b = new EmbedBuilder();
            b.WithColor(Color.DarkRed);
            b.WithDescription(text);
            b.WithTitle("Error");
            if (author != null)
                b.WithAuthor(author);
            return b;
        }
        protected EmbedBuilder GetMsg(Color color, string title = null, string msg = null, SocketUser author = null, string footer = null, string url = null, string imgUrl = null, string thumbnailUrl = null, bool currentTime = false)
        {
            EmbedBuilder msgRet = GetMsg(title, msg, author, footer, url, imgUrl, thumbnailUrl, currentTime);
            msgRet.WithColor(color);

            return msgRet;
        }
        protected EmbedBuilder GetMsg(string title = null, string msg = null, SocketUser author = null, string footer = null, string url = null, string imgUrl = null, string thumbnailUrl = null, bool currentTime = false)
        {
            EmbedBuilder b = new EmbedBuilder();

            if (author != null)
                b.WithAuthor((IGuildUser)author);


            b.WithColor(defaultColor);

            if (currentTime)
                b.WithCurrentTimestamp();

            if (!string.IsNullOrEmpty(msg))
                b.WithDescription(msg);

            if (!string.IsNullOrEmpty(footer))
                b.WithFooter(footer);

            if (!string.IsNullOrEmpty(imgUrl))
                b.WithImageUrl(imgUrl);

            if (!string.IsNullOrEmpty(thumbnailUrl))
                b.WithThumbnailUrl(thumbnailUrl);

            //b.WithTimestamp(timeStamp);
            if (!string.IsNullOrEmpty(title))
                b.WithTitle(title);

            if (!string.IsNullOrEmpty(url))
                b.WithUrl(url);

            return b;
        }
    }
}
