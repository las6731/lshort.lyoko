namespace LShort.Lyoko.Messaging.Abstractions.Services;

public interface IFailedConsumptionSupervisor
{
    /// <summary>
    /// Determines if a message has exceeded the configured amount of retries.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>A boolean indicating if the message should not be retried.</returns>
    public Task<bool> HasExceededRetryCount(EventMessage message);
}