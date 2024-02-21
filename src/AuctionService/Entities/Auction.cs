namespace AuctionService.Entities
{
    public sealed class Auction
    {
        public Guid Id { get; set; }
        public int ReservePrice { get; set; }
        public string Seller { get; set; } = string.Empty;
        public string Winner { get; set; } = string.Empty;
        public int? SoldAmount { get; set; }
        public int? CurrentHighBid { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime AuctionEnd { get; set; }
        public Status Status { get; set; }
        public Item? Item { get; set; }

        public bool HasReservePrice() => ReservePrice > 0;
    }
}
