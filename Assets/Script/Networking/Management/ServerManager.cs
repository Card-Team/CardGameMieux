using System;
using System.Reflection;
using Network;
using Network.Enums;
using Network.Interfaces;
using Script.Networking.Management.EstablishmentPackets;
using UnityEngine;
using Random = System.Random;

namespace Script.Networking.Management
{
    public class ServerManager : BiManager
    {
        private ServerConnectionContainer _serverConnectionContainer;

        private int? ResumeToken;

        public Connection Client => Other;

        public ServerManager(NetworkConfiguration networkConfiguration, Action onOtherSideConnect) : base(
            networkConfiguration, onOtherSideConnect)
        {
        }

        public void Host()
        {
            _serverConnectionContainer =
                ConnectionFactory.CreateServerConnectionContainer(_networkConfiguration.IPAddress.ToString(),
                    _networkConfiguration.Port);
            _serverConnectionContainer.AddKownType(Assembly.GetExecutingAssembly());
            _serverConnectionContainer.ConnectionEstablished += OnClientConnect;
            _serverConnectionContainer.ConnectionLost += OnClientLost;
            _serverConnectionContainer.AllowUDPConnections = false;
        }

        private void OnClientLost(Connection connection, ConnectionType connectionType, CloseReason closeReason)
        {
            ConnectionState = ConnectionState.NOT_CONNECTED;
            Debug.Log($"Close reason : {closeReason}");
        }

        private void OnClientConnect(Connection connection, ConnectionType connectionType)
        {
            ConnectionState = ConnectionState.ESTABLISHMENT;
            Debug.Log("Received client");
            connection.LogIntoStream(GetDebugLogStream());
            connection.EnableLogging = true;
            Other = connection;
            Other.RegisterStaticPacketHandler<ConnectionRequest>(OnConnectionRequest);
        }

        private void OnConnectionRequest(ConnectionRequest packet, Connection connection)
        {
            Debug.Log("Received connection request");
            if (ConnectionState != ConnectionState.ESTABLISHMENT)
            {
                Debug.LogError("Received request while not in establishment");  
                return;
            }

            if (ResumeToken == null)
            {
                ResumeToken = new Random().Next(16000);
                Other.Send(new ConnectionAcceptation(packet) { ResumeToken = ResumeToken.Value });
                ConnectionState = ConnectionState.CONNECTED;
                Debug.Log("First time connection");
            }
            else
            {
                if (ResumeToken == packet.ResumeToken)
                {
                    Other.Send(new ConnectionAcceptation(packet) { ResumeToken = ResumeToken.Value });
                    ConnectionState = ConnectionState.CONNECTED;
                    Debug.Log("resumed connection");
                }
                else
                {
                    Debug.Log("Fail resume, suspicious");
                    Other.Close(CloseReason.ServerClosed, true);
                    // ConnectionState = ConnectionState.NOT_CONNECTED;
                }
            }

            if (ConnectionState != ConnectionState.CONNECTED) return;
            _onOtherSideConnect();
        }

        public void Stop()
        {
            _serverConnectionContainer.Stop();
        }
    }
}