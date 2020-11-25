using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Bot.Discord.Commands
{
    public class PingCommand : BaseCommandModule
    {
        [Command("ping")]
        [Description("Ping the bot.")]
        public async Task Ping(
            CommandContext context)
        {
            await context.Channel.SendMessageAsync("Pong!");
        }
    }
}
