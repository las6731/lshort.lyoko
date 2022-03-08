using LShort.Lyoko.Messaging.RabbitMQ.Configuration;
using RabbitMQ.Client;

namespace LShort.Lyoko.Messaging.RabbitMQ;

public class RabbitMQConnectionProvider : IDisposable
{
    private readonly ConnectionFactory factory;
    private IConnection connection;

    public RabbitMQConnectionProvider(IRabbitMQConfiguration configuration)
    {
        factory = new ConnectionFactory();
        factory.Uri = configuration.ConnectionString;

        factory.DispatchConsumersAsync = true;
        factory.AutomaticRecoveryEnabled = true;
    }

    public IConnection Connect()
    {
        if (connection is not null) return connection;

        connection = factory.CreateConnection();
        return connection;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        connection.Close();
    }
}