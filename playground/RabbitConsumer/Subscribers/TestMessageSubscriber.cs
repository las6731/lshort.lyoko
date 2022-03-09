using LShort.Lyoko.Messaging.Abstractions;
using LShort.Lyoko.Messaging.RabbitMQ.Attributes;
using ILogger = Serilog.ILogger;

namespace RabbitConsumer.Subscribers;

/// <summary>
/// Example of a basic message subscriber implementation.
/// </summary>
[QueueBinding("playground.test.event", "playground.test-queue", "playground.direct")]
public class TestMessageSubscriber : IMessageSubscriber
{
    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestMessageSubscriber"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public TestMessageSubscriber(ILogger logger)
    {
        this.logger = logger.ForContext<TestMessageSubscriber>();
    }

    /// <inheritdoc />
    public Task<bool> ConsumeEvent(EventMessage e)
    {
        var payload = e.GetPayload<string>();

        this.logger.ForContext("correlationId", e.CorrelationId)
            .Information("Test event received: {payload}", payload);

        return Task.FromResult(true);
    }
}