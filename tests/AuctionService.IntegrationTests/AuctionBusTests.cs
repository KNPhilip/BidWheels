namespace AuctionService.IntegrationTests
{
    [Collection("SharedFixture")]
    public sealed class AuctionBusTests : IAsyncLifetime
    {
        private readonly CustomWebAppFactory _factory;
        private readonly HttpClient _httpClient;
        private readonly ITestHarness _testHarness;

        public AuctionBusTests(CustomWebAppFactory factory)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient();
            _testHarness = _factory.Services.GetTestHarness();
        }

        [Fact]
        public async Task CreateAuction_WithValidObject_ShouldPublishAuctionCreated()
        {
            // Arrange
            var auction = GetAuctionForCreate();
            _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));

            // Act
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/auctions", auction);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.True(await _testHarness.Published.Any<AuctionCreated>());
        }

        public Task InitializeAsync() => Task.CompletedTask;

        public Task DisposeAsync()
        {
            using IServiceScope scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AuctionContext>();
            DataHelper.ReinitDbForTests(db);
            return Task.CompletedTask;
        }

        private static CreateAuctionDto GetAuctionForCreate() =>
            new()
            {
                Make = "test",
                Model = "testModel",
                ImageUrl = "test",
                Year = 2005,
                Mileage = 10000,
                ReservePrice = 10,
                Color = "Red"
            };
    }
}
