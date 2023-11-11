using AuctionService.Contracts;
using AuctionService.Data;
using AuctionService.Entities;
using MassTransit;

namespace AuctionService.Consumers
{
    public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
    {
        private readonly AuctionContext _context;

        public AuctionFinishedConsumer(AuctionContext context)
        {
            _context = context;    
        }

        public async Task Consume(ConsumeContext<AuctionFinished> context)
        {
            Console.WriteLine("--> Consuming auction finished");

            Auction? auction = await _context.Auctions.FindAsync(context.Message.AuctionId);

            if (context.Message.ItemSold)
            {
                auction!.Winner = context.Message.Winner!;
                auction.SoldAmount = context.Message.Amount;
            }

            auction!.Status = auction.SoldAmount > auction.ReservePrice
                ? Status.Finished : Status.ReserveNotMet;

            await _context.SaveChangesAsync();
        }
    }
}