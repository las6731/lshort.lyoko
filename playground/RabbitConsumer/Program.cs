using LShort.Lyoko.Messaging.Abstractions;
using LShort.Lyoko.Messaging.RabbitMQ.Extensions;
using RabbitConsumer.Subscribers;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// add logging
builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

// Add services to the container.
builder.Services
    .UseRabbitMQ(builder.Configuration.GetSection("RabbitMQ"))
    .AddTransient<TestMessageSubscriber>()
    .AddTransient<IMessageSubscriber, TestMessageSubscriber>()
    .AddControllers();

var app = builder.Build();

app.UseHttpsRedirection()
    .UseRouting()
    .UseCors(x => x
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin())
    .UseEndpoints(endpoints => endpoints.MapControllers());

app.Services.InitializeRabbitMQ();

app.Run();