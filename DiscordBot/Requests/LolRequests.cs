using DiscordBot.Helper;
using DiscordBot.ResponseJson;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;

namespace DiscordBot.Requests
{
    public class LolRequests : ILolrequests
    {
        private string lolToken = ConfigHelper.LolToken;
        private string _baseUrl = "https://eun1.api.riotgames.com/lol";

        public SummonerModelResponse GetSummoner(string nick)
        {

            var client = new RestClient(_baseUrl);
            var request = new RestRequest("summoner/v4/summoners/by-name/{nick}", Method.GET);
            request.AddUrlSegment("nick", nick);
            request.AddParameter("api_key", lolToken);
            var response = client.Get(request);
            SummonerModelResponse summonerModelResponse = JsonConvert.DeserializeObject<SummonerModelResponse>(response.Content);
            return summonerModelResponse;
        }

        public List<SummonerRankedModelResponse> GetRankedSummoner(string summonerId)
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest("league/v4/entries/by-summoner/{summonerId}", Method.GET);
            request.AddUrlSegment("summonerId", summonerId);
            request.AddParameter("api_key", lolToken);
            var response = client.Get(request);
            try
            {
                var summonerRankedModelResponse = JsonConvert.DeserializeObject< List<SummonerRankedModelResponse>>(response.Content);
                return summonerRankedModelResponse;
            }
            catch (Exception e)
            {
                var a = e.Message;
            }
            return null;
        }
    }
}
