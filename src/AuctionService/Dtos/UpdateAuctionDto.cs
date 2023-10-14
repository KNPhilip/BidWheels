namespace AuctionService.Dtos
{
    public class UpdateAuctionDto
    {
        public required string? Make { get; set; }
        public required string? Model { get; set; }
        public required int Year { get; set; }
        public required string? Color { get; set; }
        public required int Mileage { get; set; }
    }
}