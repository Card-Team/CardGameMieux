using Network.Attributes;
using Network.Packets;

namespace Script.Networking.Management.EstablishmentPackets
{
    [PacketRequest(typeof(ConnectionRequest))]
    public class ConnectionAcceptation : ResponsePacket
    {
        public int ResumeToken { get; set; }

        public ConnectionAcceptation(RequestPacket packet) : base(packet)
        {
        }
    }
}