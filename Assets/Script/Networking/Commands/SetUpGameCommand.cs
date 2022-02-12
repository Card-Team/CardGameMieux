using System.Collections.Generic;
using Network.Packets;

namespace Script.Networking.Commands
{
    public class SetUpGameCommand : RequestPacket

    {
        public string name { get; set; }
        public List<string> deck;
        public int randomSeed { get; set; }
    }
}