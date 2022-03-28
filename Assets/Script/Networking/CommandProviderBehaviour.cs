using System;
using UnityEngine;

namespace Script.Networking
{
    public abstract class CommandProviderBehaviour : MonoBehaviour
    {
        public object InfoStruct;
        [NonSerialized] public volatile bool IsNeeded;
        protected NetworkedGame NetworkedGame;

        private void Awake()
        {
            NetworkedGame = FindObjectOfType<NetworkedGame>();
        }

        private void Update()
        {
            if (IsNeeded)
            {
                IsNeeded = false;
                DoAction();
            }
        }

        protected abstract void DoAction();
    }
}