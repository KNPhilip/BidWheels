using AuctionService.Entities;

namespace AuctionService.UnitTests;

public class AuctionEntityTests
{
    [Fact]
    public void HasReservePrice_ReservePriceGtZero_True()
    {
        // Arrange
        Auction auction = new()
        {
            Id = Guid.NewGuid(),
            ReservePrice = 10
        };

        // Act
        bool result = auction.HasReservePrice();

        // Assert
        Assert.True(result);
    }
}