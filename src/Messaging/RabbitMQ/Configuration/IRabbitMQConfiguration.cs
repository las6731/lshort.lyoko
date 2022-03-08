namespace LShort.Lyoko.Messaging.RabbitMQ.Configuration;

/// <summary>
/// Configuration for RabbitMQ services.
/// </summary>
public interface IRabbitMQConfiguration
{
    /// <summary>
    /// The uri to connect to RabbitMQ.
    /// </summary>
    public Uri ConnectionString { get; set; }

    /// <summary>
    /// The exchange to publish messages to by default.
    /// </summary>
    public string DefaultExchange { get; set; }

    /// <summary>
    /// How many threads should be created per queue for consuming messages.
    /// </summary>
    public int ThreadsPerQueue { get; set; }
}