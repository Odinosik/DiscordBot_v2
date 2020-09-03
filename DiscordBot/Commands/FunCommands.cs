using DiscordBot.Attributes;
using DiscordBot.Handlers.Dialogue;
using DiscordBot.Handlers.Dialogue.Steps;
using DiscordBot.Requests;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    class FunCommands : BaseCommandModule
    {
        [RequireCategories(ChannelCheckMode.None,"aba")]
        [Command("ping")]
        [Description("Pong")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Pong").ConfigureAwait(false);
        }

        [Command("add")]
        [Description("Adds two numbers")]
        public async Task Add(CommandContext ctx, [Description("Number one")] int numberOne, [Description("Number two")] int numberTwo)
        {
            await ctx.Channel
                .SendMessageAsync((numberOne + numberTwo).ToString())
                .ConfigureAwait(false);
        }
        [Command("responsemessage")]
        [Description("Adds two numbers")]
        public async Task Response(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var message = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel).ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync(message.Result.Content).ConfigureAwait(false);
        }

        [Command("responseemoji")]
        [Description("Adds two numbers")]
        public async Task ResponseEmoji(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var message = await interactivity.WaitForReactionAsync(x => x.Channel == ctx.Channel).ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync(message.Result.Emoji).ConfigureAwait(false);

        }
        [Command("greet"), Description("Says hi to specified user."), Aliases("sayhi", "say_hi")]
        public async Task Greet(CommandContext ctx, [Description("The user to say hi to.")] DiscordMember member) // this command takes a member as an argument; you can pass one by username, nickname, id, or mention
        {
            await ctx.TriggerTypingAsync();
            var emoji = DiscordEmoji.FromName(ctx.Client, ":michal_emotka:");
            await ctx.RespondAsync($"{emoji} Hello, {member.Mention}!");
        }


        [Command("pepe"), Aliases("feelsbadman"), Description("Feels bad, man.")]
        public async Task Pepe(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            var embed = new DiscordEmbedBuilder
            {
                Title = "Pepe",
                ImageUrl = "http://i.imgur.com/44SoSqS.jpg"
            };
            await ctx.RespondAsync(embed: embed);
        }

        [Command("gif"), Description("Send Random Gif")]
        public async Task Gif(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();

            string[] GIFtype = { "cat", "dog" };

            var giphyRequest = new GiphyRequests();
            string urlGif = giphyRequest.SendRequestRandomGif(GIFtype[0]);

            var embed = new DiscordEmbedBuilder
            {
                Title = GIFtype[0],
                Url = urlGif
            };

            await ctx.Channel.SendMessageAsync(content: urlGif);
        }

        [Command("dialogue"), Description("Send Random Gif")]
        public async Task Dialogue(CommandContext ctx)
        {
            var thirdStep = new TextStep("Enter something sad!", null);
            var secondStep = new TextStep("Enter something boring!", thirdStep);
            var inputStep = new TextStep("Enter something interesting!", secondStep);
            var funnyStep = new IntStep("Haha funny",null,4,10);

            string input = string.Empty;
            int value = 0;


            funnyStep.OnValidResult += (result) => value = result;

            var userChannel = await ctx.Member.CreateDmChannelAsync().ConfigureAwait(false);

            var inputDialogueHandler = new DialogueHandler(ctx.Client, userChannel, ctx.User, inputStep);

            List<DiscordMessage> dialogue =  inputDialogueHandler.GetDialogue();

            bool succeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            StringBuilder inputDialogue = new StringBuilder();


            foreach (var message in dialogue)
            {
                inputDialogue.Append($"** {message.Content} ** \n");
            }

            var finalEmbed = new DiscordEmbedBuilder
            {
                Description = inputDialogue.ToString(),
                Color = DiscordColor.DarkBlue,
                Title = "Dialogue"
            };

            if (!succeded)
            {
                return;
            }

            await ctx.Channel.SendMessageAsync(embed : finalEmbed).ConfigureAwait(false);

            await ctx.Channel.SendMessageAsync(value.ToString()).ConfigureAwait(false);

        }

        [Command("dialoguePoll"), Description("Create Poll")]
        public async Task DialoguePoll(CommandContext ctx, TimeSpan? timespan)
        {

            var pollEmbed = new DiscordEmbedBuilder
            {
                Title = "Create Poll",
                Color = DiscordColor.DarkBlue
            };

            var userChannel = await ctx.Member.CreateDmChannelAsync().ConfigureAwait(false);

            var pollMessage = await userChannel.SendMessageAsync(embed: pollEmbed).ConfigureAwait(false);

            var pollStep = new PollStep("Send to me options in poll", null, pollMessage, pollEmbed);

            var inputDialogueHandler = new DialogueHandler(ctx.Client, userChannel, ctx.User, pollStep);

            bool succeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if (!succeded)
            {
                return;
            }

            var interactivity = ctx.Client.GetInteractivity();
            var emoji = DiscordEmoji.FromName(ctx.Client,":1:");

            var channelMessage = await ctx.Channel.SendMessageAsync(embed : pollStep.GetPollMessage()).ConfigureAwait(false);
        }
    }
}
