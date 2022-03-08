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
        services.AddSingleton<IRabbitMQConfiguration>(configuration.BindAs<RabbitMQConfiguration>())
            .AddSingleton<RabbitMQConnectionProvider>()
            .AddSingleton<IMessagingService, MessagingService>();

        return services;
    }

    /// <summary>
    /// Initializes RabbitMQ messaging.
    /// </summary>
    /// <param name="services">The service provider.</param>
    /// <returns>The service provider.</returns>
    public static IServiceProvider InitializeRabbitMQ(this IServiceProvider services)
    {
        var service = services.GetRequiredService<MessagingService>();

        service.EnsureTopology();
        Task.Run(() => service.StartConsumers());

        return services;
    }
}