using System;
using Script.Networking.Commands.Extern;
using UnityEngine;

namespace Script.Networking
{
    public abstract class CommandProviderBehaviour : MonoBehaviour
    {
        [NonSerialized] public volatile bool IsNeeded;
        public object InfoStruct;
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