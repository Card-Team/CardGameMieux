using UnityEngine;

namespace Script
{
    public class MainRenderer : PileRenderer
    {
        private Vector2 GetNewCardDestination()
        {
            //TODO Coordonnées locales, décaler à droite un peu
            return Vector2.zero;
        }

        protected void OnCardArrived(CardRenderer cardRenderer)
        {
            //TODO décale toute la pile à gauche
        }
    }
}