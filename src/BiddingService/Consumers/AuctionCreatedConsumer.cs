using BiddingService.Contracts;
using BiddingService.Models;
using MassTransit;
using MongoDB.Entities;

namespace BiddingService.Consumers
{
    public sealed class AuctionCreatedConsumer : IConsumer<AuctionCreated>
    {
        public async Task Consume(ConsumeContext<AuctionCreated> context)
        {
            Auction auction = new()
            {
                ID = context.Message.Id.ToString(),
                Seller = context.Message.Seller,
                AuctionEnd = context.Message.AuctionEnd,
                ReservePrice = context.Message.ReservePrice
            };

            await auction.SaveAsync();
        }
    }
}
