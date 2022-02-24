using Script.Networking;
using UnityEngine;

namespace Script
{
    public class MainRenderer : PileRenderer
    {
        protected override Vector2 GetNewCardDestination(CardRenderer cardRenderer)
        {
            var x = this.cards.Count * (cardRenderer.Width / 2);

            return new Vector2(x, 0);
        }

        protected override void OnCardArrived(CardRenderer cardRenderer)
        {
            if (owner == UnityGame.LocalPlayer)
            {
                cardRenderer.Flip();
            }
        }
    }
}