using Newtonsoft.Json;

namespace DiscordBot.ResponseJson
{
    public class GiphyModelResponse
    {
        public Data data;
    }
    public class Data
    {
        [JsonProperty("url")]
        public string url { get; set; }
    }
}
