using UnityEngine.InputSystem;

namespace Script.Input
{
    public class TargetingInputManager : PointableCardInputManager, PlayerActions.ITargetingActions
    {
        private CardTargetCP _cardTargetCp;

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
    }
}