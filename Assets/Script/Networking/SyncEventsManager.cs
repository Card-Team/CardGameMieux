using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using CardGameEngine.EventSystem;
using UnityEngine;
using Event = CardGameEngine.EventSystem.Events.Event;

namespace Script.Networking
{
    public class SyncEventsManager : MonoBehaviour
    {
        public ConcurrentQueue<(TaskCompletionSource<bool>, Action)> eventHandlers =
            new ConcurrentQueue<(TaskCompletionSource<bool>, Action)>();

        public SyncEventWrapper SyncEventWrapper;

        public EventManager EventManager;
        public NetworkedGame networkedGame;

        private void Awake()
        {
            networkedGame = FindObjectOfType<NetworkedGame>();
        }

        private void Start()
        {
            SyncEventWrapper = new SyncEventWrapper
            {
                SyncEventsManager = this
            };
        }

        public void WaitForEventCalled<T>(Event evt, EventManager.OnEvent<T> onEvent)
            where T : Event
        {
            var tcs = new TaskCompletionSource<bool>();
            eventHandlers.Enqueue((tcs, () => onEvent((T)evt)));
            try
            {
                networkedGame.WaitForTaskWithPolling(tcs, true);
            }
            catch (ThreadInterruptedException)
            {
                Debug.Log("Thread interrompu, on arette d'attendre l'event");
            }
        }

        private void Update()
        {
            if (eventHandlers.TryDequeue(out var elem))
            {
                var (tcs, action) = elem;
                action();
                tcs.SetResult(true);
                lock (networkedGame)
                {
                    Monitor.PulseAll(networkedGame);
                }
            }
        }
    }
}