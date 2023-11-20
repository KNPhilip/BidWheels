using AuctionService.Dtos;
using AuctionService.Entities;

namespace AuctionService.Repositories
{
    public interface IAuctionRepository
    {
        Task<List<AuctionDto>> GetAuctionsAsync(string request);
        Task<AuctionDto?> GetAuctionAsync(Guid request);
        Task<Auction?> GetAuctionEntityById(Guid request);
        void AddAuction(Auction request);
        void RemoveAuction(Auction request);
        Task<bool> SaveChangesAsync();
    }
}