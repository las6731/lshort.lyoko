using LShort.Lyoko.Messaging.Abstractions;
using LShort.Lyoko.Messaging.RabbitMQ.Attributes;
using ILogger = Serilog.ILogger;

namespace RabbitConsumer.Subscribers;

[QueueBinding("playground.test.event", "playground.test-queue", "playground.direct")]
public class TestMessageSubscriber : IMessageSubscriber
{
    private readonly ILogger logger;

    public TestMessageSubscriber(ILogger logger)
    {
        this.logger = logger.ForContext<TestMessageSubscriber>();
    }

    public Task<bool> ConsumeEvent(EventMessage e)
    {
        logger.Information("Consumed event: {e}", e.Name);
        var payload = e.GetPayload<string>();

        this.logger.ForContext("correlationId", e.CorrelationId)
            .Information("Test event received: {payload}", payload);

        return Task.FromResult(true);
    }
}