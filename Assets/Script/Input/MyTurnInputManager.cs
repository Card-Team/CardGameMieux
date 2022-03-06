using System.Collections.Generic;
using System.Linq;
using Script.Networking;
using Script.Networking.Commands;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Script.Input
{
    public class MyTurnInputManager : PointableCardInputManager, PlayerActions.IMainActions
    {
        private MainRenderer _mainRenderer;
        private NetworkedGame _game;
        private void Start()
        {
            _game = FindObjectOfType<NetworkedGame>();
            _mainRenderer = FindObjectsOfType<MainRenderer>().First(r => r.owner == UnityGame.LocalPlayer);
        }

        public void OnCardPlay(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (HoveredCard != null)
                {
                    if (HoveredCard.PreconditionJouable && HoveredCard.AssezDePa)
                    {
                        HoveredCard.Hover = false;
                        var carte = HoveredCard;
                        HoveredCard = null;
                        _game.DoLocalAction(
                            new PlayCardCommand((int)UnityGame.LocalPlayer, carte.Card.Id, false));
                        
                    }
                    else
                    {
                        Debug.Log(
                            "tentative de jouage d'une carte qui peut pas etre jouée actuellement (précondition fausse)");
                    }
                }
            }
        }

        public void OnCardUpgrade(InputAction.CallbackContext context)
        {
            //upgrade de carte
            if (context.performed)
            {
                if (HoveredCard != null)
                {
                    if (HoveredCard.Card.CurrentLevel.Value < HoveredCard.Card.MaxLevel && HoveredCard.AssezDePa)
                    {
                        HoveredCard.Hover = false;
                        var card = HoveredCard;
                        HoveredCard = null;
                        _game.DoLocalAction(
                            new PlayCardCommand((int)UnityGame.LocalPlayer, card.Card.Id, true));
                        
                    }
                    else
                    {
                        Debug.Log("on tente d'upgrade une carte max");
                    }
                }
            }
        }

        public void OnEndTurn(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed)
            {
                _game.DoLocalAction(new EndTurnCommand());
            }
        }

        public override bool IsPointable(CardRenderer r)
        {
            return _mainRenderer.cards.Contains(r);
        }
    }
}