using System;
using System.Threading.Tasks;
using Bot.Discord.Handlers.Dialogue.Base;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace Bot.Discord.Handlers.Dialogue.Steps
{
    public class TextDialogStep : BaseDialogueStep
    {
        private readonly int? minLength;
        private readonly int? maxLength;

        private IDialogueStep nextStep;

        public TextDialogStep(
            string content,
            IDialogueStep nextStep,
            int? minLength = null,
            int? maxLength = null) : base(
                content)
        {
            this.nextStep = nextStep;
            this.minLength = minLength;
            this.maxLength = maxLength;
        }

        public Action<string> OnValidResult { get; set; } = delegate { };

        public override IDialogueStep NextStep => nextStep;

        public override async Task<bool> ProcessStep(
            DiscordClient client,
            DiscordChannel channel,
            DiscordUser user)
        {
            var embedBuilder = new DiscordEmbedBuilder()
            {
                Title = "Please respond below",
                Description = $"{user.Mention}, {content}"
            };

            if (minLength.HasValue)
            {
                embedBuilder.AddField(
                    "Min length: ",
                    $"`{minLength.Value}` characters");
            }
            if (maxLength.HasValue)
            {
                embedBuilder.AddField(
                    "Max length: ",
                    $"`{maxLength.Value}` characters");
            }

            InteractivityExtension interactivity = client.GetInteractivity();

            while (true)
            {
                DiscordMessage botMessage = await channel.SendMessageAsync(
                    embed: embedBuilder.Build());
                OnMessageAdded(botMessage);

                InteractivityResult<DiscordMessage> resultUserMessage = await interactivity.WaitForMessageAsync(
                    x => x.ChannelId == channel.Id &&
                        x.Author.Id == user.Id);
                OnMessageAdded(resultUserMessage.Result);

                string messageContent = resultUserMessage.Result.Content;

                if (minLength.HasValue &&
                    messageContent.Length < minLength.Value)
                {
                    await ShowTryAgain(
                        channel,
                        $"Your input is `{minLength.Value - messageContent.Length}` characters too short");
                    continue;
                }
                if (maxLength.HasValue &&
                    messageContent.Length > maxLength.Value)
                {
                    await ShowTryAgain(
                        channel,
                        $"Your input is `{messageContent.Length - maxLength.Value}` characters too long");
                    continue;
                }

                OnValidResult(messageContent);

                return false;
            }
        }
    }
}
