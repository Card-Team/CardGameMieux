using System;
using System.Collections.Generic;
using CardGameEngine;
using CardGameEngine.EventSystem;
using CardGameEngine.EventSystem.Events;
using CardGameEngine.EventSystem.Events.CardEvents;
using CardGameEngine.EventSystem.Events.CardEvents.PropertyChange;
using CardGameEngine.EventSystem.Events.GameStateEvents;
using UnityEngine;

namespace Script.Networking
{
    public class NetworkStarter : MonoBehaviour
    {
        public CardRenderer cardRenderer;
        public PileRenderer piocheJ1;
        public PileRenderer piocheJ2;

        // Start is called before the first frame update
        void Start()
        {
            var network = FindObjectOfType<NetworkedGame>();
            network.EventRegistration = GameInit;
            network.SetUpNetworkGame("Raoult", new List<string>() {"pistolet"});
        }

        private void GameInit(Game game)
        {
            // P1
            foreach (var player1Card in game.Player1.Cards)
            {
                var card = Instantiate(cardRenderer);
                card.card = player1Card;
                card.retournee = true;
                piocheJ1.cards.Add(card);
            }

            // P2
            foreach (var player2Card in game.Player2.Cards)
            {
                var card = Instantiate(cardRenderer);
                card.card = player2Card;
                card.retournee = true;
                piocheJ2.cards.Add(card);
            }

            // RegisterAllEvents(game.EventManager);
        }

        // private void RegisterAllEvents(EventManager eventManager)
        // {
        //     // Carte monte de niveau
        //     eventManager.SubscribeToEvent<CardLevelChangeEvent>(OnCardLevelChange, postEvent: true);
        //
        //     // Carte jouée puis effet exécuté
        //     eventManager.SubscribeToEvent<CardPlayEvent>(OnCardPlay, postEvent: false);
        //     eventManager.SubscribeToEvent<CardEffectPlayEvent>(OnCardEffectPlay, postEvent: false);
        //     eventManager.SubscribeToEvent<CardEffectPlayEvent>(OnPostCardEffectPlay, postEvent: true,
        //         evenIfCancelled: true);
        //     eventManager.SubscribeToEvent<CardPlayEvent>(OnPostCardPlay, postEvent: true);
        //
        //     // Déplacement d'une carte
        //     eventManager.SubscribeToEvent<CardMovePileEvent>(OnCardChangePile, postEvent: true);
        //
        //     // Mise en amélioration d'une carte 
        //     eventManager.SubscribeToEvent<CardMarkUpgradeEvent>(OnCardMarkedUpgrade, postEvent: true);
        //
        //     eventManager.SubscribeToEvent<CardUnMarkUpgradeEvent>(OnCardRemovedMarkedUpgrade, postEvent: true);
        //
        //     // Bouclage du deck
        //     eventManager.SubscribeToEvent<DeckLoopEvent>(OnDeckLoop, postEvent: true);
        //
        //     // Nombre de point d'action
        //     eventManager.SubscribeToEvent<ActionPointsEditEvent>(OnActionPointsEdit, postEvent: true);
        //
        //     // Nombre max de points d'actions
        //     eventManager.SubscribeToEvent<MaxActionPointsEditEvent>(OnMaxActionPointsEdit, postEvent: true);
        // }
    }
}