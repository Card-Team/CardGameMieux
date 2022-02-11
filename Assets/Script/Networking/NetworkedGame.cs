using System;
using System.IO;
using System.Text;
using CardGameEngine;
using Network;
using Network.Packets;
using Script.Debugging;
using UnityEngine;

namespace Script.Networking
{
    public class NetworkedGame : MonoBehaviour
    {
        private Game _game;

        private NetworkManager _networkManager;

        public readonly bool IsReadyToStart;


        private void Awake()
        {
            FindObjectOfType<DebugConsole>().Init();
            _networkManager = GetComponent<NetworkManager>();
        }

        private void OnEnable()
        {
            _networkManager.SetupNetworking(ReceiveRemoteAction, () =>
            {
                Debug.Log("Connection established");
                if (_networkManager.IsServer)
                {
                    Debug.Log("Sending packet");
                    _networkManager.Send(new GameCommand(){data = "salaasdsdqsd"});
                }
            });
        }

        public void SetUpNetworkGame()
        {
            if (_game != null)
            {
                Debug.LogError("Trying to setup game after creation");
                return;
            }
            
            //todo
 
            
        }

        
        
        //todo explications
        // Déja ca fonctionne que en ipv6 je crois ?
        // pas sur mais pas le temps de test la, quqnd ca fonctionnait pas le port etait pas bon aussi
        // 
        //
        // Les deux doivent RegisterKnownTypes, mais je crois que ca le fait tout seul dans les containers
        // Le CLIENT, doit ajouter un listener avec PacketHandler et un objet
        // Le SERVEUR doit utiliser StaticPacketHandler
        // Quand le client envoit un paquet, il doit utiliser l'objet associé (pk ????)
        
        // Les paquets héritent RequestPacket
        // et potentiellement un responsePacket, avec une annotation pour dire il correspond a quel request
        // les champs doivent etre des PROPRIETES
        // sinon ca marche c cool

        private void ReceiveRemoteAction(GameCommand packet, Connection connection)
        {
            Debug.Log($"J'ai recu un paket avec {packet.data}");
            //todo process action
            // mais checker que c'est une action de j2
        }

       

        public void DoLocalAction(GameCommand command)
        {
            //todo
        }
        
        
    }
}