using System;
using System.Threading.Tasks;

namespace Network.RSA
{
    /// <summary>
    ///     A secure <see cref="ClientConnectionContainer" />, implementing RSA encryption.
    /// </summary>
    /// <seealso cref="ClientConnectionContainer" />
    public class SecureClientConnectionContainer : ClientConnectionContainer
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SecureClientConnectionContainer" /> class.
        /// </summary>
        /// <param name="ipAddress">The remote ip address.</param>
        /// <param name="port">The remote port.</param>
        /// <param name="rsaPair">The local RSA key-pair.</param>
        internal SecureClientConnectionContainer(string ipAddress, int port, RSAPair rsaPair) : base(ipAddress, port)
        {
            RSAPair = rsaPair;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        ///     Creates and returns a new <see cref="SecureTcpConnection" />.
        /// </summary>
        /// <returns>The created <see cref="SecureTcpConnection" />.</returns>
        protected override async Task<Tuple<TcpConnection, ConnectionResult>> CreateTcpConnection()
        {
            return await ConnectionFactory.CreateSecureTcpConnectionAsync(IPAddress, Port, RSAPair);
        }

        /// <summary>
        ///     Creates and returns a new <see cref="SecureUdpConnection" />, with the current <see cref="SecureTcpConnection" />
        ///     as the parent.
        /// </summary>
        /// <returns>The created <see cref="SecureUdpConnection" />.</returns>
        protected override async Task<Tuple<UdpConnection, ConnectionResult>> CreateUdpConnection()
        {
            return await ConnectionFactory.CreateSecureUdpConnectionAsync(TcpConnection, RSAPair);
        }

        #endregion Methods
    }
}