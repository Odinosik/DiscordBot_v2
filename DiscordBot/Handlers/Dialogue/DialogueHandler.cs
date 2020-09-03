using DiscordBot.Handlers.Dialogue.Steps;
using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Handlers.Dialogue
{
    public class DialogueHandler
    {
        private readonly DiscordClient _client;
        private readonly DiscordChannel _channel;
        private readonly DiscordUser _user;
        private  IDialogueStep _currentStep;

        public DialogueHandler(DiscordClient client, DiscordChannel channel, DiscordUser user, IDialogueStep startingStep)
        {
            _client = client;
            _channel = channel;
            _user = user;
            _currentStep = startingStep;
        }

        private List<DiscordMessage> messages = new List<DiscordMessage>();

        private List<DiscordMessage> PollMessages = new List<DiscordMessage>();

        public async Task<bool> ProcessDialogue()
        {
            while(_currentStep != null)
            {
                _currentStep.OnMessageAdded += (message) => messages.Add(message);

                var ResultEnum = await _currentStep.ProcessStep(_client, _channel, _user);
                if(ResultEnum == eDialogueType.Cancel)
                {
                    await DeleteMessages().ConfigureAwait(false);

                    var cancelEmbed = new DiscordEmbedBuilder
                    {
                        Title = "The Dialogue Has Succesfully Been Cancelled",
                        Description = _user.Mention,
                        Color = DiscordColor.DarkRed,
                    };

                    await _channel.SendMessageAsync(embed: cancelEmbed).ConfigureAwait(false);
                    return false;
                }

                else if (ResultEnum == eDialogueType.Continue)
                {
                    _currentStep = _currentStep.NextStep;
                }

                else if (ResultEnum == eDialogueType.Timeout)
                {
                    _currentStep = _currentStep.NextStep;
                    await DeleteMessages().ConfigureAwait(false);

                    var cancelEmbed = new DiscordEmbedBuilder
                    {
                        Title = "The Dialogue Has Succesfully Been Cancelled",
                        Description = _user.Mention,
                        Color = DiscordColor.DarkRed,
                    };
                    await _channel.SendMessageAsync(embed: cancelEmbed).ConfigureAwait(false);
                    return false;
                }
            }
            return true;
        }
        private async Task DeleteMessages()
        {
            if (_channel.IsPrivate) { return; }

            foreach(var message in messages)
            {
                await message.DeleteAsync().ConfigureAwait(false);
            }
        }
        public List<DiscordMessage> GetDialogue()
        {
            return messages;
        }
    }
}
