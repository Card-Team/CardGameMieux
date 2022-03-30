using System.Collections.Generic;
using System.Linq;
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
            
            var oldContainer = FindObjectsOfType<GameSettingsContainer>().FirstOrDefault(o => o != this);
            if(oldContainer != null) Destroy(oldContainer);
            DontDestroyOnLoad(this);
        }
    }
}