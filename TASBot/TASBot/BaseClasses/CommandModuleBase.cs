using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
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



namespace TASLibBot.BaseClasses
{
    public class CommandModuleBase : ModuleBase<SocketCommandContext>
    {
        protected IServiceProvider Services { get { return BotBase.services; } }
        
        protected IConfigurationRoot config;
        protected Color defaultColor = new Color(0, 170, 255);

        protected Dictionary<string, List<string>> UserCommandHistory = new Dictionary<string, List<string>>();

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

        protected virtual async Task SendHelpMessage(string title, string msg, string footer = null, string echo = "")
        {
            EmbedBuilder msgToSend = null;

            msgToSend = GetMsg(Color.Green,title, msg);

            if(!string.IsNullOrEmpty(footer))
                msgToSend.WithFooter(footer);

            await SendMessageAsync(msgToSend);
        }

        protected virtual async Task SendMessageAsync(EmbedBuilder msg, string text = null, string footer = null, bool isTTS = false)
        {
            if (text == null)
                text = Context.Message.Content;

            if (!string.IsNullOrEmpty(footer))
                msg.WithFooter(footer);
            msg.WithAuthor(Context.User.Username.Replace(Context.User.Discriminator,""), Context.User.GetAvatarUrl());
            await Context.Channel.SendMessageAsync(text, isTTS, msg.Build());
        }

        protected virtual string GetJSONFileData(string file)
        {
            string json = string.Empty;

            using (StreamReader sr = new StreamReader(file))
            {
                json = sr.ReadToEnd();
            }

            return json;
        }

        protected virtual T DeserializeJSON<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        protected List<CommandInfo> GetMyCommandsList()
        {
            List<CommandInfo> retVal = commandsService.Commands.Where(c => c.Module.Name == GetType().Name).ToList();

            // Check access rigts..
            IGuildUser gu = Context.User as IGuildUser;

            if (gu != null)
            {
                int cnt = retVal.Count;
                for(int c = cnt-1;c >= 0;c--)
                {
                    CommandInfo cmd = retVal[c];
                    RequireUserPermissionAttribute ruap = (RequireUserPermissionAttribute)cmd.Preconditions.SingleOrDefault(a => a is RequireUserPermissionAttribute);

                    if (HideIfNotAdmin(ruap, gu))
                    {
                        retVal.Remove(cmd);
                    }
                }
            }

            return retVal;
        }

        protected virtual bool HideIfNotAdmin(RequireUserPermissionAttribute rupa, IGuildUser gu)
        {
            return rupa != null &&
                           rupa.GuildPermission != null &&
                           rupa.GuildPermission.Value == GuildPermission.Administrator &&
                           !gu.GuildPermissions.Administrator;

        }

        protected virtual string GetResponse(string url)
        {
            string json = string.Empty;

            WebRequest wr = WebRequest.Create(url);
            using (WebResponse response = wr.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(stream, Encoding.UTF8))
                    {
                        json = sr.ReadToEnd();
                    }
                }
            }

            return json;
        }

        protected virtual string GetResponseImage(string url, string name)
        {
            string fileName = $"{name}.png";

            WebRequest wr = WebRequest.Create(url);
            using (WebResponse response = wr.GetResponse())
            {

                using (BinaryReader br = new BinaryReader(response.GetResponseStream()))
                {
                    byte[] data = null;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        byte[] lnBuffer = br.ReadBytes(1024);
                        while (lnBuffer.Length > 0)
                        {
                            ms.Write(lnBuffer, 0, lnBuffer.Length);
                            lnBuffer = br.ReadBytes(1024);
                        }
                        data = new byte[(int)ms.Length];
                        ms.Position = 0;
                        ms.Read(data, 0, data.Length);
                    }

                    using (FileStream fs = new FileStream(fileName, FileMode.Create))
                    {
                        fs.Write(data, 0, data.Length);
                    }
                }

            }

            return fileName;
        }

        protected virtual string GetResponseToFile(string url, string name, string ext = null)
        {
            string fileName = $"{name}.{ext}";

            if (ext == null)
                fileName = name;

            WebRequest wr = WebRequest.Create(url);


            using (WebResponse response = wr.GetResponse())
            {
                using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    response.GetResponseStream().CopyTo(stream);
                }

            }

            return fileName;
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
