WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
