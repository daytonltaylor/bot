using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;

namespace Bot.Discord.Handlers.Dialogue.Base
{
    public interface IDialogueStep
    {
        IDialogueStep NextStep { get; }

        Action<DiscordMessage> OnMessageAdded { get; set; }

        Task<bool> ProcessStep(
            DiscordClient client,
            DiscordChannel channel,
            DiscordUser user);
    }
}
