namespace AuctionService.IntegrationTests.Utils
{
    public sealed class AuthHelper
    {
        public static Dictionary<string, object> GetBearerForUser(string username) =>
            new()
            {
                { ClaimTypes.Name, username }
            };
    }
}
