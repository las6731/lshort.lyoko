using LShort.Lyoko.Messaging.RabbitMQ.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// add logging
builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

// Add services to the container.
builder.Services
    .UseRabbitMQ(builder.Configuration.GetSection("RabbitMQ"))
    .AddControllers();

var app = builder.Build();

app.UseHttpsRedirection()
    .UseRouting()
    .UseAuthorization();

app.Services.InitializeRabbitMQ();

app.Run();
