namespace AuctionService.IntegrationTests.Fixtures
{
    /// <summary>
    /// Represents a custom web application factory used for integration testing.
    /// </summary>
    public sealed class CustomWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        /// <summary>
        /// Represents a PostgreSqlContainer used for integration testing.
        /// </summary>
        private readonly PostgreSqlContainer _postgresSqlContainer = new PostgreSqlBuilder().Build();
        
        public async Task InitializeAsync() =>
            await _postgresSqlContainer.StartAsync();

        async Task IAsyncLifetime.DisposeAsync() =>
            await _postgresSqlContainer.DisposeAsync().AsTask();

        protected sealed override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveDbContext<AuctionContext>();

                services.AddDbContext<AuctionContext>(options =>
                {
                    options.UseNpgsql(_postgresSqlContainer.GetConnectionString());
                });

                services.AddMassTransitTestHarness();

                services.EnsureCreated<AuctionContext>();

                services.AddAuthentication(FakeJwtBearerDefaults.AuthenticationScheme)
                    .AddFakeJwtBearer(options =>
                    {
                        options.BearerValueType = FakeJwtBearerBearerValueType.Jwt;
                    });
            });
        }
    }
}
