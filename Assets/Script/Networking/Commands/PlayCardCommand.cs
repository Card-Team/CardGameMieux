using System.Collections.Generic;
using CardGameEngine;

namespace Script.Networking.Commands
{
    public class PlayCardCommand : GameCommand
    {
        public int PlayerId { get; private set; }
        public int CardId { get; private set; }
        public bool Upgrade { get; private set; }

        public PlayCardCommand(int playerId, int cardId, bool upgrade)
        {
            PlayerId = playerId;
            CardId = cardId;
            Upgrade = upgrade;
        }

        public override IDictionary<string, string> ToDict(UnityGame unityGame)
        {
            return new Dictionary<string, string>
            {
                { "Player", PlayerId.ToString() },
                { "Card", $"({CardId}){unityGame.Network.ResolveCard(CardId)}" },
                { "Upgrade", Upgrade.ToString() },
            };
        }
    }
}