namespace NotificationService.Consumers
{
    public sealed class BidPlacedConsumer(IHubContext<NotificationHub> hubContext) : IConsumer<BidPlaced>
    {
        public async Task Consume(ConsumeContext<BidPlaced> context)
        {
            Console.WriteLine($"--> Bid placed message recieved.");

            await hubContext.Clients.All.SendAsync("BidPlaced", context.Message);
        }
    }
}
