namespace LShort.Lyoko.Messaging.Abstractions;

/// <summary>
/// Subscribes to messages from a messaging service.
/// </summary>
public interface IMessageSubscriber
{
    /// <summary>
    /// Consumes an event.
    /// </summary>
    /// <param name="e">The event.</param>
    /// <returns>A boolean indicating whether the event was consumed successfully.</returns>
    public Task<bool> ConsumeEvent(EventMessage e);
}