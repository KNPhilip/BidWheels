namespace AuctionService.Dtos
{
    public class CreateAuctionDto
    {
        public required string? Make { get; set; }
        public required string? Model { get; set; }
        public required int Year { get; set; }
        public required string? Color { get; set; }
        public required int Mileage { get; set; }
        public required string ImageUrl { get; set; } = string.Empty;
        public required int ReservePrice { get; set; }
        public required DateTime AuctionEnd { get; set; }
    }
}