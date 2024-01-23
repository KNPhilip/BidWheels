namespace AuctionService.IntegrationTests.Utils
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Removes the specified DbContext from the IServiceCollection.
        /// </summary>
        /// <typeparam name="T">The type of DbContext to remove.</typeparam>
        /// <param name="services">The IServiceCollection to remove the DbContext from.</param>
        public static void RemoveDbContext<T>(this IServiceCollection services)
        {
            ServiceDescriptor descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AuctionContext>))!;

            if (descriptor is not null)
                services.Remove(descriptor);
        }

        /// <summary>
        /// Ensures that the specified database is created and migrated for testing purposes.
        /// </summary>
        /// <typeparam name="T">The type of the database context.</typeparam>
        /// <param name="services">The service collection.</param>
        public static void EnsureCreated<T>(this IServiceCollection services)
        {
            ServiceProvider sp = services.BuildServiceProvider();

            using IServiceScope scope = sp.CreateScope();
            IServiceProvider scopedServices = scope.ServiceProvider;
            AuctionContext db = scopedServices.GetRequiredService<AuctionContext>();

            db.Database.Migrate();
            DataHelper.InitDbForTests(db);
        }
    }
}