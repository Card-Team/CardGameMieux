using System.Collections.Generic;
using Network.Packets;

namespace Script.Networking.Commands
{
    public abstract class GameCommand : Packet
    {
        public abstract IDictionary<string, string> ToDict(UnityGame unityGame);
    }
}