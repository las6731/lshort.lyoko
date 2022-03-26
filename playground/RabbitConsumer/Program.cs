using LShort.Lyoko.Common.Extensions;
using LShort.Lyoko.Messaging.RabbitMQ.Extensions;
using RabbitConsumer.Subscribers;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// add services
builder.UseSerilog()
    .UseRabbitMQ()
    .Services
        .AddSubscriber<TestMessageSubscriber>()
        .AddControllers();

var app = builder.Build();

// configure services
app.UseSerilogRequestLogging()
    .UseBasicRouting()
    .InitializeRabbitMQ();

app.Run();