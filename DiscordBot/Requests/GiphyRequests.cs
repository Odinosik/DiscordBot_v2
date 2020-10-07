using DiscordBot.Helper;
using DiscordBot.ResponseJson;
using Newtonsoft.Json;
using RestSharp;

namespace DiscordBot.Requests
{
    public class GiphyRequests
    {
        private string giphyToken = ConfigJson.GiphyToken;

        public string SendRequestRandomGif(string tag)
        {
            var client = new RestClient("https://api.giphy.com/v1/gifs");
            var request = new RestRequest("random", Method.GET);
            request.AddParameter("api_key", giphyToken);
            request.AddParameter("tag", tag);
            request.AddParameter("rating", "pg");
            var response = client.Get(request);
            GiphyModelResponse giphyModelResponse = JsonConvert.DeserializeObject<GiphyModelResponse>(response.Content);
            return giphyModelResponse.data.url;
        }
    }
}
