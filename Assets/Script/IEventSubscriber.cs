using Script.Networking;

public interface IEventSubscriber
{
    void Subscribe(SyncEventWrapper eventManager);
}