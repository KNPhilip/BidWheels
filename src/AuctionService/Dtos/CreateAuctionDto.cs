namespace AuctionService.Dtos
{
    public class CreateAuctionDto
    {
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int Year { get; set; }
        public string? Color { get; set; }
        public int Mileage { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int ReservePrice { get; set; }
        public DateTime AuctionEnd { get; set; }
    }
}