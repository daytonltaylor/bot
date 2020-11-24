using System.Threading.Tasks;
using Discord.Commands;

namespace Bot.Console.Modules.Discord.Commands
{
    public class PingCommand : DiscordBotCommand
    {
        [Command("ping")]
        public async Task Handle()
        {
            await ReplyAsync("Pong");
        }
    }
}
