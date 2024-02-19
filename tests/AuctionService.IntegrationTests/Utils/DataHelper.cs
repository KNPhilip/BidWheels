namespace AuctionService.IntegrationTests.Utils
{
    public static class DataHelper
    {
        public static void InitDbForTests(AuctionContext db)
        {
            db.Auctions.AddRange(GetAuctionsForTest());
            db.SaveChanges();
        }

        public static void ReinitDbForTests(AuctionContext db)
        {
            db.Auctions.RemoveRange(db.Auctions);
            db.SaveChanges();
            InitDbForTests(db);
        }

        private static List<Auction> GetAuctionsForTest() =>
            DbInitializer.GetDbAuctions();
    }
}
