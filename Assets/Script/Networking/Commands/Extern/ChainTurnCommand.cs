using System.Collections.Generic;

namespace Script.Networking.Commands.Extern
{
    public class ChainTurnCommand : ExternalCommand
    {
        public int CardId { get; set; } //négatif si jamais le joueur cancel

        public override IDictionary<string, string> ToDict(UnityGame unityGame)
        {
            return new Dictionary<string, string> {{"CardId", CardId.ToString()}};
        }
    }
}