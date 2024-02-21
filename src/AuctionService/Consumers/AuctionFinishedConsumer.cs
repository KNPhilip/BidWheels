using AuctionService.Contracts;
using AuctionService.Data;
using AuctionService.Entities;
using MassTransit;

namespace AuctionService.Consumers
{
    public sealed class AuctionFinishedConsumer(AuctionContext dbContext) : IConsumer<AuctionFinished>
    {
        public async Task Consume(ConsumeContext<AuctionFinished> context)
        {
            Console.WriteLine("--> Consuming auction finished");

            Auction? auction = await dbContext.Auctions.FindAsync(Guid.Parse(context.Message.AuctionId!));

            if (context.Message.ItemSold)
            {
                auction!.Winner = context.Message.Winner!;
                auction.SoldAmount = context.Message.Amount;
            }

            auction!.Status = auction.SoldAmount > auction.ReservePrice
                ? Status.Finished : Status.ReserveNotMet;

            await dbContext.SaveChangesAsync();
        }
    }
}
