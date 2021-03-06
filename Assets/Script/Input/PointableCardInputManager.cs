using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Script.Input
{
    public abstract class PointableCardInputManager : MonoBehaviour
    {
        protected CardRenderer HoveredCard;
        protected InputManager InputManager;

        private void Awake()
        {
            InputManager = FindObjectOfType<InputManager>();
        }

        public void OnPointCard(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                var pos = context.ReadValue<Vector2>();
                var worldPos = Camera.main.ScreenToWorldPoint(pos);

                var resultats = new List<Collider2D>();
                var count = Physics2D.OverlapPoint(worldPos, InputManager.cardFilter, resultats);
                var newHover = resultats.Count > 0
                    ? resultats
                        .Select(c => c.GetComponentInParent<CardRenderer>())
                        .FirstOrDefault(IsPointable)
                    : null;
                if (newHover != HoveredCard)
                {
                    if (HoveredCard != null)
                    {
                        HoveredCard.Hover = false;
                        HoveredCard = null;
                    }

                    if (newHover != null)
                    {
                        newHover.Hover = true;
                        HoveredCard = newHover;
                    }
                }
            }
        }

        public abstract bool IsPointable(CardRenderer r);

        public void Disable()
        {
            if (HoveredCard != null)
            {
                HoveredCard.Hover = false;
            }
        }
    }
}