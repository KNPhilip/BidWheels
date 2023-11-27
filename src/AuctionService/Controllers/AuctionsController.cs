using AuctionService.Dtos;
using AuctionService.Entities;
using AuctionService.Repositories;
using AutoMapper;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IAuctionRepository _auctionRepository;

        public AuctionsController(IMapper mapper, IPublishEndpoint publishEndpoint, IAuctionRepository auctionRepository)
        {
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _auctionRepository = auctionRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string? date) =>
            Ok(await _auctionRepository.GetAuctionsAsync(date!));

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuction(Guid id)
        {
            AuctionDto? auction = await _auctionRepository.GetAuctionAsync(id);
            return auction is null ? NotFound() : Ok(auction);
        }

        [HttpPost, Authorize]
        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto request)
        {
            Auction? auction = _mapper.Map<Auction>(request);

            auction.Seller = User.Identity!.Name!;

            _auctionRepository.AddAuction(auction);

            AuctionDto newAuction = _mapper.Map<AuctionDto>(auction);

            await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));
            
            bool success = await _auctionRepository.SaveChangesAsync();

            return success 
                ? CreatedAtAction(nameof(GetAuction),
                    new {auction.Id}, newAuction)
                : BadRequest("Could not save changes to the database.");
        }

        [HttpPut("{id}"), Authorize]
        public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto request)
        {
            Auction? auction = await _auctionRepository.GetAuctionEntityById(id);

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

            bool success = await _auctionRepository.SaveChangesAsync();

            return success ? Ok() : BadRequest("Problem saving changes..");
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            Auction? auction = await _auctionRepository.GetAuctionEntityById(id);

            if (auction is null)
                return NotFound();

            if (auction.Seller != User.Identity!.Name)
                return Forbid();

            _auctionRepository.RemoveAuction(auction);

            await _publishEndpoint.Publish<AuctionDeleted>(new { Id = auction.Id.ToString() });

            bool success = await _auctionRepository.SaveChangesAsync();

            return success ? Ok() : BadRequest("Could not update the database.");
        }
    }
}