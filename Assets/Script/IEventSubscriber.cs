using CardGameEngine.EventSystem;

public interface IEventSubscriber
{
    void Subscribe(EventManager eventManager);
}