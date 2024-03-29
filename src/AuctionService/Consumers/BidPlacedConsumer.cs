using AuctionService.Contracts;
using AuctionService.Data;
using AuctionService.Entities;
using MassTransit;

namespace AuctionService.Consumers
{
    public sealed class BidPlacedConsumer(AuctionContext dbContext) : IConsumer<BidPlaced>
    {
        public async Task Consume(ConsumeContext<BidPlaced> context)
        {
            Console.WriteLine("--> Consuming bid placed");

            Auction? auction = await dbContext.Auctions.FindAsync(Guid.Parse(context.Message.AuctionId!));

            if (auction!.CurrentHighBid is null 
                || context.Message.BidStatus!.Contains("Accepted")
                && context.Message.Amount > auction.CurrentHighBid)
            {
                auction.CurrentHighBid = context.Message.Amount;
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
