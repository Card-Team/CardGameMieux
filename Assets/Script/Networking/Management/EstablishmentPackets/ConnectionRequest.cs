using Network.Packets;

namespace Script.Networking.Management.EstablishmentPackets
{
    public class ConnectionRequest : RequestPacket
    {
        public int? ResumeToken { get; set; }
    }
}