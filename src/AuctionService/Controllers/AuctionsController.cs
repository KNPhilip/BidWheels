using AuctionService.Data;
using AuctionService.Dtos;
using AuctionService.Entities;
using AutoMapper;
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

        public AuctionsController(AuctionContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
        {
            List<Auction> auctions = await _context.Auctions
                .Include(x => x.Item)
                .OrderBy(x => x.Item!.Make)
                .ToListAsync();

            return _mapper.Map<List<AuctionDto>>(auctions);
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

        [HttpPost]
        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto request)
        {
            Auction? auction = _mapper.Map<Auction>(request);
            // TODO: add current user as seller
            auction.Seller = "Bob";

            _context.Auctions.Add(auction);
            bool success = await _context.SaveChangesAsync() > 0;

            return success 
                ? CreatedAtAction(nameof(GetAuction),
                    new {auction.Id}, _mapper.Map<AuctionDto>(auction))
                : BadRequest("Could not save changes to the database.");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto request)
        {
            Auction? auction = await _context.Auctions
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (auction is null)
                return NotFound();

            // TODO: check seller == username

            auction.Item!.Make = request.Make ?? auction.Item.Make;
            auction.Item!.Model = request.Model ?? auction.Item.Model;
            auction.Item!.Color = request.Color ?? auction.Item.Color;
            auction.Item!.Mileage = request.Mileage ?? auction.Item.Mileage;
            auction.Item!.Year = request.Year ?? auction.Item.Year;

            bool success = await _context.SaveChangesAsync() > 0;

            return success ? Ok() : BadRequest("Problem saving changes..");
        }
    }
}