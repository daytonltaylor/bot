using System;
using System.Threading.Tasks;
using Bot.Discord.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bot.Discord
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            IServiceProvider services = new ServiceCollection()
                .AddOptions()
                .Configure<BotSettings>(settings =>
                {
                    configuration.GetSection("Settings").Bind(settings);
                })
                .BuildServiceProvider();

            await new Bot(services).StartAsync();

            await Task.Delay(-1);
        }
    }
}
