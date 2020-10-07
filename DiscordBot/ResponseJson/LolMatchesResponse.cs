using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.ResponseJson
{
    public class LolMatchesResponse
    {
        public string platformId { get; set; }
        public long gameId { get; set; }
        public int champion { get; set; }
        public int queue { get; set; }
        public int season { get; set; }
        public long timestamp { get; set; }
        public string role { get; set; }
        public string lane { get; set; }
    }

    public class LolHistory
    {
        public List<LolMatchesResponse> matches { get; set; }
    }
}
