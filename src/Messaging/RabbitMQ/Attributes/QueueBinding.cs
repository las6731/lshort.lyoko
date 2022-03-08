namespace LShort.Lyoko.Messaging.RabbitMQ.Attributes;

public class QueueBinding : Attribute
{
    public string EventName { get; init; }

    public string QueueName { get; init; }

    public string ExchangeName { get; init; }

    public string ExchangeType { get; init; }

    public QueueBinding(string eventName, string queueName, string exchangeName, string exchangeType = global::RabbitMQ.Client.ExchangeType.Direct)
    {
        EventName = eventName;
        QueueName = queueName;
        ExchangeName = exchangeName;
        ExchangeType = exchangeType;
    }
}