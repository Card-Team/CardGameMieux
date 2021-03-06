using System;
using Network.Interfaces;
using Network.Packets;
using Script.Networking.Management;
using UnityEngine;

namespace Script.Networking
{
    public class NetworkManager : MonoBehaviour
    {
        #region parametres reseaux

        private NetworkConfiguration _networkConfiguration;

        #endregion

        private readonly bool networkinSetUp = false;

        public bool IsServer { get; private set; }

        public bool IsClient => !IsServer;

        public bool IsConnected => _networkConfiguration.NetworkMode switch
        {
            NetworkMode.Client => ClientManager.ConnectionState,
            NetworkMode.Server => ServerManager.ConnectionState,
            _ => throw new ArgumentOutOfRangeException()
        } == ConnectionState.CONNECTED;


        private ClientManager ClientManager { get; set; }

        private ServerManager ServerManager { get; set; }

        private void OnDestroy()
        {
            // on coupe la co 
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


        public void SetupNetworking(NetworkConfiguration otherSideConnect, Action onOtherSideConnect)
        {
            _networkConfiguration = otherSideConnect;

            if (networkinSetUp)
            {
                Debug.LogWarning("Trying to setup networking twice");
                return;
            }

            switch (_networkConfiguration.NetworkMode)
            {
                case NetworkMode.Client:
                    Debug.Log($"Trying to connect to {_networkConfiguration.IPAddress} : {_networkConfiguration.Port}");
                    ClientManager = new ClientManager(_networkConfiguration, onOtherSideConnect);
                    ClientManager.Connect();
                    IsServer = false;
                    break;
                case NetworkMode.Server:
                    Debug.Log($"Listening on : {_networkConfiguration.Port}");
                    ServerManager = new ServerManager(_networkConfiguration, onOtherSideConnect);
                    ServerManager.Host();
                    IsServer = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Send(Packet packet)
        {
            if (IsConnected == false)
            {
                Debug.LogError("Trying to send but connection dropped");
                return;
            }

            switch (_networkConfiguration.NetworkMode)
            {
                case NetworkMode.Client:
                    ClientManager.Server.Send(packet, ClientManager);
                    break;
                case NetworkMode.Server:
                    ServerManager.Client.Send(packet);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void AddPacketHandler<T>(PacketReceivedHandler<T> onPacket) where T : Packet
        {
            switch (_networkConfiguration.NetworkMode)
            {
                case NetworkMode.Client:
                    ClientManager.Server.RegisterPacketHandler(onPacket, ClientManager);
                    break;
                case NetworkMode.Server:
                    ServerManager.Client.RegisterStaticPacketHandler(onPacket);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}