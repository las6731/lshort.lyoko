namespace LShort.Lyoko.Messaging.Abstractions;

public interface ITypedMessageSubscriber<in T> : IMessageSubscriber
{
    /// <inheritdoc />
    Task<bool> IMessageSubscriber.ConsumeEvent(EventMessage e)
    {
        var payload = e.GetPayload<T>();

        return this.ConsumeEvent(e, payload);
    }

    public Task<bool> ConsumeEvent(EventMessage e, T payload);
}