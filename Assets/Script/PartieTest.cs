using System.Collections;
using System.Collections.Generic;
using CardGameEngine;
using CardGameEngine.Cards;
using CardGameEngine.GameSystems;
using UnityEngine;

public class PartieTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        var partie = new Game(Application.streamingAssetsPath + "/EffectsScripts/", new externCallbacksNull(),
            new[] {"pistolet", "chance"}, new[] {"mongolfiere", "extracteur"});
        partie.StartGame();

        var p1FirstCard = partie.Player1.Hand[0];
        FindObjectOfType<CardRenderer>().card = p1FirstCard;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private class externCallbacksNull : IExternCallbacks
    {
        public Card ExternCardAskForTarget(Player effectOwner, string targetName, List<Card> cardList)
        {
            throw new System.NotImplementedException();
        }

        public Player ExternPlayerAskForTarget(Player effectOwner, string targetName)
        {
            throw new System.NotImplementedException();
        }

        public void ExternShowCard(Player player, Card card)
        {
            throw new System.NotImplementedException();
        }

        public Card ExternChooseBetween(Player player, List<Card> card)
        {
            throw new System.NotImplementedException();
        }

        public void ExternGameEnded(Player winner)
        {
            throw new System.NotImplementedException();
        }

        public bool ExternChainOpportunity(Player player)
        {
            throw new System.NotImplementedException();
        }

        public void DebugPrint(string @from, string source, string debugPrint)
        {
            throw new System.NotImplementedException();
        }
    }
}