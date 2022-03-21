using System.Collections;
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
            eventManager.SubscribeToEvent<CardUnMarkUpgradeEvent>(OnCardUnMarkUpgrade, false, true);
        }

        private void OnCardMarkUpgrade(CardMarkUpgradeEvent evt)
        {
            var cardRenderer = this.cards.FirstOrDefault(cr => cr.Card == evt.Card);
            if (cardRenderer != null)
            {
                var pos = upgradeLocation.transform.localPosition;
                pos.z = cardRenderer.transform.localPosition.z;

                IEnumerator Action()
                {
                    yield return new WaitForSeconds(0.2f);
                    yield return MoveCardInTime(cardRenderer, pos, 0.2f, c => { });
                }

                UnityGame.AddToQueue(
                    Action, owner);
            }
        }
        
        private void OnCardUnMarkUpgrade(CardUnMarkUpgradeEvent evt)
        {
            var cardRenderer = this.cards.FirstOrDefault(cr => cr.Card == evt.Card);
            if (cardRenderer != null)
            {
                Vector3 pos = GetNewCardDestination(cardRenderer);
                pos.z = cardRenderer.transform.localPosition.z;

                IEnumerator Action()
                {
                    yield return new WaitForSeconds(0.2f);
                    yield return MoveCardInTime(cardRenderer, pos, 0.2f, c => { });
                }

                UnityGame.AddToQueue(
                    Action, owner);
            }
        }
    }
}