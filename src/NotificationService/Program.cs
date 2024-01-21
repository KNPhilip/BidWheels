global using MassTransit;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();

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

app.Run();