using System;
using UnityEngine;

namespace Script.Networking
{
    public abstract class CommandProviderBehaviour : MonoBehaviour
    {
        public volatile bool isNeeded;
        public ExternData infoStruct;

        private void Update()
        {
            if (isNeeded)
            {
                isNeeded = false;
                DoAction();
            }
        }

        protected abstract void DoAction();
    }
}