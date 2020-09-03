using DiscordBot.Attributes;
using DiscordBot.Requests;
using DiscordBot.Requests.Enums;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    public class LolCommands : BaseCommandModule
    {
        [Command("summoner")]
        [Description("Get Summoner Profile by NickName")]
        public async Task GetSummoner(CommandContext ctx, [Description("nickname")] params string[] nickName)
        {
            var lolRequest = new LolRequests();

            var nick = String.Join(" ", nickName.ToArray());
            var summoner = lolRequest.GetSummoner(nick);
            var summonerRankedProfile = lolRequest.GetRankedSummoner(summoner.id);
            var soloqRankedProfile = summonerRankedProfile.Where(x => x.queueType == "RANKED_SOLO_5x5").SingleOrDefault();

            Enum.TryParse(soloqRankedProfile.tier, out eLolRanks tier);
            Enum.TryParse(soloqRankedProfile.rank, out eLolRome rank);

            var summonerEmbed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Rose,
                ImageUrl = "https://lolg-cdn.porofessor.gg/img/league-icons-v2/160/" + tier.ToString("d") + "-" + rank.ToString("d") + ".png",
                Title = nick,
                Description = $"Lv: {summoner.summonerLevel}",
                Timestamp = DateTimeOffset.UtcNow,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Height = 100,
                    Width = 100,
                    Url= "https://lolg-cdn.porofessor.gg/img/summonerIcons/10.16/64/" + summoner.profileIconId + ".png"
                },
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    IconUrl = "https://lolg-cdn.porofessor.gg/img/summonerIcons/10.16/64/" + summoner.profileIconId + ".png",
                    Text = soloqRankedProfile.hotStreak ? "PepeHappy" : "PepeSad",
                }
            };

            summonerEmbed.AddField(tier.ToString("g"), $"W: {soloqRankedProfile.wins}", true);
            summonerEmbed.AddField(rank.ToString("g"), $"L: {soloqRankedProfile.losses}",true);
            var winratio = Math.Round((double)100 * soloqRankedProfile.wins / (soloqRankedProfile.losses + soloqRankedProfile.wins), 2);

            summonerEmbed.AddField($"WinRatio", $" {winratio.ToString()}%",true);

            await ctx.Channel.SendMessageAsync(embed: summonerEmbed).ConfigureAwait(false);
        }
    }
}
