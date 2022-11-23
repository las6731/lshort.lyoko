using System.Reflection;
using System.Text.Json;
using LShort.Lyoko.Messaging.Abstractions;
using LShort.Lyoko.Messaging.Abstractions.Services;
using LShort.Lyoko.Messaging.RabbitMQ.Attributes;
using LShort.Lyoko.Messaging.RabbitMQ.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Serilog;

namespace LShort.Lyoko.Messaging.RabbitMQ;

public class MessagingService : IMessagingService, IDisposable
{
    private readonly ILogger logger;
    private readonly IRabbitMQConfiguration configuration;
    private readonly IModel channel;
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessagingService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="configuration">The RabbitMQ configuration.</param>
    /// <param name="connectionProvider">The connection provider.</param>
    /// <param name="serviceProvider">The service provider.</param>
    public MessagingService(
        ILogger logger,
        IRabbitMQConfiguration configuration,
        RabbitMQConnectionProvider connectionProvider,
        IServiceProvider serviceProvider)
    {
        this.logger = logger.ForContext<MessagingService>();
        this.configuration = configuration;
        this.serviceProvider = serviceProvider;

        var connection = connectionProvider.Connect();
        this.channel = connection.CreateModel();
        this.channel.ConfirmSelect();
        this.logger.Information("RabbitMQ connection established.");
    }

    public void EnsureTopology()
    {
        // get all queue bindings in current domain
        var bindings = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IMessageSubscriber))))
            .Select(t => t.GetCustomAttribute<QueueBinding>())
            .Where(b => b is not null)
            .OfType<QueueBinding>()
            .ToList();

        this.logger.Information("Ensuring RabbitMQ topology.");

        // ensure default exchange
        if (!string.IsNullOrEmpty(this.configuration.DefaultExchange)) this.channel.ExchangeDeclare(this.configuration.DefaultExchange, ExchangeType.Direct, true);

        foreach (var binding in bindings)
        {
            this.channel.ExchangeDeclare(binding.ExchangeName, binding.ExchangeType, true);
            this.DeclareAndBindQueue(binding);
            this.logger.Information("RabbitMQ queue bound: {queueName}", binding.QueueName);
        }

        this.logger.Information("RabbitMQ topology ensured.");
    }

    private void DeclareAndBindQueue(QueueBinding binding)
    {
        var args = new Dictionary<string, object>();
        if (this.configuration.RetryAttempts > 0) args.Add("x-dead-letter-exchange", this.DeclareRetryQueue(binding));

        this.channel.QueueDeclare(binding.QueueName, true, false, false, args);
        this.channel.QueueBind(binding.QueueName, binding.ExchangeName, binding.EventName);
    }

    private string DeclareRetryQueue(QueueBinding binding)
    {
        var args = new Dictionary<string, object>
        {
            { "x-dead-letter-exchange", binding.ExchangeName },
            { "x-message-ttl", this.configuration.RetryDelay * 1000 },
        };
        var retryExchange = $"{binding.ExchangeName}-retry";
        var retryQueue = $"{binding.QueueName}-retry";

        this.channel.ExchangeDeclare(retryExchange, binding.ExchangeType, true);
        this.channel.QueueDeclare(retryQueue, true, false, false, args);
        this.channel.QueueBind(retryQueue, retryExchange, binding.EventName);

        return retryExchange;
    }

    public async Task StartConsumers()
    {
        var subscriberMap = this.serviceProvider
            .GetServices<IMessageSubscriber>()
            .Select(s => s.GetType())
            .Select(t => new { Type = t, Binding = t.GetCustomAttribute<QueueBinding>() })
            .Where(t => t.Binding is not null)
            .GroupBy(t => t.Binding!.QueueName)
            .ToDictionary(g => g.Key, g => g.Select(t => t.Type));

        foreach (var queue in subscriberMap.Keys)
        {
            this.logger.Information("Starting message consumption on queue: {queue}", queue);
            for (int i = 0; i < this.configuration.ThreadsPerQueue; i++)
            {
                var consumer = new AsyncMessageConsumer(this.channel, this.serviceProvider, subscriberMap[queue]);
                await Task.Run(() => this.channel.BasicConsume(queue, false, consumer));
            }
        }
    }

    /// <inheritdoc />
    public Task<bool> Publish(EventMessage e)
    {
        return this.Publish(this.configuration.DefaultExchange, e);
    }

    /// <inheritdoc />
    public Task<bool> Publish(string exchange, EventMessage e)
    {
        var body = JsonSerializer.SerializeToUtf8Bytes(e);
        var props = this.channel.CreateBasicProperties();
        props.ContentType = "application/json";
        props.CorrelationId = e.CorrelationId.ToString();

        this.channel.BasicPublish(exchange, e.Name, true, props, body);
        return Task.Run(() => this.channel.WaitForConfirms());
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.channel.Close();
        this.logger.Information("RabbitMQ channel closed.");
    }
}