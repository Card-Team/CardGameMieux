using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            var x = this.cards.Count * (cardRenderer.Width / 2);

            return new Vector2(x, 0);
        }

        protected override void OnCardArrived(CardRenderer cardRenderer)
        {
            base.OnCardArrived(cardRenderer);
            
            UpdatePlayable();
            
            if (cards.Count < 2) return;

            var toMove = cards.Where(c => c != cardRenderer).ToList();

            StartCoroutine(ReorganiseCards(toMove, cardRenderer));
        }

        private IEnumerator ReorganiseCards(List<CardRenderer> toMove, CardRenderer cardRenderer)
        {
            var start = 0;
            var dest = cardRenderer.Width / 2;
            float cur = 0;

            List<Vector3> oldPositions = toMove.Select(c => c.transform.localPosition).ToList();

            while (cur < 1)
            {
                for (var i = 0; i < oldPositions.Count; i++)
                {
                    var act = toMove[i];
                    act.transform.localPosition = oldPositions[i] - new Vector3(Mathf.Lerp(start, dest, cur), 0);
                }

                cur += 0.05f;
                yield return new WaitForEndOfFrame();
            }
        }

        public void UpdatePlayable()
        {
            foreach (var cr in cards)
            {
                cr.RefreshPrecondition();
            }

        }
    }
}