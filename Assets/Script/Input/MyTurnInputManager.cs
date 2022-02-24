using System.Collections.Generic;
using System.Linq;
using Script.Networking;
using Script.Networking.Commands;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Script.Input
{
    public class MyTurnInputManager : MonoBehaviour,PlayerActions.IMainActions
    {
        private InputManager _inputManager;
        private MainRenderer _mainRenderer;
        private CardRenderer _hoveredCard;
        private NetworkedGame _game;
        
        private void Start()
        {
            _game = FindObjectOfType<NetworkedGame>();
            _mainRenderer = FindObjectsOfType<MainRenderer>().First(r => r.owner == UnityGame.LocalPlayer);
            _inputManager = FindObjectOfType<InputManager>();
        }

        public void OnCardPlay(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (_hoveredCard != null)
                {
                    if (_hoveredCard.preconditionJouable && _hoveredCard.assezDePA)
                    {
                        _game.DoLocalAction(
                            new PlayCardCommand((int)UnityGame.LocalPlayer, _hoveredCard.Card.Id, false));
                        _hoveredCard.Hover = false;
                        _hoveredCard = null;
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
                if (_hoveredCard != null)
                {
                    if (_hoveredCard.Card.CurrentLevel.Value < _hoveredCard.Card.MaxLevel && _hoveredCard.assezDePA)
                    {
                        _game.DoLocalAction(
                            new PlayCardCommand((int)UnityGame.LocalPlayer, _hoveredCard.Card.Id, true));
                        _hoveredCard.Hover = false;
                        _hoveredCard = null;
                    }
                    else
                    {
                        Debug.Log("on tente d'upgrade une carte max");
                    }
                }
            }
        }

        public void OnPointCard(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                var pos = context.ReadValue<Vector2>();
                var worldPos = Camera.main.ScreenToWorldPoint(pos);

                var resultats = new List<Collider2D>();
                var count = Physics2D.OverlapPoint(worldPos, _inputManager.cardFilter, resultats);
                var newHover = resultats.Count > 0
                    ? resultats
                        .Select(c => c.GetComponent<CardRenderer>())
                        .FirstOrDefault(c => _mainRenderer.cards.Contains(c))
                    : null;
                if (newHover != _hoveredCard)
                {
                    if (_hoveredCard != null)
                    {
                        _hoveredCard.Hover = false;
                        _hoveredCard = null;
                    }

                    if (newHover != null)
                    {
                        newHover.Hover = true;
                        _hoveredCard = newHover;
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
    }
}