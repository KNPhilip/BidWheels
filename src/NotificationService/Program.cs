global using MassTransit;
global using Microsoft.AspNetCore.SignalR;
global using NotificationService.Hubs;
global using Contracts;
global using NotificationService.Consumers;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddMassTransit(config => 
{
    config.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();

    config.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("nt", false));

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

WebApplication app = builder.Build();

app.MapHub<NotificationHub>("/notifications");

app.Run();
