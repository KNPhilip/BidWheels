namespace AuctionService.IntegrationTests
{
    [Collection("SharedFixture")]
    public class AuctionControllerTests : IAsyncLifetime
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
        public async Task GetAuction_WithValidGuid_ReturnAuction()
        {
            // Arrange

            // Act
            HttpResponseMessage response = await _httpClient.GetAsync($"api/auctions/{carGuid}");

            // Assert
            response.EnsureSuccessStatusCode();
            AuctionDto? auction = await response.Content.ReadFromJsonAsync<AuctionDto>();
            Assert.Equal("GT", auction!.Model);
        }

        [Fact]
        public async Task GetAuction_WithNotExistingGuid_Returns404NotFound()
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

        [Fact]
        public async Task CreateAuction_WithNoAuth_Returns401Unauthorized()
        {
            // Arrange
            CreateAuctionDto request = new()
            {
                Make = "Ford",
            };

            // Act
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"api/auctions", request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateAuction_Valid_Returns201Created()
        {
            // Arrange
            string seller = "Philip";
            CreateAuctionDto request = GetAuctionForCreate();
            _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser(seller));

            // Act
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"api/auctions", request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            AuctionDto? createdAuction = await response.Content.ReadFromJsonAsync<AuctionDto>()!;
            Assert.Equal(seller, createdAuction!.Seller);
        }

        [Fact]
        public async Task CreateAuction_WithInvalidCreateAuctionDto_Returns400BadRequest()
        {
            // Arrange
            CreateAuctionDto request = GetAuctionForCreate();
            request.Make = null;
            _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));

            // Act
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"api/auctions", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateAuction_WithValidUpdateDtoAndUser_Returns200OK()
        {
            // Arrange
            UpdateAuctionDto request = new() 
            {
                Color = "Blue"
            };
            _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));

            // Act
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/auctions/{carGuid}", request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UpdateAuction_WithValidUpdateDtoAndInvalidUser_Returns403Forbidden()
        {
            // Arrange
            UpdateAuctionDto request = new() 
            {
                Color = "Blue"
            };
            _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("Philip"));

            // Act
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/auctions/{carGuid}", request);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
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