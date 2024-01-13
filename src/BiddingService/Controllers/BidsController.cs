using BiddingService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;

namespace BiddingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BidsController : ControllerBase
    {
        [HttpPost, Authorize]
        public async Task<ActionResult<Bid>> PlaceBid(string auctionId, int amount)
        {
            Auction? auction = await DB.Find<Auction>().OneAsync(auctionId);

            if (auction is null)
            {
                // TODO: check with auction service if that has auction
                return NotFound();
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

            return Ok(bid);
        }

        [HttpGet("{auctionId}")]
        public async Task<ActionResult<List<Bid>>> GetBidsForAuction(string auctionId)
        {
            List<Bid>? bids = await DB.Find<Bid>()
                .Match(b => b.AuctionId == auctionId)
                .Sort(b => b.Descending(b => b.BidTime))
                .ExecuteAsync();

            return bids;
        }
    }
}