using System.Security.Claims;

namespace AuctionService.UnitTests.Utils
{
    public class AuthHelper
    {
        public static ClaimsPrincipal GetClaimsPrincipal()
        {
            List<Claim> claims =
            [
                new Claim(ClaimTypes.Name, "Test User")
            ];

            ClaimsIdentity identity = new(claims, "Testing");
            ClaimsPrincipal claimsPrincipal = new(identity);

            return claimsPrincipal;
        }
    }
}