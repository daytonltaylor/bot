using System;
using System.Threading.Tasks;
using Bot.Console.Modules.Discord;
using Microsoft.Extensions.DependencyInjection;

namespace Bot.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program()
                .RunAsync()
                .GetAwaiter()
                .GetResult();
        }

        public async Task RunAsync()
        {
            // Create bots
            var discordBot = new DiscordBot();

            // register services
            IServiceCollection serviceCollection = new ServiceCollection();

            await discordBot.RegisterServicesAsync(serviceCollection);

            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            await discordBot.SetServiceProviderAsync(serviceProvider);

            // start bots
            await discordBot.StartAsync();

            // Prevent app exit
            await Task.Delay(-1);
        }
    }
}
