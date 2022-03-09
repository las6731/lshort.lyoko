using LShort.Lyoko.Messaging.RabbitMQ.Configuration;
using RabbitMQ.Client;

namespace LShort.Lyoko.Messaging.RabbitMQ;

/// <summary>
/// Provides a connection to RabbitMQ.
/// </summary>
public class RabbitMQConnectionProvider : IDisposable
{
    private readonly ConnectionFactory factory;
    private IConnection connection;

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMQConnectionProvider"/> class.
    /// </summary>
    /// <param name="configuration">The rabbit configuration.</param>
    public RabbitMQConnectionProvider(IRabbitMQConfiguration configuration)
    {
        factory = new ConnectionFactory();
        factory.Uri = configuration.ConnectionString;

        factory.DispatchConsumersAsync = true;
        factory.AutomaticRecoveryEnabled = true;
    }

    /// <summary>
    /// Retrieve a connection to RabbitMQ.
    /// </summary>
    /// <returns>The RabbitMQ connection.</returns>
    public IConnection Connect()
    {
        if (connection is not null) return connection;

        connection = factory.CreateConnection();
        return connection;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        connection.Close();
    }
}