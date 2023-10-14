using AuctionService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<AuctionContext>(options => 
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});
// AppDomain provides the assemblies where the application is running in, from there AutoMapper
// detects that the MappingProfiles class inherits from the Profile class and therefore uses it.
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

try 
{
    DbInitializer.InitDb(app);
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}

app.Run();
