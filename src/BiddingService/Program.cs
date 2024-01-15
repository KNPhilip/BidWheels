using MongoDB.Driver;
using MongoDB.Entities;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using BiddingService.Consumers;
using BiddingService.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddMassTransit(config => 
{
    config.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();

    config.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("bids", false));

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

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHostedService<CheckAuctionFinished>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

await DB.InitAsync("BidDb", MongoClientSettings
    .FromConnectionString(builder.Configuration.GetConnectionString("BidDbConnection")));

app.Run();
