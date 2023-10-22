using AuctionService.Data;
using MassTransit;
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
builder.Services.AddMassTransit(config => 
{
    config.AddEntityFrameworkOutbox<AuctionContext>(options => 
    {
       options.QueryDelay = TimeSpan.FromSeconds(10); 
       options.UsePostgres();
       options.UseBusOutbox();
    });

    config.UsingRabbitMq((context, cfg) => 
    {
        cfg.ConfigureEndpoints(context);
    });
});

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
