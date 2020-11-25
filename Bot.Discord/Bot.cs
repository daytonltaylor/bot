using System;
using System.Reflection;
using System.Threading.Tasks;
using Bot.Discord.Configuration;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bot.Discord
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }

        public Bot(
            IServiceProvider services)
        {
            BotSettings settings = services.GetService<IOptions<BotSettings>>().Value;

            Client = new DiscordClient(new DiscordConfiguration()
            {
                Token = settings.LoginToken,
                TokenType = TokenType.Bot,
                MinimumLogLevel = LogLevel.Debug
            });

            Commands = Client.UseCommandsNext(new CommandsNextConfiguration()
            {
                Services = services,
                StringPrefixes = new string[] { settings.CommandPrefix },
                EnableDms = false,
                EnableMentionPrefix = true
            });
            Commands.RegisterCommands(Assembly.GetEntryAssembly());

            Interactivity = Client.UseInteractivity(new InteractivityConfiguration());
        }

        public async Task StartAsync()
        {
            await Client.ConnectAsync();
        }
    }
}
