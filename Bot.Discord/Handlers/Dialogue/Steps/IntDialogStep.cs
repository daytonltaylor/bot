using System;
using System.Threading.Tasks;
using Bot.Discord.Handlers.Dialogue.Base;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace Bot.Discord.Handlers.Dialogue.Steps
{
    public class IntDialogStep : BaseDialogueStep
    {
        private readonly int? minValue;
        private readonly int? maxValue;

        private IDialogueStep nextStep;

        public IntDialogStep(
            string content,
            IDialogueStep nextStep,
            int? minValue = null,
            int? maxValue = null) : base(
                content)
        {
            this.nextStep = nextStep;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public Action<int> OnValidResult { get; set; } = delegate { };

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

            if (minValue.HasValue)
            {
                embedBuilder.AddField(
                    "Min value: ",
                    $"`{minValue.Value}`");
            }
            if (maxValue.HasValue)
            {
                embedBuilder.AddField(
                    "Max value: ",
                    $"`{maxValue.Value}`");
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

                if (!int.TryParse(messageContent, out int inputValue))
                {
                    await ShowTryAgain(
                        channel,
                        $"Your input is not an integer");
                    continue;
                }
                if (minValue.HasValue &&
                    inputValue < minValue.Value)
                {
                    await ShowTryAgain(
                        channel,
                        $"Your input, `{inputValue}`, is smaller than `{minValue.Value}`");
                    continue;
                }
                if (maxValue.HasValue &&
                    inputValue > maxValue.Value)
                {
                    await ShowTryAgain(
                        channel,
                        $"Your input, `{inputValue}`, is larger than `{maxValue.Value}`");
                    continue;
                }

                OnValidResult(inputValue);

                return false;
            }
        }
    }
}
