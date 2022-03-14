using UnityEngine.InputSystem;

namespace Script.Input
{
    public class TargetingInputManager : PointableCardInputManager, PlayerActions.ITargetingActions
    {
        private CardPickerDisplay _cardPickerDisplay;

        public float scrollSensitivity = 1;
        

        private void Start()
        {
            _cardPickerDisplay = FindObjectOfType<CardPickerDisplay>();
        }

        public override bool IsPointable(CardRenderer r)
        {
            return _cardPickerDisplay.Pickable.Contains(r);
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
                    _cardPickerDisplay.OnSelected(card);
                }
            }
        }

       

        public void OnScrollTargets(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                var axis = context.ReadValue<float>();
                _cardPickerDisplay.Scroll(axis * scrollSensitivity);
            }
        }
    }
}