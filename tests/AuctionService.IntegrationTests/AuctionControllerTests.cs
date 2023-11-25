namespace AuctionService.IntegrationTests
{
    public class AuctionControllerTests : IClassFixture<CustomWebAppFactory>, IAsyncLifetime
    {
        private readonly CustomWebAppFactory _factory;
        private readonly HttpClient _httpClient;

        public AuctionControllerTests(CustomWebAppFactory factory)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient();
        }

        [Fact]
        public async Task GetAuctions_ShouldReturn10Auctions()
        {
            // Arrange

            // Act
            List<AuctionDto>? response = await _httpClient
                .GetFromJsonAsync<List<AuctionDto>>("/api/auctions");

            // Assert
            Assert.Equal(10, response?.Count);
        }

        public Task InitializeAsync() => Task.CompletedTask;

        public Task DisposeAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AuctionContext>();
            DataHelper.ReinitDbForTests(db);
            return Task.CompletedTask;
        }
    }
}