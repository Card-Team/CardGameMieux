using System;
using System.Reflection;
using Network;
using Network.Enums;
using Script.Networking.Management.EstablishmentPackets;
using UnityEngine;

namespace Script.Networking.Management
{
    public class ClientManager : BiManager
    {
        private ClientConnectionContainer _clientConnectionContainer;

        private int?
            _resumeToken; // jeton donné par le serveur en début de connection et permet de se reco en continuant

        public ClientManager(NetworkConfiguration networkConfiguration, Action onOtherSideConnect) : base(
            networkConfiguration, onOtherSideConnect)
        {
        }
        // si on donne pas ca il va penser qu'on est un random


        public Connection Server => Other;
        //tout les callbacks sont sur un autre thread
        // faut un lien pour pouvoir appeler les composants sur le thread principal
        // normalement c'est bon ?

        public void Connect()
        {
            _clientConnectionContainer = ConnectionFactory.CreateClientConnectionContainer(
                _networkConfiguration.IPAddress.ToString()
                , _networkConfiguration.Port, false);
            _clientConnectionContainer.AddKownType(Assembly.GetExecutingAssembly());
            _clientConnectionContainer.ConnectionEstablished += OnClientConnect;
            _clientConnectionContainer.ConnectionLost += OnConnectionLost;
            _clientConnectionContainer.AutoReconnect = true;

            _clientConnectionContainer.Initialize();
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

            server.RegisterPacketHandler<ConnectionAcceptation>(ConnectionAcceptation, this);

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
            _onOtherSideConnect();
            ConnectionState = ConnectionState.CONNECTED;
        }


        public void Stop()
        {
            _clientConnectionContainer.Shutdown(CloseReason.ClientClosed);
        }
    }
}