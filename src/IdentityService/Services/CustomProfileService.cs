using System.Security.Claims;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Services
{
    public class CustomProfileService(UserManager<ApplicationUser> userManager) : IProfileService
    {
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            ApplicationUser user = await userManager.GetUserAsync(context.Subject);
            IList<Claim> existingClaims = await userManager.GetClaimsAsync(user);

            List<Claim> claims =
            [
                new("username", user.UserName)
            ];

            context.IssuedClaims.AddRange(claims);
            context.IssuedClaims.Add(existingClaims
                .FirstOrDefault(x => x.Type == JwtClaimTypes.Name));
        }

        public Task IsActiveAsync(IsActiveContext context) =>
            Task.CompletedTask;
    }
}