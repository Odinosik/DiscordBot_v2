using DiscordBot.Attributes;
using DiscordBot.Requests;
using DiscordBot.Requests.Enums;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using HtmlAgilityPack;
using System;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    public class LolCommands : BaseCommandModule
    {
        private readonly string _leagueIcon = "https://lolg-cdn.porofessor.gg/img/league-icons-v2/160/";
        private readonly string _summonerIcon = "https://lolg-cdn.porofessor.gg/img/summonerIcons/10.16/64/";
        private readonly string _championIcon = "https://lolg-cdn.porofessor.gg/img/champion-icons/10.18/64/";


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
                ImageUrl = _leagueIcon + tier.ToString("d") + "-" + rank.ToString("d") + ".png",
                Title = nick,
                Description = $"Lv: {summoner.summonerLevel}",
                Timestamp = DateTimeOffset.UtcNow,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Height = 100,
                    Width = 100,
                    Url = _summonerIcon + summoner.profileIconId + ".png"
                },
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    IconUrl = _summonerIcon + summoner.profileIconId + ".png",
                    Text = soloqRankedProfile.hotStreak ? "PepeHappy" : "PepeSad",
                }
            };

            summonerEmbed.AddField(tier.ToString("g"), $"W: {soloqRankedProfile.wins}", true);
            summonerEmbed.AddField(rank.ToString("g"), $"L: {soloqRankedProfile.losses}", true);
            var winratio = Math.Round((double)100 * soloqRankedProfile.wins / (soloqRankedProfile.losses + soloqRankedProfile.wins), 2);

            summonerEmbed.AddField($"WinRatio", $" {winratio.ToString()}%", true);

            await ctx.Channel.SendMessageAsync(embed: summonerEmbed).ConfigureAwait(false);
        }


        [Command("lastmatch")]
        [Description("Get Last match by nickname")]
        public async Task GetLastMatch(CommandContext ctx, [Description("nickname")] params string[] nickName)
        {
            var lolRequest = new LolRequests();
            var nick = String.Join(" ", nickName.ToArray());
            var summoner = lolRequest.GetSummoner(nick);

            var lastMatch = lolRequest.GetLastMatch(summoner.accountId)[0];

            var lastMatchByMatchId = lolRequest.GetLolMatchById(lastMatch.gameId.ToString());

            var partipicationId = lastMatchByMatchId.participantIdentities.Where(_ => _.player.accountId == summoner.accountId).Select(_ => _.participantId).FirstOrDefault();

            var partipication = lastMatchByMatchId.participants.FirstOrDefault(_ => _.participantId == partipicationId);

            var matchResult = lastMatchByMatchId.teams.Where(_ => _.teamId == partipication.teamId).Select(_ => _.win).FirstOrDefault();

            var allKillsInMatch = lastMatchByMatchId.participants.Where(_ => _.teamId == partipication.teamId).Sum(_ => _.stats.kills);

            var participationKills = Math.Round((double)100 * (partipication.stats.kills + partipication.stats.assists) / allKillsInMatch, 0);

            var summonerEmbed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Rose,
                Title = nick,
                Description = $"PLAYED --->",
                Timestamp = DateTimeOffset.UtcNow,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Height = 100,
                    Width = 100,
                    Url = GetChampionIconUrl(lastMatch.champion)
                },
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    IconUrl = GetSummonerIconUrl(summoner.profileIconId),
                }
            };
            summonerEmbed.AddField($"LANE: {lastMatch.lane}", $"KDA: {partipication.stats.kills}/{partipication.stats.deaths}/{partipication.stats.assists}");
            summonerEmbed.AddField($"Result: {matchResult}", $"CS -> {partipication.stats.totalMinionsKilled}");
            summonerEmbed.AddField($"K/P -> {participationKills}%", $"Vision Score -> {partipication.stats.visionScore}");

            await ctx.Channel.SendMessageAsync(embed: summonerEmbed).ConfigureAwait(false);
        }

        [Command("conter")]
        [Description("Get Last match by nickname")]
        public async Task GetConters(CommandContext ctx, [Description("nickname")] params string[] championName)
        {
            var name = String.Join("-", championName.ToArray()).ToLower();

            var nameForImage = String.Join("",championName.Select(x => FirstLetterToUpper(x)));

            HtmlWeb web = new HtmlWeb();
            HtmlDocument document = web.Load($"https://lolcounter.com/champions/{name}");
            HtmlNode[] nodes = document.DocumentNode.SelectNodes("//div[contains(@class,'left theinfo')]/a[contains(@class,'left')]/div[contains(@class,'name')]").ToArray();

            var ConterEmbed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Rose,
                Title = $"{nameForImage} Conters",
                Timestamp = DateTimeOffset.UtcNow,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Height = 100,
                    Width = 100,
                    Url = GetChampionIconUrlByName(nameForImage)
                },
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    IconUrl = GetChampionIconUrlByName(nameForImage),
                }
            };

            for (var i= 0; i<5;i+=2)
            {
                ConterEmbed.AddField($"{nodes[i].InnerText}", $"{nodes[i+1].InnerText}");
            }
            await ctx.Channel.SendMessageAsync(embed: ConterEmbed).ConfigureAwait(false);
        }

        private string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                str.ToLower();
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }

        private string GetChampionIconUrl(int id)
        {
            return _championIcon + id + ".png";
        }

        private string GetChampionIconUrlByName(string name)
        {
            return "http://ddragon.leagueoflegends.com/cdn/9.24.2/img/champion/" + name + ".png";
        }

        private string GetSummonerIconUrl(int id)
        {
            return _summonerIcon + id + ".png";
        }
    }
}
