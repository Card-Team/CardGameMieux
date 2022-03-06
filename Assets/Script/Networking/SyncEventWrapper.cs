using System;
using System.Threading;
using CardGameEngine.EventSystem;
using UnityEngine;
using Event = CardGameEngine.EventSystem.Events.Event;

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

            private volatile bool _processed;

            public UnityEventHandler(EventManager.OnEvent<T> evt, SyncEventsManager syncEventsManager)
            {
                _evt = evt;
                _syncEventsManager = syncEventsManager;
            }


            private Action _handleEventAction;

            public void HandleEvent(Event evt)
            {
                _handleEventAction = () =>
                {
                    _evt.Invoke((T)evt);
                    _processed = true;
                };
                Debug.Log("On a pris le lock, on balance au manager");
                _processed = false;

                _syncEventsManager.eventHandlers.Enqueue(_handleEventAction);
                while (_processed == false && Thread.CurrentThread.ThreadState == ThreadState.Running)
                {   
                    Thread.Sleep(1);
                }

                Debug.Log("On a été notifiés");
        }
    }
}

}