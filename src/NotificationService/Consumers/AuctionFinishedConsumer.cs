namespace NotificationService.Consumers
{
    public class AuctionFinishedConsumer(IHubContext<NotificationHub> hubContext) : IConsumer<AuctionFinished>
    {
        public async Task Consume(ConsumeContext<AuctionFinished> context)
        {
            Console.WriteLine($"--> Auction finished message recieved.");

            await hubContext.Clients.All.SendAsync("AuctionFinished", context.Message);
        }
    }
}