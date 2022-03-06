using System;
using System.Collections.Concurrent;
using System.Threading;
using CardGameEngine.EventSystem;
using UnityEngine;

namespace Script.Networking
{
    public class SyncEventsManager : MonoBehaviour
    {
        public ConcurrentQueue<Action> eventHandlers =
            new ConcurrentQueue<Action>();

        public SyncEventWrapper SyncEventWrapper;

        public EventManager EventManager;

        private void Start()
        {
            SyncEventWrapper = new SyncEventWrapper
            {
                SyncEventsManager = this
            };
        }

        private void Update()
        {
            if (eventHandlers.TryDequeue(out var elem))
            {
                elem.Invoke();
            }
        }

       
    }
}