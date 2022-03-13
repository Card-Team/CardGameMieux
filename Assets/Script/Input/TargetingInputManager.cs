using UnityEngine.InputSystem;

namespace Script.Input
{
    public class TargetingInputManager : PointableCardInputManager, PlayerActions.ITargetingActions
    {
        private CardTargetCP _cardTargetCp;

        public float scrollSensitivity = 1;
        

        private void Start()
        {
            _cardTargetCp = FindObjectOfType<CardTargetCP>();
        }

        public override bool IsPointable(CardRenderer r)
        {
            return _cardTargetCp.Pickable.Contains(r);
        }

        public void OnPickCard(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (HoveredCard != null)
                {
                    HoveredCard.Hover = false;
                    var card = HoveredCard;
                    HoveredCard = null;
                    _cardTargetCp.OnSelected(card);
                }
            }
        }

       

        public void OnScrollTargets(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                var axis = context.ReadValue<float>();
                _cardTargetCp.Scroll(axis * scrollSensitivity);
            }
        }
    }
}