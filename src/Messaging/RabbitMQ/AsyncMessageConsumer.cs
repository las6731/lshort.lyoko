using System.Reflection;
using System.Text.Json;
using LShort.Lyoko.Messaging.Abstractions;
using LShort.Lyoko.Messaging.RabbitMQ.Attributes;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LShort.Lyoko.Messaging.RabbitMQ;

/// <summary>
/// Basic asynchronous message consumer to delegate consumption to a <see cref="IMessageSubscriber"/>.
/// </summary>
public class AsyncMessageConsumer : AsyncEventingBasicConsumer
{
    private readonly IServiceProvider serviceProvider;
    private readonly IEnumerable<Type> subscriberTypes;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncMessageConsumer"/> class.
    /// </summary>
    /// <param name="model">The RabbitMQ model.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="subscriberTypes">
    /// The types of <see cref="IMessageSubscriber"/> implementations that this consumer is responsible for.
    /// </param>
    public AsyncMessageConsumer(IModel model, IServiceProvider serviceProvider, IEnumerable<Type> subscriberTypes)
        : base(model)
    {
        this.serviceProvider = serviceProvider;
        this.subscriberTypes = subscriberTypes;

        Received += (_, msg) => HandleMessage(msg);
    }

    private async Task HandleMessage(BasicDeliverEventArgs message)
    {
        var e = JsonSerializer.Deserialize<EventMessage>(message.Body.Span);
        if (e is null) Model.BasicReject(message.DeliveryTag, false); // message is not a valid event

        var type = subscriberTypes.FirstOrDefault(t => t.GetCustomAttribute<QueueBinding>()?.EventName == e.Name);
        if (type is null) Model.BasicNack(message.DeliveryTag, false, true); // app does not contain subscriber for this event

        var subscriber = (IMessageSubscriber) serviceProvider.GetRequiredService(type!);
        var result = await subscriber.ConsumeEvent(e!);

        if (result) Model.BasicAck(message.DeliveryTag, false);
        else Model.BasicNack(message.DeliveryTag, false, true);
    }
}