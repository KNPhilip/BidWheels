using AuctionService.Consumers;
using AuctionService.Data;
using AuctionService.Repositories;
using AuctionService.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddScoped<IAuctionRepository, AuctionRepository>();
builder.Services.AddGrpc();

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

    // Mass Transit will automatically recognize any other consumers within the same namespace.
    config.AddConsumersFromNamespaceContaining<AuctionCreatedFaultConsumer>();

    config.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction", false));

    config.UsingRabbitMq((context, cfg) => 
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"], "/", host => 
        {
            host.Username(builder.Configuration.GetValue("RabbitMQ:Username", "guest"));
            host.Password(builder.Configuration.GetValue("RabbitMQ:Password", "guest"));
        });

        cfg.ConfigureEndpoints(context);
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => 
    {
        options.Authority = builder.Configuration["IdentityServiceUrl"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.NameClaimType = "username";
    });

WebApplication app = builder.Build();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapGrpcService<GrpcAuctionService>();

try 
{
    DbInitializer.InitDb(app);
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}

app.Run();

/// <summary>
/// Represents the entry point for the AuctionService program.
/// </summary>
public sealed partial class Program {}
