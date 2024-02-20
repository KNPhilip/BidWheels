using AuctionService.Data;
using AuctionService.Dtos;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Repositories
{
    public sealed class AuctionRepository(AuctionContext context, IMapper mapper) : IAuctionRepository
    {
        public void AddAuction(Auction request) =>
            context.Auctions.Add(request);

        public async Task<AuctionDto?> GetAuctionAsync(Guid request) =>
            await context.Auctions
                .ProjectTo<AuctionDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == request);

        public async Task<Auction?> GetAuctionEntityById(Guid request) =>
            await context.Auctions
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == request);

        public async Task<List<AuctionDto>> GetAuctionsAsync(string request)
        {
            IQueryable<Auction> query = context.Auctions.OrderBy(x => x.Item!.Make).AsQueryable();

            if (!string.IsNullOrEmpty(request))
                query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(request).ToUniversalTime()) > 0);

            return await query.ProjectTo<AuctionDto>(mapper.ConfigurationProvider).ToListAsync();
        }

        public void RemoveAuction(Auction request) => context.Auctions.Remove(request);

        public async Task<bool> SaveChangesAsync() => await context.SaveChangesAsync() > 0;
    }
}
