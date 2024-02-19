using MassTransit;
using MongoDB.Entities;
using SearchService.Contracts;
using SearchService.Models;

namespace SearchService.Consumers
{
    public sealed class BidPlacedConsumer : IConsumer<BidPlaced>
    {
        public async Task Consume(ConsumeContext<BidPlaced> context)
        {
            Console.WriteLine("--> Consuming bid placed");

            Item? auction = await DB.Find<Item>().OneAsync(context.Message.AuctionId!);

            if (context.Message.BidStatus!.Contains("Accepted") 
                && context.Message.Amount > auction!.CurrentHighBid)
            {
                auction.CurrentHighBid = context.Message.Amount;
                await auction.SaveAsync();
            }
        }
    }
}
