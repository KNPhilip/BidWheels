using AuctionService.Controllers;
using AuctionService.Repositories;
using AuctionService.RequestHelpers;
using AutoFixture;
using AutoMapper;
using MassTransit;
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
        public void TestName()
        {
            // Arrange
        
            // Act
        
            // Assert
        }
    }
}