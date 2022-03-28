using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Network.Enums;
using Network.Packets;

namespace Network
{
    /// <summary>
    ///     Builds upon the <see cref="Connection" /> class, implementing TCP and allowing for messages to be conveniently
    ///     sent without a large serialisation header.
    /// </summary>
    public class TcpConnection : Connection
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TcpConnection" /> class.
        /// </summary>
        /// <param name="tcpClient">The TCP client to use.</param>
        /// <param name="skipInitializationProcess">
        ///     Whether to skip the initialisation process for the underlying <see cref="Connection" />. If <c>true</c>
        ///     <see cref="Connection.Init()" /> will have to be manually called later.
        /// </param>
        internal TcpConnection(TcpClient tcpClient, bool skipInitializationProcess = false)
        {
            client = tcpClient;
            socket = tcpClient.Client;
            stream = client.GetStream();

            KeepAlive = true;
            ForceFlush = true;
            tcpClient.NoDelay = true;
            tcpClient.SendTimeout = 0;
            tcpClient.ReceiveTimeout = 0;
            tcpClient.LingerState = new LingerOption(false, 0);

            //The initialization has to be done elsewhere.
            //The caller of the constructor wants to apply
            //additional settings before starting the network comm.
            if (!skipInitializationProcess)
                Init();
        }

        #endregion Constructors

        #region Variables

        /// <summary>
        ///     The <see cref="TcpClient" /> for this <see cref="TcpConnection" /> instance.
        /// </summary>
        private readonly TcpClient client;

        /// <summary>
        ///     The <see cref="NetworkStream" /> on which to send and receive data.
        /// </summary>
        private readonly NetworkStream stream;

        /// <summary>
        ///     The <see cref="Socket" /> for this <see cref="TcpConnection" /> instance.
        /// </summary>
        private readonly Socket socket;

        #endregion Variables

        #region Properties

        /// <inheritdoc />
        public override IPEndPoint IPLocalEndPoint => (IPEndPoint) client?.Client?.LocalEndPoint;

        /// <summary>
        ///     The local <see cref="EndPoint" /> for the <see cref="socket" />.
        /// </summary>
        public EndPoint LocalEndPoint => socket.LocalEndPoint;

        /// <inheritdoc />
        public override IPEndPoint IPRemoteEndPoint => (IPEndPoint) client?.Client?.RemoteEndPoint;

        /// <summary>
        ///     The remote <see cref="EndPoint" /> for the <see cref="socket" />.
        /// </summary>
        public EndPoint RemoteEndPoint => socket.RemoteEndPoint;

        /// <inheritdoc />
        public override bool DualMode
        {
            get => socket.DualMode;
            set => socket.DualMode = value;
        }

        /// <inheritdoc />
        public override bool Fragment
        {
            get => !socket.DontFragment;
            set => socket.DontFragment = !value;
        }

        /// <inheritdoc />
        public override int HopLimit
        {
            get => (int) socket.GetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.HopLimit);
            set => socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.HopLimit, value);
        }

        /// <inheritdoc />
        public override bool IsRoutingEnabled
        {
            get => !(bool) socket.GetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.DontRoute);
            set => socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.DontRoute, !value);
        }

        /// <inheritdoc />
        public override bool NoDelay
        {
            get => client.Client.NoDelay;
            set => client.Client.NoDelay = value;
        }

        /// <inheritdoc />
        public override short TTL
        {
            get => socket.Ttl;
            set => socket.Ttl = value;
        }

        /// <inheritdoc />
        public override bool UseLoopback
        {
            get => (bool) socket.GetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.UseLoopback);
            set => socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.UseLoopback, value);
        }

        #endregion Properties

        #region Methods

        /// <summary>
        ///     Establishes a <see cref="UdpConnection" /> with the remote endpoint.
        /// </summary>
        /// <param name="connectionEstablished">The action to perform upon connection.</param>
        internal void EstablishUdpConnection(Action<IPEndPoint, IPEndPoint> connectionEstablished)
        {
            var localEndpoint = new IPEndPoint(IPAddress.IPv6Any, GetFreePort());

            RegisterPacketHandler<EstablishUdpResponse>((packet, connection) =>
            {
                UnRegisterPacketHandler<EstablishUdpResponse>(this);
                connectionEstablished.Invoke(localEndpoint, new IPEndPoint(IPRemoteEndPoint.Address, packet.UdpPort));
                Send(new EstablishUdpResponseACK());
            }, this);

            Send(new EstablishUdpRequest(localEndpoint.Port), this);
        }

        /// <inheritdoc />
        protected override byte[] ReadBytes(int amount)
        {
            if (amount == 0) return new byte[0];
            var requestedBytes = new byte[amount];
            var receivedIndex = 0;

            while (receivedIndex < amount)
            {
                while (client.Available == 0)
                    Thread.Sleep(IntPerformance);

                var clientAvailable = client.Available;
                var readAmount = amount - receivedIndex >= clientAvailable ? clientAvailable : amount - receivedIndex;
                stream.Read(requestedBytes, receivedIndex, readAmount);
                receivedIndex += readAmount;
            }

            return requestedBytes;
        }

        /// <inheritdoc />
        protected override void WriteBytes(byte[] bytes)
        {
            stream.Write(bytes, 0, bytes.Length);
            if (ForceFlush) stream.Flush();
        }

        /// <inheritdoc />
        /// <remarks>
        ///     Since TCP ensures the ordering of packets, we will always receive the <see cref="AddPacketTypeRequest" /> before
        ///     a <see cref="Packet" /> of the unknown type. Thus, it is theoretically impossible that this method is called for
        ///     a <see cref="TcpConnection" /> instance. Still gotta handle it though :),
        /// </remarks>
        protected override void HandleUnknownPacket()
        {
            Logger.Log("Connection can't handle the received packet. No listener defined.", LogLevel.Error);
            CloseHandler(CloseReason.ReadPacketThreadException);
        }

        /// <inheritdoc />
        protected override void CloseHandler(CloseReason closeReason)
        {
            Close(closeReason, true);
        }

        /// <inheritdoc />
        protected override void CloseSocket()
        {
            stream.Close();
            client.Close();
        }

        #endregion Methods
    }
}