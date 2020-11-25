using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bot.Discord.Handlers.Dialogue.Base;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace Bot.Discord.Handlers.Dialogue.Steps
{
    public class ReactionDialogStep : BaseDialogueStep
    {
        private readonly IDictionary<DiscordEmoji, ReactionDialogStepData> options;

        private DiscordEmoji selectedEmoji;

        public ReactionDialogStep(
            string content,
            IDictionary<DiscordEmoji, ReactionDialogStepData> options) : base(
                content)
        {
            this.options = options;
        }

        public override IDialogueStep NextStep => options[selectedEmoji].NextStep;

        public Action<DiscordEmoji> OnValidResult { get; set; } = delegate { };

        public override async Task<bool> ProcessStep(
            DiscordClient client,
            DiscordChannel channel,
            DiscordUser user)
        {
            DiscordEmoji cancelEmoji = DiscordEmoji.FromName(client, ":x:");

            var embedBuilder = new DiscordEmbedBuilder()
            {
                Title = "Please react below",
                Description = $"{user.Mention}, {content}"
            };
            embedBuilder.AddField(
                "To stop the dialogue",
                "React with :X:");

            InteractivityExtension interactivity = client.GetInteractivity();

            while (true)
            {
                var botMessage = await channel.SendMessageAsync(
                    embed: embedBuilder.Build());
                OnMessageAdded(botMessage);

                foreach (DiscordEmoji emoji in options.Keys)
                {
                    await botMessage.CreateReactionAsync(emoji);
                }
                await botMessage.CreateReactionAsync(cancelEmoji);

                InteractivityResult<MessageReactionAddEventArgs> resultReaction = await interactivity.WaitForReactionAsync(
                    x => options.ContainsKey(x.Emoji) ||
                        x.Emoji == cancelEmoji,
                    botMessage,
                    user);

                DiscordEmoji selectedEmoji = resultReaction.Result.Emoji;

                if (selectedEmoji == cancelEmoji)
                {
                    return true;
                }

                this.selectedEmoji = selectedEmoji;

                OnValidResult(selectedEmoji);

                return false;
            }
        }
    }

    public class ReactionDialogStepData
    {
        public string content { get; set; }
        public IDialogueStep NextStep { get; set; }
    }
}
