using System.Collections.Generic;
using CardGameEngine.Cards;
using CardGameEngine.GameSystems;

namespace Script
{
    internal class DumbCallbacks : IExternCallbacks
    {
        public Card ExternCardAskForTarget(Player effectOwner, string targetName, List<Card> cardList)
        {
            return cardList[0];
        }

        public Player ExternPlayerAskForTarget(Player effectOwner, string targetName)
        {
            return effectOwner;
        }

        public void ExternShowCard(Player player, Card card)
        {
            //empty
        }

        public Card ExternChooseBetween(Player player, List<Card> card)
        {
            return card[0];
        }

        public void ExternGameEnded(Player winner)
        {
            //empty
        }

        public bool ExternChainOpportunity(Player player)
        {
            return false;
        }

        public void DebugPrint(string from, string source, string debugPrint)
        {
            //empty
        }

        public int GetExternalRandomNumber(int a, int b)
        {
            return a;
        }
    }
}