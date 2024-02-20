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
    public sealed class AuctionsController(IMapper mapper, IPublishEndpoint publishEndpoint, 
        IAuctionRepository auctionRepository) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string? date) =>
            Ok(await auctionRepository.GetAuctionsAsync(date!));

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuction(Guid id)
        {
            AuctionDto? auction = await auctionRepository.GetAuctionAsync(id);
            return auction is null ? NotFound() : Ok(auction);
        }

        [HttpPost, Authorize]
        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto request)
        {
            Auction? auction = mapper.Map<Auction>(request);

            auction.Seller = User.Identity!.Name!;

            auctionRepository.AddAuction(auction);

            AuctionDto newAuction = mapper.Map<AuctionDto>(auction);

            await publishEndpoint.Publish(mapper.Map<AuctionCreated>(newAuction));
            
            bool success = await auctionRepository.SaveChangesAsync();

            return success 
                ? CreatedAtAction(nameof(GetAuction),
                    new {auction.Id}, newAuction)
                : BadRequest("Could not save changes to the database.");
        }

        [HttpPut("{id}"), Authorize]
        public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto request)
        {
            Auction? auction = await auctionRepository.GetAuctionEntityById(id);

            if (auction is null)
                return NotFound();

            if (auction.Seller != User.Identity!.Name)
                return Forbid();

            auction.Item!.Make = request.Make ?? auction.Item.Make;
            auction.Item!.Model = request.Model ?? auction.Item.Model;
            auction.Item!.Color = request.Color ?? auction.Item.Color;
            auction.Item!.Mileage = request.Mileage ?? auction.Item.Mileage;
            auction.Item!.Year = request.Year ?? auction.Item.Year;

            await publishEndpoint.Publish(mapper.Map<AuctionUpdated>(auction));

            bool success = await auctionRepository.SaveChangesAsync();

            return success ? Ok() : BadRequest("Problem saving changes..");
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            Auction? auction = await auctionRepository.GetAuctionEntityById(id);

            if (auction is null)
                return NotFound();

            if (auction.Seller != User.Identity!.Name)
                return Forbid();

            auctionRepository.RemoveAuction(auction);

            await publishEndpoint.Publish<AuctionDeleted>(new { Id = auction.Id.ToString() });

            bool success = await auctionRepository.SaveChangesAsync();

            return success ? Ok() : BadRequest("Could not update the database.");
        }
    }
}
