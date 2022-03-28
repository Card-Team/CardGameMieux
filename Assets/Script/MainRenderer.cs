using System.Collections;
using System.Linq;
using CardGameEngine.EventSystem.Events.CardEvents;
using Script.Networking;
using UnityEngine;

namespace Script
{
    public class MainRenderer : PileRenderer
    {
        private const float Margin = 0.1f;

        protected void Start()
        {
            base.Awake();
            cartesCachées = owner != UnityGame.LocalPlayer;
        }

        protected override Vector2 GetNewCardDestination(CardRenderer cardRenderer)
        {
            var x = (cardRenderer.Width + Margin) * cards.Count - (cardRenderer.Width + Margin) * cards.Count / 2;

            return new Vector2(x, 0);
        }

        protected override void OnCardMovePile(CardMovePileEvent e)
        {
            base.OnCardMovePile(e);
            if (e.SourcePile != CardPile) return;
            UnityGame.CardRenderers[e.Card].HoverHeight = false;
            if (cards.Count > 0) StartCoroutine(ReorganiseCards(cards[0]));
        }

        protected override void OnCardArrived(CardRenderer cardRenderer)
        {
            base.OnCardArrived(cardRenderer);
            cardRenderer.HoverHeight = true;
            UpdatePlayable();

            if (cards.Count < 2) return;

            UnityGame.AddToQueue(() => ReorganiseCards(cardRenderer), owner);
        }

        private IEnumerator ReorganiseCards(CardRenderer cardRenderer)
        {
            const float time = 0.1f;
            var elapsed = 0.0f;

            var oldPositions = cards.Select(c => c.transform.localPosition).ToList();
            var theCrds = cards.ToList();

            while (elapsed < time)
            {
                for (var i = 0; i < oldPositions.Count; i++)
                {
                    var act = theCrds[i];
                    var destVec =
                        new Vector2(
                            (cardRenderer.Width + Margin) * i -
                            (cardRenderer.Width + Margin) * (oldPositions.Count - 1) / 2, 0f);

                    act.transform.localPosition = Vector2.Lerp(oldPositions[i],
                        destVec, elapsed / time);
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            for (var i = 0; i < oldPositions.Count; i++)
            {
                var act = theCrds[i];
                var destVec =
                    new Vector2(
                        (cardRenderer.Width + Margin) * i -
                        (cardRenderer.Width + Margin) * (oldPositions.Count - 1) / 2, 0f);

                act.transform.localPosition = destVec;
            }
        }

        public void UpdatePlayable()
        {
            // Debug.Log("Updated hand");
            foreach (var cr in cards)
            {
                cr.RefreshPrecondition();
                cr.HoverHeight = true;
            }
        }
    }
}