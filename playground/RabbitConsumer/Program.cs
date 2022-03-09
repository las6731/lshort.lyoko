using LShort.Lyoko.Common.Extensions;
using LShort.Lyoko.Messaging.RabbitMQ.Extensions;
using RabbitConsumer.Subscribers;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// add logging
builder.UseSerilog();

// Add services to the container.
builder.Services
    .UseRabbitMQ(builder.Configuration.GetSection("RabbitMQ"))
    .AddSubscriber<TestMessageSubscriber>()
    .AddControllers();

var app = builder.Build();

app.UseSerilogRequestLogging()
    .UseHttpsRedirection()
    .UseRouting()
    .UseCors(x => x
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin())
    .UseEndpoints(endpoints => endpoints.MapControllers());

app.Services.InitializeRabbitMQ();

app.Run();