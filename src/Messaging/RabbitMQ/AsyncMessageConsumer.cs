using System.Reflection;
using System.Text;
using System.Text.Json;
using LShort.Lyoko.Messaging.Abstractions;
using LShort.Lyoko.Messaging.RabbitMQ.Attributes;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LShort.Lyoko.Messaging.RabbitMQ;

public class AsyncMessageConsumer : AsyncEventingBasicConsumer
{
    private readonly IServiceProvider serviceProvider;
    private readonly IEnumerable<Type> subscriberTypes;

    public AsyncMessageConsumer(IModel model, IServiceProvider serviceProvider, IEnumerable<Type> subscriberTypes)
        : base(model)
    {
        this.serviceProvider = serviceProvider;
        this.subscriberTypes = subscriberTypes;

        Received += (_, msg) => HandleMessage(msg);
    }

    private async Task HandleMessage(BasicDeliverEventArgs message)
    {
        Console.WriteLine(message.ConsumerTag);
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