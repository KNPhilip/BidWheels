#nullable disable

namespace SearchService.IntegrationTests
{
    public class ConsumerTests(CustomWebAppFactory factory) : IClassFixture<CustomWebAppFactory>
    {
        private readonly ITestHarness _testHarness = factory.Services.GetTestHarness();
        private readonly Fixture _fixture = new();

        [Fact]
        public async Task AuctionCreated_ShouldCreateItemInDb()
        {
            // Arrange
            IConsumerTestHarness<AuctionCreatedConsumer> consumerHarness = _testHarness
                .GetConsumerHarness<AuctionCreatedConsumer>();
            AuctionCreated auction = _fixture.Create<AuctionCreated>();

            // Act
            await _testHarness.Bus.Publish(auction);

            // Assert
            Assert.True(await consumerHarness.Consumed.Any<AuctionCreated>());
            Item item = await DB.Find<Item>().OneAsync(auction.Id.ToString());
            Assert.Equal(auction.Make, item.Make);
        }
    }
}