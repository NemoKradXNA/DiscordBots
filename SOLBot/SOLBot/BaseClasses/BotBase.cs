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

using SOLBot.Configuration;
//using SOLBot.Interfaces;

namespace SOLBot.BaseClasses
{
    public class BotBase : IServiceProvider
    {
        protected DiscordSocketClient discord;
        protected CommandService commands = new CommandService();
        protected IServiceCollection serviceMap = new ServiceCollection();
        public static IServiceProvider services;

        public BotBase()
        {
            discord = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = BotConfiguration.thisBotConfig.LogSeverity,
            });

            discord.Log += Logger;
            commands.Log += Logger;
        }

        public object GetService(Type serviceType)
        {
            return this;
        }

        protected virtual async Task Initialize(object broadcastService)
        {
            // Setup services..
            //services.AddSingleton(new Service());
            //serviceMap.AddSingleton(new BotCommandService(commands));
            serviceMap.AddSingleton(broadcastService);

            services = serviceMap.BuildServiceProvider();

            // Set up modules
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), this);

            // Subscribe a handler to see if a message invokes a command.
            discord.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            // Bail out if it's a System Message.
            var msg = arg as SocketUserMessage;
            if (msg == null)
                return;

            // We don't want the bot to respond to itself or other bots.
            // NOTE: Selfbots should invert this first check and remove the second
            // as they should ONLY be allowed to respond to messages from the same account.
            if (msg.Author.Id == discord.CurrentUser.Id || msg.Author.IsBot) return;

            // Create a number to track where the prefix ends and the command begins
            int pos = 0;
            // Replace the '!' with whatever character
            // you want to prefix your commands with.
            // Uncomment the second half if you also want
            // commands to be invoked by mentioning the bot instead.
            if (msg.HasCharPrefix(BotConfiguration.thisBotConfig.CommandIndicator, ref pos) /* || msg.HasMentionPrefix(_client.CurrentUser, ref pos) */)
            {
                // Log the command...
                await CommandLogger(arg, msg);

                // Create a Command Context.
                var context = new SocketCommandContext(discord, msg);

                // Execute the command. (result does not indicate a return value, 
                // rather an object stating if the command executed successfully).
                var result = await commands.ExecuteAsync(context, pos, services);

                if (result.IsSuccess)
                {
                    //await context.Message.DeleteAsync();
                    await context.Channel.DeleteMessageAsync(context.Message);
                }

                // Uncomment the following lines if you want the bot
                // to send a message if it failed (not advised for most situations).
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    await CommandLogger(arg, msg, LogSeverity.Error);
                }
                //    await msg.Channel.SendMessageAsync(result.ErrorReason);
            }
        }

        async Task CommandLogger(SocketMessage arg, SocketUserMessage msg, LogSeverity logSeverity = LogSeverity.Info)
        {
            string commandLogger = $"\n\tUser: {arg.Author.Username}#{arg.Author.Discriminator} {arg.Author.Mention}\n\tServer: {((SocketGuildChannel)arg.Channel).Guild.Name}\n\tChannel: {arg.Channel.Name} {arg.Channel.Id}\n\tMessage: {msg}";
            await Logger(new LogMessage(logSeverity, "HandleCommandAsync", commandLogger));

        }

        public async Task StartServer(object service)
        {
            await Initialize(service);

            await discord.LoginAsync(TokenType.Bot, BotConfiguration.thisBotConfig.Token);

            await discord.StartAsync();
        }

        public static Task Logger(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,-8}] {message.Source}: {message.Message} {message.Exception}");
            Console.ResetColor();

            // If you get an error saying 'CompletedTask' doesn't exist,
            // your project is targeting .NET 4.5.2 or lower. You'll need
            // to adjust your project's target framework to 4.6 or higher
            // (instructions for this are easily Googled).
            // If you *need* to run on .NET 4.5 for compat/other reasons,
            // the alternative is to 'return Task.Delay(0);' instead.
            return Task.CompletedTask;
        }
    }
}
