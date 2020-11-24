using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Bot.Console.Modules.Discord
{
    public class DiscordBot
    {
        private IServiceProvider serviceProvider;

        private DiscordSocketClient client;
        private CommandService commands;

        public DiscordBot()
        {
            client = new DiscordSocketClient();

            client.Log += Client_LogAsync;
            client.MessageReceived += Client_MessageReceivedAsync;
        }

        public Task RegisterServicesAsync(
            IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(client);

            return Task.CompletedTask;
        }

        public Task SetServiceProviderAsync(
            IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;

            commands = new CommandService();

            commands.AddModulesAsync(
                Assembly.GetEntryAssembly(),
                this.serviceProvider);

            return Task.CompletedTask;
        }

        public async Task StartAsync()
        {
            await client.LoginAsync(
                TokenType.Bot,
                ProjectEnvironment.DiscordBot_LoginToken);

            await client.StartAsync();
        }

        private Task Client_LogAsync(
            LogMessage arg)
        {
            System.Console.WriteLine(arg);

            return Task.CompletedTask;
        }

        private async Task Client_MessageReceivedAsync(
            SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(client, message);

            int argPos = 0;
            if (message.Author.IsBot ||
                !message.HasStringPrefix(".", ref argPos))
            {
                return;
            }

            IResult result = await commands.ExecuteAsync(
                context,
                argPos,
                serviceProvider);

            if (!result.IsSuccess)
            {
                System.Console.WriteLine(result.ErrorReason);

                if (result.Error.Equals(CommandError.UnmetPrecondition))
                {
                    await message.Channel.SendMessageAsync(result.ErrorReason);
                }
            }
        }
    }
}
