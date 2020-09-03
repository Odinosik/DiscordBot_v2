using DiscordBot.Helper;
using DiscordBot.Requests.Enums;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DiscordBot.ResponseJson
{
    public class SummonerRankedResponse
    {
        public List<SummonerRankedModelResponse> SummonerRankedList { get; set; }
    }

    public class SummonerRankedModelResponse
    {
        public string leagueId { get; set; }
        public string queueType { get; set; }
        [JsonConverter(typeof(LolRanksConverter))]
        public string tier { get; set; }
        [JsonConverter(typeof(LolRomeConverter))]
        public string rank { get; set; }
        public string summonerId { get; set; }
        public string summonerName { get; set; }
        public int leaguePoints { get; set; }
        public int wins { get; set; }
        public int losses { get; set; }
        public bool veteran { get; set; }
        public bool inactive { get; set; }
        public bool freshBlood { get; set; }
        public bool hotStreak { get; set; }
    }
}
