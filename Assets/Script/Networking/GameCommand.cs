using Network.Packets;

namespace Script.Networking
{
    public class GameCommand : RequestPacket
    {
        public string data { get; set; } = "";
        //todo
    }
}