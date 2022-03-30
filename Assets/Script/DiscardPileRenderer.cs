using System;
using System.Collections;
using System.Linq;
using CardGameEngine.Cards.CardPiles;
using CardGameEngine.EventSystem.Events.CardEvents;
using Script.Networking;
using TMPro;
using UnityEngine;

namespace Script
{
    public class DiscardPileRenderer : PileRenderer
    {
        public Transform upgradeLocation;
        public TextMeshPro upgradeCount;

        private DiscardPile DPile => (DiscardPile) CardPile;

        protected override Vector3 GetNewCardDestination(CardRenderer cardRenderer)
        {
            return base.GetNewCardDestination(cardRenderer);
        }

        private void Start()
        {
            if (UnityGame.LocalPlayer != owner) upgradeCount.transform.localRotation = Quaternion.Euler(0, 0, 180);
            upgradeCount.SetText(0.ToString());
        }

        protected override void UpdateCount()
        {
            this.countText.SetText(DPile.Count(c => !DPile.IsMarkedForUpgrade(c)).ToString());
        }


        public override void Subscribe(SyncEventWrapper eventManager)
        {
            base.Subscribe(eventManager);
            eventManager.SubscribeToEvent<CardMarkUpgradeEvent>(OnCardMarkUpgrade, false, true);
            eventManager.SubscribeToEvent<CardUnMarkUpgradeEvent>(OnCardUnMarkUpgrade, false, true);
        }

        private void OnCardMarkUpgrade(CardMarkUpgradeEvent evt)
        {
            var cardRenderer = cards.FirstOrDefault(cr => cr.Card == evt.Card);
            if (cardRenderer != null)
            {
                var pos = upgradeLocation.transform.localPosition;
                pos.z = cardRenderer.transform.localPosition.z;

                IEnumerator Action()
                {
                    yield return new WaitForSeconds(0.2f);
                    yield return MoveCardInTime(cardRenderer, pos, 0.2f, c => { });
                    upgradeCount.SetText(DPile.Count(c => DPile.IsMarkedForUpgrade(c)).ToString());
                    UpdateCount();
                }

                UnityGame.AddToQueue(
                    Action, owner);
            }
        }

        private void OnCardUnMarkUpgrade(CardUnMarkUpgradeEvent evt)
        {
            var cardRenderer = cards.FirstOrDefault(cr => cr.Card == evt.Card);
            if (cardRenderer != null)
            {
                Vector3 pos = GetNewCardDestination(cardRenderer);
                pos.z = cardRenderer.transform.localPosition.z;

                IEnumerator Action()
                {
                    yield return new WaitForSeconds(0.2f);
                    yield return MoveCardInTime(cardRenderer, pos, 0.2f, c => { });
                    upgradeCount.SetText(DPile.Count(c => DPile.IsMarkedForUpgrade(c)).ToString());
                    UpdateCount();
                }

                UnityGame.AddToQueue(
                    Action, owner);
            }
        }
    }
}