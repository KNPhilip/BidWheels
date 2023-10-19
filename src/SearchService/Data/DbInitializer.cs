using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Services;

namespace SearchService.Data
{
    public class DbInitializer
    {
        public static async Task InitDb(WebApplication app)
        {
            await DB.InitAsync("SearchDb", MongoClientSettings
            .FromConnectionString(app.Configuration.GetConnectionString("MongoDb")));

            await DB.Index<Item>()
                .Key(x => x.Make, KeyType.Text)
                .Key(x => x.Model, KeyType.Text)
                .Key(x => x.Color, KeyType.Text)
                .CreateAsync();

            Int64 count = await DB.CountAsync<Item>();

            using var scope = app.Services.CreateScope();

            AuctionServiceHttpClient httpClient = scope.ServiceProvider.GetRequiredService<AuctionServiceHttpClient>();

            List<Item> items = await httpClient.GetItemsForSearchDb();

            Console.WriteLine($"--> {items.Count} returned from the auction service");

            if (items.Count > 0) await DB.SaveAsync(items);
        }
    }
}