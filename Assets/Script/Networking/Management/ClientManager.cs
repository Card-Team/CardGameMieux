using System;
using System.Reflection;
using JetBrains.Annotations;
using Network;
using Network.Enums;
using Network.Interfaces;
using Network.Packets;
using Script.Networking.Management.EstablishmentPackets;
using UnityEngine;

namespace Script.Networking.Management
{
    public class ClientManager : BiManager
    {
        private ClientConnectionContainer _clientConnectionContainer;

        private int? _resumeToken; // jeton donné par le serveur en début de connection et permet de se reco en continuant
        // si on donne pas ca il va penser qu'on est un random
        
        

        public Connection Server => Other;

        public ClientManager(NetworkConfiguration networkConfiguration,
            PacketReceivedHandler<GameCommand> gameCommandReiceiver, Action onOtherSideConnect) : base(
            networkConfiguration, gameCommandReiceiver, onOtherSideConnect)
        {
        }
        //todo tout les callbacks sont sur un autre thread
        // faut un lien pour pouvoir appeler les composants sur le thread principal

        public void Connect()
        {
            _clientConnectionContainer = ConnectionFactory.CreateClientConnectionContainer(
                _networkConfiguration.IPAddress.ToString()
                , _networkConfiguration.Port);
            _clientConnectionContainer.AddKownType(Assembly.GetExecutingAssembly());
            _clientConnectionContainer.ConnectionEstablished += OnClientConnect;
            _clientConnectionContainer.ConnectionLost += OnConnectionLost;
            _clientConnectionContainer.AutoReconnect = true;
        }

        private void OnConnectionLost(Connection connection, ConnectionType connectionType, CloseReason closeReason)
        {
            Other = null;
            ConnectionState = ConnectionState.NOT_CONNECTED;
            Debug.Log($"Close Reaseon : {closeReason}");
        }

        private void OnClientConnect(Connection server, ConnectionType arg2)
        {
            Debug.Log("Server responded");
            server.LogIntoStream(GetDebugLogStream());
            server.EnableLogging = true;
            Other = server;
            
            server.RegisterPacketHandler<ConnectionAcceptation>(ConnectionAcceptation,this);

            ConnectionState = ConnectionState.ESTABLISHMENT;
            
            _clientConnectionContainer.Send(new ConnectionRequest {ResumeToken = _resumeToken});
        }

        private void ConnectionAcceptation(ConnectionAcceptation packet, Connection connection)
        {
            Debug.Log("Received acceptation");
            if (ConnectionState != ConnectionState.ESTABLISHMENT)
            {
                Debug.LogError("Received acceptation while not in establishment");
                return;
            }
            _resumeToken = packet.ResumeToken;
            Other.RegisterPacketHandler(_gameCommandReiceiver,this);
            _onOtherSideConnect();
            ConnectionState = ConnectionState.CONNECTED;
        }
        

        public void Stop()
        {
            _clientConnectionContainer.Shutdown(CloseReason.ClientClosed);
        }
    }
}