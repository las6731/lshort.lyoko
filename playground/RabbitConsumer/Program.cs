using LShort.Lyoko.Common.Extensions;
using LShort.Lyoko.DataAccess.MongoDb.Extensions;
using LShort.Lyoko.Messaging.RabbitMQ.Extensions;
using RabbitConsumer;
using RabbitConsumer.DataAccess;
using RabbitConsumer.Subscribers;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// add services
builder.UseSerilog()
    .UseRabbitMQ()
    .UseMongoDb()
    .Services
        .AddSubscriber<TestMessageSubscriber>()
        .AddRepository<EventRepository, StoredEventMessage>()
        .AddHostedService<RabbitPublisher>()
        .AddControllers();

var app = builder.Build();

// configure services
app.UseSerilogRequestLogging()
    .UseBasicRouting()
    .InitializeRabbitMQ();

app.Run();