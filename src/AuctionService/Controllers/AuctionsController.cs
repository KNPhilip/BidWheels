using System.Security.Claims;
using AuctionService.Data;
using AuctionService.Dtos;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionsController : ControllerBase
    {
        private readonly AuctionContext _context;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public AuctionsController(AuctionContext context, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string date)
        {
            var query = _context.Auctions.OrderBy(x => x.Item!.Make).AsQueryable();

            if (!string.IsNullOrEmpty(date))
                query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);

            return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuction(Guid id)
        {
            Auction? auction = await _context.Auctions
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (auction is null)
                return NotFound();

            return _mapper.Map<AuctionDto>(auction);
        }

        [HttpPost, Authorize]
        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto request)
        {
            Auction? auction = _mapper.Map<Auction>(request);
            auction.Seller = User.Identity!.Name!;

            _context.Auctions.Add(auction);

            AuctionDto newAuction = _mapper.Map<AuctionDto>(auction);

            await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));
            
            bool success = await _context.SaveChangesAsync() > 0;

            return success 
                ? CreatedAtAction(nameof(GetAuction),
                    new {auction.Id}, newAuction)
                : BadRequest("Could not save changes to the database.");
        }

        [HttpPut("{id}"), Authorize]
        public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto request)
        {
            Auction? auction = await _context.Auctions
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (auction is null)
                return NotFound();

            if (auction.Seller != User.Identity!.Name)
                return Forbid();

            auction.Item!.Make = request.Make ?? auction.Item.Make;
            auction.Item!.Model = request.Model ?? auction.Item.Model;
            auction.Item!.Color = request.Color ?? auction.Item.Color;
            auction.Item!.Mileage = request.Mileage ?? auction.Item.Mileage;
            auction.Item!.Year = request.Year ?? auction.Item.Year;

            await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));

            bool success = await _context.SaveChangesAsync() > 0;

            return success ? Ok() : BadRequest("Problem saving changes..");
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            Auction? auction = await _context.Auctions.FindAsync(id);

            if (auction is null)
                return NotFound();

            if (auction.Seller != User.Identity!.Name)
                return Forbid();

            _context.Auctions.Remove(auction);

            await _publishEndpoint.Publish<AuctionDeleted>(new { Id = auction.Id.ToString() });

            bool success = await _context.SaveChangesAsync() > 0;

            return success ? Ok() : BadRequest("Could not update the database.");
        }
    }
}