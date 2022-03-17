using System.Linq;
using CardGameEngine.Cards.CardPiles;
using CardGameEngine.EventSystem.Events.CardEvents;
using Script.Networking;
using UnityEngine;

namespace Script
{
    public class DiscardPileRenderer : PileRenderer
    {
        public Transform upgradeLocation;

        private DiscardPile DPile => (DiscardPile)CardPile;

        protected override Vector2 GetNewCardDestination(CardRenderer cardRenderer)
        {
            return base.GetNewCardDestination(cardRenderer);
        }


        public override void Subscribe(SyncEventWrapper eventManager)
        {
            base.Subscribe(eventManager);
            eventManager.SubscribeToEvent<CardMarkUpgradeEvent>(OnCardMarkUpgrade, false, true);
        }

        private void OnCardMarkUpgrade(CardMarkUpgradeEvent evt)
        {
            var cardRenderer = this.cards.FirstOrDefault(cr => cr.Card == evt.Card);
            if (cardRenderer != null)
            {
                var pos = upgradeLocation.transform.localPosition;
                pos.z = cardRenderer.transform.localPosition.z;
                UnityGame.AddToQueue(
                    () => MoveCardInTime(cardRenderer, pos, 0.1f, c => { })
                    , owner);
            }
        }
    }
}