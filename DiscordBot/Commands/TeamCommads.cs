using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    class TeamCommands : BaseCommandModule
    {
        [Command("join")]
        public async Task Join(CommandContext ctx)
        {

            var joinEmbed = new DiscordEmbedBuilder
            {
                Title = "Hidden role",
                ImageUrl = ctx.Client.CurrentUser.AvatarUrl,
                Color = DiscordColor.Azure,
            };

            var joinMessage = await ctx.Channel.SendMessageAsync(embed: joinEmbed).ConfigureAwait(false);

            var michalEmoji = DiscordEmoji.FromName(ctx.Client, ":michal_emotka:");
            var thumbsDownEmoji = DiscordEmoji.FromName(ctx.Client, ":-1:");

            await joinMessage.CreateReactionAsync(michalEmoji).ConfigureAwait(false);
            await joinMessage.CreateReactionAsync(thumbsDownEmoji).ConfigureAwait(false);

            var interacitibty = ctx.Client.GetInteractivity();
            var reactonResult = await interacitibty
                .WaitForReactionAsync(x => x.Message == joinMessage &&
                x.User.Id == ctx.User.Id &&
                (x.Emoji == michalEmoji || x.Emoji == thumbsDownEmoji), TimeSpan.FromSeconds(10))
                .ConfigureAwait(false);

            if (reactonResult.TimedOut == true)
            {
                await joinMessage.DeleteAsync().ConfigureAwait(false);
            }
            else if (reactonResult.Result.Emoji == michalEmoji)
            {
                var role = ctx.Guild.GetRole(693438162969559060);
                await ctx.Member.GrantRoleAsync(role);
            }
            else if (reactonResult.Result.Emoji == thumbsDownEmoji)
            {
                //Nothing
            }
            else
            {

            }
            await joinMessage.DeleteAsync().ConfigureAwait(false);
        }


        [Command("poll")]
        public async Task Poll(CommandContext ctx, TimeSpan duration, params DiscordEmoji[] emojiOptions)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var options = emojiOptions.Select(x => x.ToString());

            var pollEmbed = new DiscordEmbedBuilder
            {
                Title = "Poll",
                Description = string.Join(" ", options)
            };

            var pollMessage = await ctx.Channel.SendMessageAsync(embed: pollEmbed).ConfigureAwait(false);

            foreach (var option in emojiOptions)
            {
                await pollMessage.CreateReactionAsync(option).ConfigureAwait(false);
            }
            var result = await interactivity.CollectReactionsAsync(pollMessage, duration).ConfigureAwait(false);


            var results = result.GroupBy(x => x.Emoji).Select(y => $"{y.Key} : {y.Sum(s => s.Total)}").Distinct();


            await ctx.Channel.SendMessageAsync(string.Join("\n", results)).ConfigureAwait(false);
        }

        [Command("customemoji"), Description("Start a simple emoji poll for a simple yes/no question"), Cooldown(2, 30, CooldownBucketType.Guild)]
        public async Task CustomEmojiPollAsync(CommandContext clx, [Description("How long should the poll last. (e.g. 1m = 1 minute)")] TimeSpan duration, string question ,params DiscordEmoji[] discordEmojis)
        {
            if (!string.IsNullOrEmpty(question))
            {
                var client = clx.Client;
                var interactivity = client.GetInteractivity();
                var options = discordEmojis.Select(x => x.ToString());

                var pollEmbed = new DiscordEmbedBuilder
                {
                    Title = "Poll",
                    Description = question,
                    Color = DiscordColor.CornflowerBlue,
                    Timestamp = DateTimeOffset.Now
                }.WithAuthor(clx.Message.Author.Username);
                
                // Creating the poll message
                var pollStartText = new StringBuilder();
                pollStartText.Append("**").Append("Poll started for:").AppendLine("**");
                pollStartText.Append(question);
                var pollStartMessage = await clx.RespondAsync(embed: pollEmbed);


                // DoPollAsync adds automatically emojis out from an emoji array to a special message and waits for the "duration" of time to calculate results.
                var pollResult = await interactivity.DoPollAsync(pollStartMessage, discordEmojis, PollBehaviour.KeepEmojis, duration);

                var yesVotes = pollResult[0].Total;
                var noVotes = pollResult[1].Total;

                // Printing out the result
                var pollResultText = new StringBuilder();
                pollResultText.AppendLine(question);
                pollResultText.Append("Poll result: ");
                pollResultText.Append("**\n");
                
                foreach (var result in pollResult)
                {
                    pollResultText.AppendLine($"{result.Emoji} : {result.Total}");
                }


                await clx.RespondAsync(pollResultText.ToString());
            }
            else
            {
                await clx.RespondAsync("Error: the question can't be empty");
            }
        }


    }
}
