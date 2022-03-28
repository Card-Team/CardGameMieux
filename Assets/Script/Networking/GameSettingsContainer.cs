using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace Script.Networking
{
    internal class GameSettingsContainer : MonoBehaviour
    {
        public NetworkMode NetworkMode;
        public IPAddress IPAddress;
        public int port;
        public IEnumerable<string> playerDeck;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}