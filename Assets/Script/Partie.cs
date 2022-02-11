using System.Collections.Generic;
using CardGameEngine;
using CardGameEngine.Cards;
using CardGameEngine.GameSystems;
using UnityEngine;

public class Partie : MonoBehaviour
{
    public Game Game;

    // Start is called before the first frame update
    void Start()
    {
        // Game = new Game(Application.streamingAssetsPath + "/EffectsScripts/", new ExternCallbacks(),
        //     null, null);
        //
        // Game.StartGame();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private class ExternCallbacks : IExternCallbacks
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