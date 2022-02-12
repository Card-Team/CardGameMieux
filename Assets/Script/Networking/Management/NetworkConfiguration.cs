using System.Net;

namespace Script.Networking
{
    public struct NetworkConfiguration
    {
        public NetworkMode NetworkMode { get; set; }
        public IPAddress IPAddress { get; set; }
        public int Port { get; set; }
    }
}