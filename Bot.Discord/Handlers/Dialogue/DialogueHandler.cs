using System.Collections.Generic;
using System.Threading.Tasks;
using Bot.Discord.Handlers.Dialogue.Base;
using DSharpPlus;
using DSharpPlus.Entities;

namespace Bot.Discord.Handlers.Dialogue
{
    public class DialogueHandler
    {
        private readonly DiscordClient client;
        private readonly DiscordChannel channel;
        private readonly DiscordUser user;

        private IDialogueStep currentStep;

        private readonly List<DiscordMessage> messages;

        public DialogueHandler(
            DiscordClient client,
            DiscordChannel channel,
            DiscordUser user,
            IDialogueStep startingStep)
        {
            this.client = client;
            this.channel = channel;
            this.user = user;

            currentStep = startingStep;

            messages = new List<DiscordMessage>();
        }

        public async Task<bool> ProcessDialogue()
        {
            bool isCancelled = false;

            while (currentStep != null)
            {
                currentStep.OnMessageAdded += (message) => messages.Add(message);

                isCancelled = await currentStep.ProcessStep(
                    client,
                    channel,
                    user);

                if (isCancelled)
                {
                    await channel.SendMessageAsync(
                        embed: new DiscordEmbedBuilder()
                        {
                            Title = "Dialogue Cancelled",
                            Description = user.Mention,
                            Color = DiscordColor.Green
                        });

                    break;
                }
            }

            if (!channel.IsPrivate)
            {
                foreach (DiscordMessage message in messages)
                {
                    await message.DeleteAsync();
                }
            }

            return !isCancelled;
        }
    }
}
