using System;
using System.Threading.Tasks;
using Bot.Discord.Handlers.Dialogue.Base;
using DSharpPlus;
using DSharpPlus.Entities;

namespace Bot.Discord.Handlers.Dialogue
{
    public abstract class BaseDialogueStep : IDialogueStep
    {
        protected readonly string content;

        public BaseDialogueStep(
            string content)
        {
            this.content = content;
        }

        public Action<DiscordMessage> OnMessageAdded { get; set; } = delegate { };

        public abstract IDialogueStep NextStep { get; }

        public abstract Task<bool> ProcessStep(
            DiscordClient client,
            DiscordChannel channel,
            DiscordUser user);

        protected async Task ShowTryAgain(
            DiscordChannel channel,
            string reason)
        {
            var embedBuilder = new DiscordEmbedBuilder()
            {
                Title = "Please try again",
                Color = DiscordColor.Red
            };
            embedBuilder.AddField(
                "There was a problem with your previous input",
                reason);

            DiscordMessage message = await channel.SendMessageAsync(
                embed: embedBuilder.Build());

            OnMessageAdded(message);
        }
    }
}
