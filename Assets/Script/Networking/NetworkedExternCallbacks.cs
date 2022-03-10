using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using CardGameEngine;
using CardGameEngine.Cards;
using CardGameEngine.GameSystems;
using Script.Networking.Commands;
using Script.Networking.Commands.Extern;
using UnityEngine;
using Random = System.Random;

namespace Script.Networking.Commands.Extern
{
}

namespace Script.Networking
{
    public class NetworkedExternCallbacks : IExternCallbacks
    {
        private readonly NetworkedGame _networkedGame;
        private readonly Random _random;

        public NetworkedExternCallbacks(int randomSeed,NetworkedGame _networkedGame)
        {
            this._random = new Random(randomSeed);
            this._networkedGame = _networkedGame;
        }

        public Card ExternCardAskForTarget(Player effectOwner, string targetName, List<Card> cardList)
        {
            Debug.Log($"AskForTarget for player {effectOwner.Id}");
            Debug.Log("Network Ask Target");
            if (UnityGame.IsLocalPlayer(effectOwner))
            {
                Debug.Log("Want local");
                _networkedGame.WantLocal<ChooseCardTargetCommand>(new ChooseCardTargetData() {TargetName = targetName,CardList =  cardList});
            }

            var cardCommand = _networkedGame.WaitForExternalCommand<ChooseCardTargetCommand>();

            Card card = _networkedGame.ResolveCard(cardCommand.CardId);

            Debug.Log("Received card");

            return card;
        }

        public Player ExternPlayerAskForTarget(Player effectOwner, string targetName)
        {
            if (UnityGame.IsLocalPlayer(effectOwner))
            {
                _networkedGame.WantLocal<ChooseCardTargetCommand>(new ChoosePlayerTargetData() {TargetName = targetName});
            }

            var command= _networkedGame.WaitForExternalCommand<ChoosePlayerTargetCommand>();

            var player = _networkedGame.ResolvePlayer(command.PlayerId);
            
            Debug.Log("Received player");

            return player;
        }

        public void ExternShowCard(Player player, Card card)
        {
            _networkedGame.WantLocal<ShowCardFalseCommand>( new ShowCardFalseData() { Card = card});
        }

        public Card ExternChooseBetween(Player player, List<Card> cardList)
        {
            if (UnityGame.IsLocalPlayer(player))
            {
                _networkedGame.WantLocal<ChooseBetweenCardsCommand>(new ChooseCardTargetData() {CardList =  cardList});
            }

            var command = _networkedGame.WaitForExternalCommand<ChooseBetweenCardsCommand>();

            var card = _networkedGame.ResolveCard(command.CardId);

            Debug.Log("Received card");

            return card;
        }

        public void ExternGameEnded(Player winner)
        {
            _networkedGame.WantLocal<GameEndedFalseCommand>(new GameEndedFalseData(){Winner = winner});
        }

        public bool ExternChainOpportunity(Player player)
        {
            //en gros
            // meme principe qu'en haut pour un ChainOpportunityAnswerCommnd
            // si il dit non on arette la
            // si il dit oui par contre...
            
            
            // dans CETTE FONCTION il faut boucler comme en haut mais pour attendre le paquet PlayCardCommand
            // ensuite c'est dans CETTE FONCTIOn qu'il est appliqué 
            // ensuite on finit la fonction
            //todo
            return false;
            if (UnityGame.IsLocalPlayer(player))
            {
                _networkedGame.WantLocal<ChainOpportunityCommand>();
            }

            var chainCommand = _networkedGame.WaitForExternalCommand<ChainOpportunityCommand>();

            var wantChain = chainCommand.chain;
            
            Debug.Log("Received chain answer");

            if (!wantChain)
            {
                return false;
            }
            
            // la en fait faut attendre pour l'un ou l'autre
            // soit jouer une carte soit fini tour
            // si la carte a un indice négatif on a rien fait
            
            if (UnityGame.IsLocalPlayer(player))
            {
                _networkedGame.WantLocal<ChainTurnCommand>();
            }

            var command = _networkedGame.WaitForExternalCommand<ChainTurnCommand>();

            Card played = _networkedGame.ResolveCard(command.CardID);
            if (played == null)
            {
                //on a annulé
                return false;
            }
            else
            {
                // la il faut déclencher l'effet de la carte

                _networkedGame.Game.PlayCard(_networkedGame.Game.AllowedToPlayPlayer, played, false);
                
                return true;
            }
            // je crois que ca devrais marcher ?
        }

        public void DebugPrint(string component, string source, string debugPrint)
        {
            Debug.Log($"[ENG][{component}]${source}: {debugPrint}");
        }

        public int GetExternalRandomNumber(int a, int b)
        {
            // on utilise le seed préchargé
            // je mettrai l'algo stylé plus tard (ou jamais)
            return _random.Next(a, b);
        }
    }
}