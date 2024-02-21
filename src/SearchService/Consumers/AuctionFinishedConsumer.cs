using MassTransit;
using MongoDB.Entities;
using SearchService.Contracts;
using SearchService.Models;

namespace SearchService.Consumers
{
    public sealed class AuctionFinishedConsumer : IConsumer<AuctionFinished>
    {
        public async Task Consume(ConsumeContext<AuctionFinished> context)
        {
            Item? auction = await DB.Find<Item>().OneAsync(context.Message.AuctionId!);

            if (context.Message.ItemSold)
            {
                auction!.Winner = context.Message.Winner;
                auction.SoldAmount = (int)context.Message.Amount!;
            }

            auction!.Status = "Finished";

            await auction.SaveAsync();
        }
    }
}
