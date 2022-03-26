namespace LShort.Lyoko.Messaging.Abstractions.Services;

public interface IMessagingService
{
    /// <summary>
    /// Publish an event to the default exchange.
    /// </summary>
    /// <param name="e">The event to publish.</param>
    public void Publish(EventMessage e);

    /// <summary>
    /// Publish an event to an exchange.
    /// </summary>
    /// <param name="exchange">The exchange name.</param>
    /// <param name="e">The event to publish.</param>
    public void Publish(string exchange, EventMessage e);
}