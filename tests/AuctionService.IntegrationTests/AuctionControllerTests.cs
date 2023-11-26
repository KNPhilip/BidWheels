using System.Net;

namespace AuctionService.IntegrationTests
{
    public class AuctionControllerTests : IClassFixture<CustomWebAppFactory>, IAsyncLifetime
    {
        private readonly CustomWebAppFactory _factory;
        private readonly HttpClient _httpClient;
        private const string carGuid = "afbee524-5972-4075-8800-7d1f9d7b0a0c";

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
            List<AuctionDto>? response = await _httpClient.GetFromJsonAsync<List<AuctionDto>>("api/auctions");

            // Assert
            Assert.Equal(10, response!.Count);
        }

        [Fact]
        public async Task GetAuction_WithValidId_ReturnAuction()
        {
            // Arrange

            // Act
            AuctionDto? response = await _httpClient.GetFromJsonAsync<AuctionDto>($"api/auctions/{carGuid}");

            // Assert
            Assert.Equal("GT", response!.Model);
        }

        [Fact]
        public async Task GetAuction_WithInvalidId_Returns404NotFound()
        {
            // Arrange

            // Act
            HttpResponseMessage response = await _httpClient.GetAsync($"api/auctions/{Guid.NewGuid()}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAuction_WithInvalidGuid_Returns400BadRequest()
        {
            // Arrange

            // Act
            HttpResponseMessage response = await _httpClient.GetAsync($"api/auctions/notaguid");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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