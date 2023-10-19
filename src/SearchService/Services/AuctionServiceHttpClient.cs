using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Services
{
    public class AuctionServiceHttpClient
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public AuctionServiceHttpClient(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<List<Item>> GetItemsForSearchDb()
        {
            string? lastupdated = await DB.Find<Item, string>()
                .Sort(x => x.Descending(x => x.UpdatedAt))
                .Project(x => x.UpdatedAt.ToString())
                .ExecuteFirstAsync();

            List<Item>? response = await _httpClient.GetFromJsonAsync<List<Item>>(
                _config["AuctionServiceUrl"]
                + "/api/auctions?date=" + lastupdated);

            return response!;
        }
    }
}