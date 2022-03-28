using System.Collections.Generic;

namespace Script.Networking.Commands
{
    public class PlayCardCommand : GameCommand
    {
        public PlayCardCommand(int playerId, int cardId, bool upgrade)
        {
            PlayerId = playerId;
            CardId = cardId;
            Upgrade = upgrade;
        }

        public int PlayerId { get; }
        public int CardId { get; }
        public bool Upgrade { get; }

        public override IDictionary<string, string> ToDict(UnityGame unityGame)
        {
            return new Dictionary<string, string>
            {
                {"Player", PlayerId.ToString()},
                {"Card", $"({CardId}){unityGame.Network.ResolveCard(CardId)}"},
                {"Upgrade", Upgrade.ToString()}
            };
        }
    }
}