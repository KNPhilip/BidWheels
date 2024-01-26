using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.RequestHelpers;

namespace SearchService.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Item>>> SearchItems([FromQuery] SearchParams searchParams)
        {
            PagedSearch<Item, Item> query = DB.PagedSearch<Item, Item>();

            if (!string.IsNullOrEmpty(searchParams.SearchTerm))
                query.Match(Search.Full, searchParams.SearchTerm).SortByTextScore();

            query = searchParams.OrderBy switch
            {
                "make" => query.Sort(x => x.Ascending(a => a.Make))
                    .Sort(x => x.Ascending(a => a.Model)),
                "new" => query.Sort(x => x.Descending(a => a.CreatedAt)),
                // Underscore is the default ordering option.
                _ => query.Sort(x => x.Ascending(a => a.AuctionEnd))
            };

            query = searchParams.FilterBy switch
            {
                "finished" => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
                "endingSoon" => query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(6)
                    && x.AuctionEnd > DateTime.UtcNow),
                // Underscore is the default filtering option.
                _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow)
            };

            if (!string.IsNullOrEmpty(searchParams.Seller))
                query.Match(x => x.Seller == searchParams.Seller);

            if (!string.IsNullOrEmpty(searchParams.Winner))
                query.Match(x => x.Seller == searchParams.Winner);

            query.PageNumber(searchParams.PageNumber);
            query.PageSize(searchParams.PageSize);

            (IReadOnlyList<Item> Results, long TotalCount, int PageCount) result = await query.ExecuteAsync();

            return Ok(new
            {
                results = result.Results,
                pageCount = result.PageCount,
                totalCount = result.TotalCount
            });
        }
    }
}