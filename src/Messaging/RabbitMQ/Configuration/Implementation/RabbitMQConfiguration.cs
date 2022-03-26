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

    /// <inheritdoc />
    public int RetryAttempts { get; set; }

    /// <inheritdoc />
    public int RetryDelay { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMQConfiguration"/> class.
    /// </summary>
    public RabbitMQConfiguration()
    {
        this.ConnectionString = new Uri("amqp://localhost");
        this.DefaultExchange = string.Empty;
        this.ThreadsPerQueue = 1;
        this.RetryAttempts = 3;
        this.RetryDelay = 10;
    }
}