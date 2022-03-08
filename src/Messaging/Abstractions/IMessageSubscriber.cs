namespace LShort.Lyoko.Messaging.Abstractions;

public interface IMessageSubscriber
{
    public Task<bool> ConsumeEvent(EventMessage e);
}