using DiscordBot.ResponseJson;

namespace DiscordBot.Requests
{
    public interface ILolrequests
    {
        public SummonerModelResponse GetSummoner(string nick);
    }
}
