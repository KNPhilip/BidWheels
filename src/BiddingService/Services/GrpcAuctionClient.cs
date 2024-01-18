using AuctionService;
using BiddingService.Models;
using Grpc.Net.Client;

namespace BiddingService.Services
{
    public class GrpcAuctionClient(ILogger<GrpcAuctionClient> logger, IConfiguration config)
    {
        public Auction GetAuction(string id)
        {
            logger.LogInformation("Calling GRPC Service");

            GrpcChannel channel = GrpcChannel.ForAddress(config["GrpcAuction"]!);
            GrpcAuction.GrpcAuctionClient client = new(channel);
            GetAuctionRequest request = new() 
            {
                Id = id
            };

            try 
            {
                GrpcAuctionResponse reply = client.GetAuction(request);
                Auction auction = new()
                {
                    ID = reply.Auction.Id,
                    AuctionEnd = DateTime.Parse(reply.Auction.AuctionEnd),
                    Seller = reply.Auction.Seller,
                    ReservePrice = reply.Auction.ReservePrice
                };

                return auction;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Could not call gRPC server");
                return null!;
            }
        }
    }
}