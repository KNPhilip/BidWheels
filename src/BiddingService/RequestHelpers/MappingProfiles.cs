using AutoMapper;
using BiddingService.Contracts;
using BiddingService.Dtos;
using BiddingService.Models;

namespace BiddingService.RequestHelpers
{
    public sealed class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Bid, BidDto>();
            CreateMap<Bid, BidPlaced>();
        }
    }
}
