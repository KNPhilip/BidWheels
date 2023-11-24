namespace AuctionService.IntegrationTests.Fixtures
{
    /// <summary>
    /// Represents a custom web application factory used for integration testing.
    /// </summary>
    public class CustomWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        /// <summary>
        /// Represents a PostgreSqlContainer used for integration testing.
        /// </summary>
        private readonly PostgreSqlContainer _postgresSqlContainer = new PostgreSqlBuilder().Build();
        
        public async Task InitializeAsync() =>
            await _postgresSqlContainer.StartAsync();

        async Task IAsyncLifetime.DisposeAsync() =>
            await _postgresSqlContainer.DisposeAsync().AsTask();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                // Replace the existing AuctionContext with a new one 
                // that uses the Testcontainers Postgres instance.
                ServiceDescriptor descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<AuctionContext>))!;

                if (descriptor is not null)
                    services.Remove(descriptor);

                services.AddDbContext<AuctionContext>(options =>
                {
                    options.UseNpgsql(_postgresSqlContainer.GetConnectionString());
                });

                // Migrate the test database.
                ServiceProvider sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AuctionContext>();

                db.Database.Migrate();

                // Add the MassTransitTestHarness to the service collection.
                services.AddMassTransitTestHarness();
            });
        }
    }
}