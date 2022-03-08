namespace LShort.Lyoko.Messaging.RabbitMQ.Configuration.Implementation;

/// <inheritdoc />
public class RabbitMQConfiguration : IRabbitMQConfiguration
{
    /// <inheritdoc />
    public Uri ConnectionString { get; set; }

    /// <inheritdoc />
    public string DefaultExchange { get; set; }

    /// <inheritdoc />
    public int ThreadsPerQueue { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMQConfiguration"/> class.
    /// </summary>
    public RabbitMQConfiguration()
    {
        ConnectionString = new Uri("amqp://localhost");
        DefaultExchange = string.Empty;
        ThreadsPerQueue = 1;
    }
}