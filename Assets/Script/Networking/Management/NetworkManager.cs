using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Network;
using Network.Enums;
using Network.Interfaces;
using Network.Packets;
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
#if !UNITY_EDITOR
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

        public bool IsConnected => _networkConfiguration.NetworkMode switch
        {
            NetworkMode.Client => ClientManager.ConnectionState,
            NetworkMode.Server => ServerManager.ConnectionState,
            _ => throw new ArgumentOutOfRangeException()
        } == ConnectionState.CONNECTED;


        private ClientManager ClientManager { get; set; }

        private ServerManager ServerManager { get; set; }


        public void SetupNetworking(Action onOtherSideConnect)
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
    }
}