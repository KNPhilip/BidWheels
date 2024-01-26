using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Services
{
    public class AuctionServiceHttpClient(HttpClient httpClient, IConfiguration config)
    {
        public async Task<List<Item>> GetItemsForSearchDb()
        {
            string? lastupdated = await DB.Find<Item, string>()
                .Sort(x => x.Descending(x => x.UpdatedAt))
                .Project(x => x.UpdatedAt.ToString())
                .ExecuteFirstAsync();

            List<Item>? response = await httpClient.GetFromJsonAsync<List<Item>>(
                config["AuctionServiceUrl"]
                + "/api/auctions?date=" + lastupdated);

            return response!;
        }
    }
}