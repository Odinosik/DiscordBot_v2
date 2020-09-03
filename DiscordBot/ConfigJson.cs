using Newtonsoft.Json;

namespace DiscordBot
{
    public class ConfigJson
    {

        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("prefix")]
        public string Prefix { get; private set; }

        [JsonProperty("giphytoken")]
        public string GiphyToken { get; private set; }

        [JsonProperty("loltoken")]
        public string LolToken { get; private set; }
    }
}
