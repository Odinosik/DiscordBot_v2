using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Handlers.Dialogue.Steps
{
    public class PollStep : DialogueStepBase
    {
        private IDialogueStep _nextStep;
        private DiscordMessage _pollMessage;

        private DiscordEmbedBuilder _pollEmbed;

        public PollStep(string content, IDialogueStep nextStep, DiscordMessage pollMessage, DiscordEmbedBuilder pollEmbed) : base(content)
        {
            _nextStep = nextStep;
            _pollMessage = pollMessage;
            _pollEmbed = pollEmbed;
        }

        public Action<string> OnValidResult { get; set; } = delegate { };

        public override IDialogueStep NextStep => _nextStep;

        public void SetNextStep(IDialogueStep nextStep)
        {
            _nextStep = nextStep;
        }
        public override async Task<eDialogueType> ProcessStep(DiscordClient client, DiscordChannel channel, DiscordUser user)
        {
            var embedBuilder = new DiscordEmbedBuilder
            {
                Title = $"Please Respond Below",
                Description = $"{user.Mention}, {_content}"
            };

            embedBuilder.AddField("To Stop The Dialogue", "Use the ?cancel command");


            var interactivity = client.GetInteractivity();

            var embed = await channel.SendMessageAsync(embed: embedBuilder).ConfigureAwait(false);

            var pollDescription = string.Empty;

            while (true)
            {
                //OnMessageAdded(embed);

                var messageResult = await interactivity.WaitForMessageAsync(x => x.ChannelId == channel.Id && x.Author.Id == user.Id, TimeSpan.FromMinutes(1)).ConfigureAwait(false);

                OnMessageAdded(messageResult.Result);

                if (messageResult.TimedOut == true)
                {
                    return eDialogueType.Timeout;
                }

                if (messageResult.Result.Content.Equals("?cancel", StringComparison.OrdinalIgnoreCase))
                {
                    await embed.DeleteAsync();
                    return eDialogueType.Cancel;
                }

                if (messageResult.Result.Content.Equals("?start", StringComparison.OrdinalIgnoreCase))
                {
                    return eDialogueType.Continue;
                }

                pollDescription += (messageResult.Result.Content + Environment.NewLine);
                _pollEmbed.WithDescription(pollDescription);
                await _pollMessage.ModifyAsync(embed: _pollEmbed.Build());

                OnValidResult(messageResult.Result.Content);
            }
        }

        public DiscordEmbedBuilder GetPollMessage()
        {
            return _pollEmbed;
        }
    }
}
