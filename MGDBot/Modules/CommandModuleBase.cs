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
    public class CommandModuleBase : ModuleBase<SocketCommandContext>
    {
        protected IServiceProvider Services { get { return MGDBot.Bots.MGDBot.services; } }
        protected IConfigurationRoot config;
        protected Color defaultColor = new Color(0, 170, 255);

        protected EmbedBuilder GetWarningMsg(string text)
        {
            EmbedBuilder b = new EmbedBuilder();
            b.WithColor(Color.Orange);
            b.WithDescription(text);
            b.WithTitle("Warning");
            return b;
        }

        protected EmbedBuilder GetErrorMsg(string text)
        {
            EmbedBuilder b = new EmbedBuilder();
            b.WithColor(Color.DarkRed);
            b.WithDescription(text);
            b.WithTitle("Error");
            return b;
        }

        protected EmbedBuilder GetMsg(string title = null, string msg = null, string author = null, string footer = null, string url = null,  string imgUrl = null, string thumbnailUrl = null, bool currentTime = false)
        {
            EmbedBuilder b = new EmbedBuilder();

            if(!string.IsNullOrEmpty(author))
                b.WithAuthor(author);


            b.WithColor(defaultColor);

            if(currentTime)
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
