using AuctionService.Contracts;
using AuctionService.Data;
using AuctionService.Entities;
using MassTransit;

namespace AuctionService.Consumers
{
    public class BidPlacedConsumer : IConsumer<BidPlaced>
    {
        private readonly AuctionContext _context;

        public BidPlacedConsumer(AuctionContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<BidPlaced> context)
        {
            Console.WriteLine("--> Consuming bid placed");

            Auction? auction = await _context.Auctions.FindAsync(Guid.Parse(context.Message.AuctionId!));

            if (auction!.CurrentHighBid is null 
                || context.Message.BidStatus!.Contains("Accepted")
                && context.Message.Amount > auction.CurrentHighBid)
            {
                auction.CurrentHighBid = context.Message.Amount;
                await _context.SaveChangesAsync();
            }
        }
    }
}