using CardGameEngine.EventSystem;
using CardGameEngine.EventSystem.Events;

namespace Script.Networking
{
    public class SyncEventWrapper
    {
        public SyncEventsManager SyncEventsManager;

        public EventManager.IEventHandler SubscribeToEvent<T>(EventManager.OnEvent<T> deleg,
            bool evenIfCancelled = false,
            bool postEvent = false)
            where T : Event
        {
            var unityEventHandler = new UnityEventHandler<T>(deleg, SyncEventsManager);
            return SyncEventsManager.EventManager.SubscribeToEvent<T>(e => unityEventHandler.HandleEvent(e),
                evenIfCancelled, postEvent);
        }

        private class UnityEventHandler<T> where T : Event
        {
            private readonly EventManager.OnEvent<T> _evt;
            private readonly SyncEventsManager _syncEventsManager;

            public UnityEventHandler(EventManager.OnEvent<T> evt,
                SyncEventsManager syncEventsManager)
            {
                _evt = evt;
                _syncEventsManager = syncEventsManager;
            }


            public void HandleEvent(Event evt)
            {
                _syncEventsManager.WaitForEventCalled(evt, _evt);
            }
        }
    }
}