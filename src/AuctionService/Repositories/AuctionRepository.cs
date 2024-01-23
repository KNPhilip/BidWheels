using AuctionService.Data;
using AuctionService.Dtos;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Repositories
{
    public class AuctionRepository : IAuctionRepository
    {
        private readonly IMapper _mapper;
        private readonly AuctionContext _context;

        public AuctionRepository(AuctionContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddAuction(Auction request) =>
            _context.Auctions.Add(request);

        public async Task<AuctionDto?> GetAuctionAsync(Guid request) =>
            await _context.Auctions
                .ProjectTo<AuctionDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == request);

        public async Task<Auction?> GetAuctionEntityById(Guid request) =>
            await _context.Auctions
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == request);

        public async Task<List<AuctionDto>> GetAuctionsAsync(string request)
        {
            IQueryable<Auction> query = _context.Auctions.OrderBy(x => x.Item!.Make).AsQueryable();

            if (!string.IsNullOrEmpty(request))
                query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(request).ToUniversalTime()) > 0);

            return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public void RemoveAuction(Auction request) =>
            _context.Auctions.Remove(request);

        public async Task<bool> SaveChangesAsync() =>
            await _context.SaveChangesAsync() > 0;
    }
}