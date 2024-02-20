using AuctionService.Data;
using Grpc.Core;

namespace AuctionService.Services
{
    public sealed class GrpcAuctionService(AuctionContext dbContext) : GrpcAuction.GrpcAuctionBase
    {
        public sealed override async Task<GrpcAuctionResponse> GetAuction(GetAuctionRequest request, ServerCallContext context)
        {
            Console.WriteLine("==> Recieved gRPC request for auction");

            Entities.Auction? auction = await dbContext.Auctions.FindAsync(Guid.Parse(request.Id)) 
                ?? throw new RpcException(new Status(StatusCode.NotFound, "Not found"));

            GrpcAuctionResponse response = new()
            {
                Auction = new GrpcAuctionModel
                {
                    AuctionEnd = auction.AuctionEnd.ToString(),
                    Id = auction.Id.ToString(),
                    ReservePrice = auction.ReservePrice,
                    Seller = auction.Seller
                }
            };

            return response;
        }
    }
}
