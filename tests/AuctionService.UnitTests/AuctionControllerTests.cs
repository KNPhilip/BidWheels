using AuctionService.Controllers;
using AuctionService.Dtos;
using AuctionService.Entities;
using AuctionService.Repositories;
using AuctionService.RequestHelpers;
using AuctionService.UnitTests.Utils;
using AutoFixture;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Http;
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
            _auctionController = new AuctionsController(_mapper, _publishEndpoint.Object, _auctionRepository.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = AuthHelper.GetClaimsPrincipal()
                    }
                }
            };
        }

        [Fact]
        public async Task GetAuctions_WithNoParams_Returns10Auctions()
        {
            // Arrange
            List<AuctionDto> auctions = _fixture.CreateMany<AuctionDto>(10).ToList();
            _auctionRepository.Setup(repo => repo.GetAuctionsAsync(null!)).ReturnsAsync(auctions);

            // Act
            dynamic result = await _auctionController.GetAllAuctions(null!);
        
            // Assert
            Assert.Equal(10, result.Result.Value!.Count);
            Assert.IsType<ActionResult<List<AuctionDto>>>(result);
        }

        [Fact]
        public async Task GetAuction_WithValidGuid_ReturnsAuction()
        {
            // Arrange
            AuctionDto auction = _fixture.Create<AuctionDto>();
            _auctionRepository.Setup(repo => repo.GetAuctionAsync(It.IsAny<Guid>())).ReturnsAsync(auction);

            // Act
            dynamic result = await _auctionController.GetAuction(auction.Id);

            // Assert
            Assert.Equal(auction.Make, result.Result.Value.Make);
            Assert.IsType<ActionResult<AuctionDto>>(result);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetAuction_WithInvalidGuid_Returns404NotFound()
        {
            // Arrange
            _auctionRepository.Setup(repo => repo.GetAuctionAsync(It.IsAny<Guid>()))
                .ReturnsAsync(value: null);

            // Act
            dynamic result = await _auctionController.GetAuction(Guid.NewGuid());
        
            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateAuction_WithValidCreateAuctionDto_Returns201CreatedAtAction()
        {
            // Arrange
            CreateAuctionDto auction = _fixture.Create<CreateAuctionDto>(); 
            _auctionRepository.Setup(repo => repo.AddAuction(It.IsAny<Auction>()));
            _auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            dynamic result = await _auctionController.CreateAuction(auction);
        
            // Assert
            Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetAuction", result.Result.ActionName);
            Assert.IsType<AuctionDto>(result.Result.Value);
        }

        [Fact]
        public async Task CreateAuction_FailedSave_Returns400BadRequest()
        {
            // Arrange
            CreateAuctionDto auction = _fixture.Create<CreateAuctionDto>();
            _auctionRepository.Setup(repo => repo.AddAuction(It.IsAny<Auction>()));
            _auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(false);

            // Act
            dynamic result = await _auctionController.CreateAuction(auction);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task UpdateAuction_WithUpdateAuctionDto_Returns200Ok()
        {
            // Arrange
            Auction auction = _fixture.Build<Auction>().Without(a => a.Item).Create();
            auction.Item = _fixture.Build<Item>().Without(a => a.Auction).Create();
            auction.Seller = _auctionController.User.Identity!.Name!;
            UpdateAuctionDto auctionDto = _fixture.Create<UpdateAuctionDto>();
            _auctionRepository.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>()))
                .ReturnsAsync(auction);
            _auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            dynamic result = await _auctionController.UpdateAuction(auction.Id, auctionDto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task UpdateAuction_WithInvalidUser_Returns403Forbidden()
        {
            // Arrange
            Auction auction = _fixture.Build<Auction>().Without(a => a.Item).Create();
            UpdateAuctionDto auctionDto = _fixture.Create<UpdateAuctionDto>();
            _auctionRepository.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>()))
                .ReturnsAsync(auction);

            // Act
            dynamic result = await _auctionController.UpdateAuction(auction.Id, auctionDto);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task UpdateAuction_WithInvalidGuid_Returns404NotFound()
        {
            // Arrange
            UpdateAuctionDto auctionDto = _fixture.Create<UpdateAuctionDto>();
            _auctionRepository.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>()))
                .ReturnsAsync(value: null);

            // Act
            dynamic result = await _auctionController.UpdateAuction(Guid.NewGuid(), auctionDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteAuction_WithValidUser_Returns200Ok()
        {
            // Arrange
            Auction auction = _fixture.Build<Auction>().Without(a => a.Item).Create();
            auction.Seller = _auctionController.User.Identity!.Name!;
            _auctionRepository.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>()))
                .ReturnsAsync(auction);
            _auctionRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            dynamic result = await _auctionController.DeleteAuction(auction.Id);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task DeleteAuction_WithInvalidGuid_Returns404NotFound()
        {
            // Arrange
            _auctionRepository.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>()))
                .ReturnsAsync(value: null);

            // Act
            dynamic result = await _auctionController.DeleteAuction(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteAuction_WithInvalidUser_Returns403Forbidden()
        {
            // Arrange
            Auction auction = _fixture.Build<Auction>().Without(a => a.Item).Create();
            _auctionRepository.Setup(repo => repo.GetAuctionEntityById(It.IsAny<Guid>()))
                .ReturnsAsync(auction);

            // Act
            dynamic result = await _auctionController.DeleteAuction(auction.Id);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }
    }
}