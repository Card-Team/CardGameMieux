using Network.Attributes;
using Network.Packets;

namespace Script.Networking.Management.EstablishmentPackets
{
    [PacketRequest(typeof(ConnectionRequest))]
    public class ConnectionAcceptation : ResponsePacket
    {
        public ConnectionAcceptation(RequestPacket packet) : base(packet)
        {
        }

        public int ResumeToken { get; set; }
    }
}