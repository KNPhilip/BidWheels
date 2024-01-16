using AutoMapper;
using BiddingService.Contracts;
using BiddingService.Dtos;
using BiddingService.Models;
using BiddingService.Services;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;

namespace BiddingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BidsController(IMapper mapper, IPublishEndpoint publishEndpoint, 
        GrpcAuctionClient grpcClient) : ControllerBase
    {
        [HttpPost, Authorize] 
        public async Task<ActionResult<BidDto>> PlaceBid(string auctionId, int amount)
        {
            Auction? auction = await DB.Find<Auction>().OneAsync(auctionId);

            if (auction is null)
            {
                auction = grpcClient.GetAuction(auctionId);

                if (auction is null)
                    return BadRequest("Cannot accept bids on this auction at this time.");
            }

            if (auction.Seller == User.Identity!.Name)
                return BadRequest("You cannot bid on your own auction");

            Bid bid = new()
            {
                AuctionId = auctionId,
                Bidder = User.Identity!.Name,
                Amount = amount
            };

            if (auction.AuctionEnd < DateTime.UtcNow)
                bid.BidStatus = BidStatus.Finished;
            else
            {
                Bid? highBid = await DB.Find<Bid>()
                    .Match(b => b.AuctionId == auctionId)
                    .Sort(b => b.Descending(b => b.Amount))
                    .ExecuteFirstAsync();

                if (highBid is not null && amount > highBid.Amount || highBid is null)
                    bid.BidStatus = amount > auction.ReservePrice 
                        ? BidStatus.Accepted : BidStatus.AcceptedBelowReserve;

                if (highBid is not null && bid.Amount <= highBid.Amount)
                    bid.BidStatus = BidStatus.TooLow;
            }

            await DB.SaveAsync(bid);

            await publishEndpoint.Publish(mapper.Map<BidPlaced>(bid));

            return Ok(mapper.Map<BidDto>(bid));
        }

        [HttpGet("{auctionId}")]
        public async Task<ActionResult<List<BidDto>>> GetBidsForAuction(string auctionId)
        {
            List<Bid>? bids = await DB.Find<Bid>()
                .Match(b => b.AuctionId == auctionId)
                .Sort(b => b.Descending(b => b.BidTime))
                .ExecuteAsync();

            return bids.Select(mapper.Map<BidDto>).ToList();
        }
    }
}