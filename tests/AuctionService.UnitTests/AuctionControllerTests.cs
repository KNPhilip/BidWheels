using AuctionService.Controllers;
using AuctionService.Dtos;
using AuctionService.Repositories;
using AuctionService.RequestHelpers;
using AutoFixture;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AuctionService.UnitTests
{
    public class AuctionControllerTests
    {
        private readonly Mock<IAuctionRepository> _auctionRepository;
        private readonly Mock<IPublishEndpoint> _publishEndpoint;
        private readonly Fixture _fixture;
        private readonly AuctionsController _auctionController;
        private readonly IMapper _mapper;

        public AuctionControllerTests()
        {
            _fixture = new Fixture();
            _auctionRepository = new Mock<IAuctionRepository>();
            _publishEndpoint = new Mock<IPublishEndpoint>();

            MapperConfiguration mockMapper = new(mc => 
            {
                mc.AddMaps(typeof(MappingProfiles).Assembly);
            });

            _mapper = new Mapper(mockMapper);
            _auctionController = new AuctionsController(_mapper, _publishEndpoint.Object, _auctionRepository.Object);
        }

        [Fact]
        public async Task GetAuctions_WithNoParams_Returns10Auctions()
        {
            // Arrange
            List<AuctionDto> auctions = _fixture.CreateMany<AuctionDto>(10).ToList();
            _auctionRepository.Setup(repo => repo.GetAuctionsAsync(null!)).ReturnsAsync(auctions);

            // Act
            var result = await _auctionController.GetAllAuctions(null!);
        
            // Assert
            Assert.Equal(10, result.Value!.Count);
            Assert.IsType<ActionResult<List<AuctionDto>>>(result);
        }
    }
}