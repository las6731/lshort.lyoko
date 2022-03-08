using LShort.Lyoko.Common.Extensions;
using LShort.Lyoko.Messaging.Abstractions;
using LShort.Lyoko.Messaging.RabbitMQ.Configuration;
using LShort.Lyoko.Messaging.RabbitMQ.Configuration.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LShort.Lyoko.Messaging.RabbitMQ.Extensions;

/// <summary>
/// Startup extensions for using RabbitMQ in a .NET application.
/// </summary>
public static class StartupExtensions
{
    /// <summary>
    /// Add RabbitMQ dependencies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The config section that maps to a <see cref="IRabbitMQConfiguration"/>.</param>
    /// <returns>The service collection with RabbitMQ dependencies.</returns>
    public static IServiceCollection UseRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddSingleton<IRabbitMQConfiguration>(configuration.BindAs<RabbitMQConfiguration>())
            .AddSingleton<RabbitMQConnectionProvider>()
            .AddSingleton<IMessagingService, MessagingService>();
    }

    /// <summary>
    /// Register a message subscriber.
    /// </summary>
    /// <remarks>
    /// Subscribers must be resolvable as both <see cref="IMessageSubscriber"/> and <see cref="TSubscriber"/> to be used
    /// successfully in my implementation. There's probably way better ways to handle this, but I'm dumb so this is how
    /// I'm doing it.
    /// </remarks>
    /// <param name="services">The service collection.</param>
    /// <typeparam name="TSubscriber">The message subscriber to register.</typeparam>
    /// <returns>The service collection with the message subscriber registered.</returns>
    public static IServiceCollection AddSubscriber<TSubscriber>(this IServiceCollection services)
        where TSubscriber : class, IMessageSubscriber
    {
        return services.AddTransient<IMessageSubscriber, TSubscriber>()
            .AddTransient<TSubscriber>();
    }

    /// <summary>
    /// Initializes RabbitMQ messaging.
    /// </summary>
    /// <param name="services">The service provider.</param>
    /// <returns>The service provider.</returns>
    public static IServiceProvider InitializeRabbitMQ(this IServiceProvider services)
    {
        MessagingService service = (MessagingService) services.GetRequiredService<IMessagingService>();

        service.EnsureTopology();
        Task.Run(() => service.StartConsumers());

        return services;
    }
}