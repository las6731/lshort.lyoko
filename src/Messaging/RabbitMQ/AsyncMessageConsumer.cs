using System.Reflection;
using System.Text.Json;
using LShort.Lyoko.Messaging.Abstractions;
using LShort.Lyoko.Messaging.Abstractions.Services;
using LShort.Lyoko.Messaging.RabbitMQ.Attributes;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace LShort.Lyoko.Messaging.RabbitMQ;

/// <summary>
/// Basic asynchronous message consumer to delegate consumption to a <see cref="IMessageSubscriber"/>.
/// </summary>
public class AsyncMessageConsumer : AsyncEventingBasicConsumer
{
    private readonly IServiceProvider serviceProvider;
    private readonly IEnumerable<Type> subscriberTypes;
    private readonly IFailedConsumptionSupervisor? failedConsumptionSupervisor;
    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncMessageConsumer"/> class.
    /// </summary>
    /// <param name="model">The RabbitMQ model.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="subscriberTypes">
    /// The types of <see cref="IMessageSubscriber"/> implementations that this consumer is responsible for.
    /// </param>
    public AsyncMessageConsumer(
        IModel model,
        IServiceProvider serviceProvider,
        IEnumerable<Type> subscriberTypes)
        : base(model)
    {
        this.serviceProvider = serviceProvider;
        this.subscriberTypes = subscriberTypes;
        this.logger = serviceProvider
            .GetRequiredService<ILogger>()
            .ForContext<AsyncMessageConsumer>();
        this.failedConsumptionSupervisor = serviceProvider.GetService<IFailedConsumptionSupervisor>();

        this.Received += (_, msg) => this.HandleMessage(msg);
    }

    private async Task HandleMessage(BasicDeliverEventArgs message)
    {
        var e = JsonSerializer.Deserialize<EventMessage>(message.Body.Span);
        if (e is null)
        {
            // message is not a valid event. Acknowledge just to get it off the queue
            this.logger
                .ForContext("routingKey", message.RoutingKey)
                .Warning("Ignoring malformed message: {body}", message.Body.ToString());
            this.Model.BasicAck(message.DeliveryTag, false);
            return;
        }

        var type = this.subscriberTypes.FirstOrDefault(t => t.GetCustomAttribute<QueueBinding>()?.EventName == e.Name);
        if (type is null)
        {
            // app does not contain subscriber for this event
            this.Model.BasicNack(message.DeliveryTag, false, true);
            return;
        }

        var subscriber = (IMessageSubscriber) this.serviceProvider.GetRequiredService(type!);
        var result = await subscriber.ConsumeEvent(e!);

        if (!result)
        {
            if (this.failedConsumptionSupervisor is not null)
            {
                var shouldRetry = await this.failedConsumptionSupervisor.HasExceededRetryCount(e);
                if (!shouldRetry)
                {
                    // TODO: store failed message in database
                    this.logger
                        .ForContext("correlationId", e.CorrelationId)
                        .ForContext("eventId", e.Id)
                        .Error("Failed event exceeded retry limit: {name}", e.Name);
                    this.Model.BasicAck(message.DeliveryTag, false);
                    return;
                }
            }

            this.Model.BasicNack(message.DeliveryTag, false, false);
            return;
        }

        this.Model.BasicAck(message.DeliveryTag, false);
    }
}