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
            var cardBag = new ConcurrentBag<Card>();
            if (_networkedGame.IsLocalPlayer(effectOwner))
            {
                _networkedGame.WantLocal<ChooseCardTargetCommand>(new ChooseCardTargetData() {TargetName = targetName,CardList =  cardList});
            }

            _networkedGame.WaitFor<ChooseCardTargetCommand>(c =>
            {
                var card = _networkedGame.ResolveCard(c.CardId);
                cardBag.Add(card);
            });

            Card card;
            while (!cardBag.TryTake(out card))
            {
                Thread.Sleep(1000);
            }
            
            Debug.Log("Received card");

            return card;
        }

        public Player ExternPlayerAskForTarget(Player effectOwner, string targetName)
        {
            var playerBag = new ConcurrentBag<Player>();
            if (_networkedGame.IsLocalPlayer(effectOwner))
            {
                _networkedGame.WantLocal<ChooseCardTargetCommand>(new ChoosePlayerTargetData() {TargetName = targetName});
            }

            _networkedGame.WaitFor<ChoosePlayerTargetCommand>(c =>
            {
                var player = _networkedGame.ResolvePlayer(c.PlayerId);
                playerBag.Add(player);
            });

            Player player;
            while (!playerBag.TryTake(out player))
            {
                Thread.Sleep(1000);
            }
            
            Debug.Log("Received player");

            return player;
        }

        public void ExternShowCard(Player player, Card card)
        {
            _networkedGame.WantLocal<ShowCardFalseCommand>( new ShowCardFalseData() { Card = card});
        }

        public Card ExternChooseBetween(Player player, List<Card> cardList)
        {
            var cardBag = new ConcurrentBag<Card>();
            if (_networkedGame.IsLocalPlayer(player))
            {
                _networkedGame.WantLocal<ChooseBetweenCardsCommand>(new ChooseCardTargetData() {CardList =  cardList});
            }

            _networkedGame.WaitFor<ChooseBetweenCardsCommand>(c =>
            {
                var card = _networkedGame.ResolveCard(c.CardId);
                cardBag.Add(card);
            });

            Card card;
            while (!cardBag.TryTake(out card))
            {
                Thread.Sleep(1000);
            }
            
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
            var boolBag = new ConcurrentBag<bool>();
            if (_networkedGame.IsLocalPlayer(player))
            {
                _networkedGame.WantLocal<ChainOpportunityCommand>();
            }

            _networkedGame.WaitFor<ChainOpportunityCommand>(c =>
            {
                var b = c.chain;
                boolBag.Add(b);
            });

            bool wantChain;
            while (!boolBag.TryTake(out wantChain))
            {
                Thread.Sleep(1000);
            }
            
            Debug.Log("Received chain answer");

            if (!wantChain)
            {
                return false;
            }
            
            // la en fait faut attendre pour l'un ou l'autre
            // soit jouer une carte soit fini tour
            // si la carte a un indice négatif on a rien fait
            
            var chainTurnBag = new ConcurrentBag<Card>();
            if (_networkedGame.IsLocalPlayer(player))
            {
                _networkedGame.WantLocal<ChainTurnCommand>();
            }

            _networkedGame.WaitFor<ChainTurnCommand>(c =>
            {
                var b = c.CardID;
                chainTurnBag.Add(_networkedGame.ResolveCard(b));
            });

            Card played;
            while (!chainTurnBag.TryTake(out played))
            {
                Thread.Sleep(1000);
            }
            
            // ici soit on a joué une carte soit annulé

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
            Debug.Log($"[ENG][{component}]${source}{debugPrint}");
        }

        public int GetExternalRandomNumber(int a, int b)
        {
            // on utilise le seed préchargé
            // je mettrai l'algo stylé plus tard
            return _random.Next(a, b);
        }
    }
}