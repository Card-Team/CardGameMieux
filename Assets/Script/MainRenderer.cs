using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CardGameEngine.EventSystem.Events.CardEvents;
using Script.Networking;
using UnityEngine;

namespace Script
{
    public class MainRenderer : PileRenderer
    {
        protected void Start()
        {
            base.Awake();
            this.cartesCachées = owner != UnityGame.LocalPlayer;
        }

        protected override Vector2 GetNewCardDestination(CardRenderer cardRenderer)
        {
            var x = (cardRenderer.Width) * cards.Count  - ((cardRenderer.Width) * (cards.Count)) / 2;

            return new Vector2(x, 0);
        }

        protected override void OnCardMovePile(CardMovePileEvent e)
        {
            base.OnCardMovePile(e);
            if (cards.Count > 0)
            {
                StartCoroutine(ReorganiseCards(cards[0]));
            }
        }

        protected override void OnCardArrived(CardRenderer cardRenderer)
        {
            base.OnCardArrived(cardRenderer);

            UpdatePlayable();

            if (cards.Count < 2) return;

            StartCoroutine(ReorganiseCards(cardRenderer));
        }

        private IEnumerator ReorganiseCards(CardRenderer cardRenderer)
        {
            const float time = 0.1f;
            float elapsed = 0.0f;

            List<Vector3> oldPositions = cards.Select(c => c.transform.localPosition).ToList();
            List<CardRenderer> theCrds = cards.ToList();

            while (elapsed < time)
            {
                for (var i = 0; i < oldPositions.Count; i++)
                {
                    var act = theCrds[i];
                    Vector2 destVec =
                        new Vector2((cardRenderer.Width) * i - ((cardRenderer.Width) * (oldPositions.Count - 1)) / 2, 0f);

                    act.transform.localPosition = Vector2.Lerp(oldPositions[i],
                        destVec, (elapsed / time));
                }

                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        public void UpdatePlayable()
        {
            Debug.Log("Updated hand");
            foreach (var cr in cards)
            {
                cr.RefreshPrecondition();
            }
        }
    }
}