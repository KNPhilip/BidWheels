using System.Security.Claims;
using IdentityModel;
using IdentityService.Data;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IdentityService;

public class SeedData
{
    public static void EnsureSeedData(WebApplication app)
    {
        using IServiceScope scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        ApplicationDbContext context = scope.ServiceProvider.GetService<ApplicationDbContext>();
        context.Database.Migrate();

        UserManager<ApplicationUser> userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (userMgr.Users.Any()) return;

        ApplicationUser alice = userMgr.FindByNameAsync("alice").Result;
        if (alice == null)
        {
            alice = new ApplicationUser
            {
                UserName = "alice",
                Email = "AliceSmith@email.com",
                EmailConfirmed = true,
            };
            IdentityResult result = userMgr.CreateAsync(alice, "Pass123$").Result;
            if (!result.Succeeded)
                throw new Exception(result.Errors.First().Description);

            result = userMgr.AddClaimsAsync(alice, new Claim[]{
                new(JwtClaimTypes.Name, "Alice Smith"),
            }).Result;
            if (!result.Succeeded)
                throw new Exception(result.Errors.First().Description);
            Log.Debug("alice created");
        }
        else
            Log.Debug("alice already exists");

        ApplicationUser bob = userMgr.FindByNameAsync("bob").Result;
        if (bob == null)
        {
            bob = new ApplicationUser
            {
                UserName = "bob",
                Email = "BobSmith@email.com",
                EmailConfirmed = true
            };
            IdentityResult result = userMgr.CreateAsync(bob, "Pass123$").Result;
            if (!result.Succeeded)
                throw new Exception(result.Errors.First().Description);

            result = userMgr.AddClaimsAsync(bob, new Claim[]{
                new(JwtClaimTypes.Name, "Bob Smith")
            }).Result;
            if (!result.Succeeded)
                throw new Exception(result.Errors.First().Description);
            Log.Debug("bob created");
        }
        else
            Log.Debug("bob already exists");
    }
}