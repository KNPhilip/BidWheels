namespace NotificationService.Consumers
{
    public class AuctionCreatedConsumer(IHubContext<NotificationHub> hubContext) : IConsumer<AuctionCreated>
    {
        public async Task Consume(ConsumeContext<AuctionCreated> context)
        {
            Console.WriteLine($"--> Auction created message recieved.");

            await hubContext.Clients.All.SendAsync("AuctionCreated", context.Message);
        }
    }
}