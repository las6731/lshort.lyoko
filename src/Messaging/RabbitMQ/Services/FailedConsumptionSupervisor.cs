using LShort.Lyoko.Messaging.Abstractions;
using LShort.Lyoko.Messaging.Abstractions.Services;
using LShort.Lyoko.Messaging.RabbitMQ.Configuration;
using Microsoft.Extensions.Caching.Distributed;

namespace LShort.Lyoko.Messaging.RabbitMQ;

public class FailedConsumptionSupervisor : IFailedConsumptionSupervisor
{
    private readonly IRabbitMQConfiguration configuration;

    private readonly IDistributedCache cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="FailedConsumptionSupervisor"/> class.
    /// </summary>
    /// <param name="configuration">The RabbitMQ configuration.</param>
    /// <param name="cache">The cache.</param>
    public FailedConsumptionSupervisor(IRabbitMQConfiguration configuration, IDistributedCache cache)
    {
        this.configuration = configuration;
        this.cache = cache;
    }

    /// <inheritdoc />
    public async Task<bool> HasExceededRetryCount(EventMessage message)
    {
        var bytes = await this.cache.GetAsync(message.Id.ToString());
        var retries = TryGetInt(bytes);

        var shouldRetry = ++retries > this.configuration.RetryAttempts;
        if (shouldRetry)
        {
            var options = new DistributedCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromMinutes(5),
            };
            await this.cache.SetAsync(message.Id.ToString(), BitConverter.GetBytes(retries), options);
        }

        return shouldRetry;
    }

    private static int TryGetInt(byte[] bytes)
    {
        if (bytes.Length == 0) return 0;
        try
        {
            return BitConverter.ToInt32(bytes);
        }
        catch (Exception e)
        {
            return 0;
        }
    }
}