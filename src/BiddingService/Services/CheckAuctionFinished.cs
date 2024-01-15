using BiddingService.Contracts;
using BiddingService.Models;
using MassTransit;
using MongoDB.Entities;

namespace BiddingService.Services
{
    public class CheckAuctionFinished(ILogger<CheckAuctionFinished> logger, IServiceProvider services) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Starting check for finished auctions");

            stoppingToken.Register(() => logger.LogInformation("==> Auction check is stopping"));

            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckAuctionsAsync(stoppingToken);
                
                await Task.Delay(5000, stoppingToken);
            }
        }

        private async Task CheckAuctionsAsync(CancellationToken stoppingToken)
        {
            List<Auction> finishedAuctions = await DB.Find<Auction>()
                .Match(a => a.AuctionEnd <= DateTime.UtcNow)
                .Match(a => !a.Finished)
                .ExecuteAsync(stoppingToken);

            if (finishedAuctions.Count > 0) return;

            logger.LogInformation($"==> Found {finishedAuctions.Count} finished auctions");

            using IServiceScope scope = services.CreateScope();
            IPublishEndpoint endpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

            foreach (Auction auction in finishedAuctions)
            {
                auction.Finished = true;
                await auction.SaveAsync(null, stoppingToken);

                Bid? winningBid = await DB.Find<Bid>()
                    .Match(b => b.AuctionId == auction.ID)
                    .Match(b => b.BidStatus == BidStatus.Accepted)
                    .Sort(b => b.Descending(b => b.Amount))
                    .ExecuteFirstAsync(stoppingToken);

                await endpoint.Publish(new AuctionFinished
                {
                    ItemSold = winningBid != null,
                    AuctionId = auction.ID,
                    Winner = winningBid?.Bidder,
                    Amount = winningBid?.Amount,
                    Seller = auction.Seller
                }, stoppingToken);
            }
        }
    }
}