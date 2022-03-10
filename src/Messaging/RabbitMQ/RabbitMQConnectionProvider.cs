using LShort.Lyoko.Messaging.RabbitMQ.Configuration;
using RabbitMQ.Client;

namespace LShort.Lyoko.Messaging.RabbitMQ;

/// <summary>
/// Provides a connection to RabbitMQ.
/// </summary>
public class RabbitMQConnectionProvider : IDisposable
{
    private readonly ConnectionFactory factory;
    private IConnection? connection;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMQConnectionProvider"/> class.
    /// </summary>
    /// <param name="configuration">The rabbit configuration.</param>
    public RabbitMQConnectionProvider(IRabbitMQConfiguration configuration)
    {
        this.factory = new ConnectionFactory();
        this.factory.Uri = configuration.ConnectionString;

        this.factory.DispatchConsumersAsync = true;
        this.factory.AutomaticRecoveryEnabled = true;
    }

    /// <summary>
    /// Retrieve a connection to RabbitMQ.
    /// </summary>
    /// <returns>The RabbitMQ connection.</returns>
    public IConnection Connect()
    {
        if (this.connection is not null) return this.connection;

        this.connection = this.factory.CreateConnection();
        return this.connection;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.connection?.Close();
    }
}