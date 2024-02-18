namespace SearchService.IntegrationTests
{
    public class CustomWebAppFactory : WebApplicationFactory<Program>
    {
        private readonly MongoDbRunner _runner;

        public CustomWebAppFactory()
        {
            _runner = MongoDbRunner.Start();
            DB.InitAsync("test-db", MongoClientSettings.FromConnectionString(_runner.ConnectionString));

            DB.Index<Item>()
                .Key(i => i.Make, KeyType.Text)
                .Key(i => i.Model, KeyType.Text)
                .Key(i => i.Color, KeyType.Text)
                .CreateAsync();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureTestServices(services => 
            {
                services.AddMassTransitTestHarness();
            });
        }

        protected override void Dispose(bool disposing)
        {
            _runner.Dispose();
        }
    }
}
