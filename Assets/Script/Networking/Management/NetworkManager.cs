using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Network;
using Network.Enums;
using Network.Interfaces;
using Script.Networking.Management;
using UnityEngine;

namespace Script.Networking
{
    public class NetworkManager : MonoBehaviour
    {
        #region parametres reseaux

        private NetworkConfiguration _networkConfiguration =
            new NetworkConfiguration()
            {
                NetworkMode =
#if UNITY_EDITOR
                    NetworkMode.Server,
#else
                    NetworkMode.Client,
#endif
                IPAddress = IPAddress.IPv6Loopback,
                Port = 1246
            };

        #endregion

        public bool IsServer { get; private set; }

        public bool IsClient => !IsServer;

        private bool networkinSetUp = false;


        public ClientManager ClientManager { get; private set; }

        public ServerManager ServerManager { get; private set; }


        public void SetupNetworking(PacketReceivedHandler<GameCommand> receiver, Action onOtherSideConnect)
        {
            if (networkinSetUp)
            {
                Debug.LogWarning("Trying to setup networking twice");
                return;
            }

            switch (_networkConfiguration.NetworkMode)
            {
                case NetworkMode.Client:
                    Debug.Log($"Trying to connect to {_networkConfiguration.IPAddress} : {_networkConfiguration.Port}");
                    ClientManager = new ClientManager(_networkConfiguration, receiver, onOtherSideConnect);
                    ClientManager.Connect();
                    IsServer = false;
                    break;
                case NetworkMode.Server:
                    Debug.Log($"Listening on : {_networkConfiguration.Port}");
                    ServerManager = new ServerManager(_networkConfiguration, receiver, onOtherSideConnect);
                    ServerManager.Host();
                    IsServer = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Send(GameCommand gameCommand)
        {
            switch (_networkConfiguration.NetworkMode)
            {
                case NetworkMode.Client:
                    ClientManager.Server.Send(gameCommand,ClientManager);
                    break;
                case NetworkMode.Server:
                    ServerManager.Client.Send(gameCommand);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnDestroy()
        {
            switch (_networkConfiguration.NetworkMode)
            {
                case NetworkMode.Client:
                    ClientManager.Stop();
                    break;
                case NetworkMode.Server:
                    ServerManager.Stop();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}