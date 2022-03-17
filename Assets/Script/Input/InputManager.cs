using System;
using System.Collections.Generic;
using System.Linq;
using CardGameEngine.EventSystem;
using CardGameEngine.EventSystem.Events.GameStateEvents;
using Script.Networking;
using Script.Networking.Commands;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Script.Input
{
    public class InputManager : MonoBehaviour, IEventSubscriber
    {
        public ContactFilter2D cardFilter;

        public PlayerActions PlayerActions;

        private PlayerActions.IMainActions _myTurnInputManager;
        private PlayerActions.ITargetingActions _targetingActions;
        public volatile bool playFinished;

        private void Awake()
        {
            _myTurnInputManager = GetComponent<PlayerActions.IMainActions>();
            _targetingActions = GetComponent<PlayerActions.ITargetingActions>();
            PlayerActions = new PlayerActions();
            PlayerActions.Main.SetCallbacks(_myTurnInputManager);
            PlayerActions.Targeting.SetCallbacks(_targetingActions);
            PlayerActions.Disable();
        }

        private void Update()
        {
            if (playFinished)
            {
                playFinished = false;
                if (_lastInputType != null)
                {
                    EnableThis(_lastInputType.Value);
                }
                
            }
        }


        private void OnDisable()
        {
            PlayerActions.Disable();
        }

        public void Subscribe(SyncEventWrapper eventManager)
        {
            eventManager.SubscribeToEvent<StartTurnEvent>(OnStartTurn, false, true);
            eventManager.SubscribeToEvent<EndTurnEvent>(OnEndTurn, false, true);
        }

        private void OnEndTurn(EndTurnEvent evt)
        {
            DisableAll();
            _lastInputType = null;
        }
        

        private void OnStartTurn(StartTurnEvent evt)
        {
            if (Equals(evt.Player, UnityGame.LocalGamePlayer))
            {
                EnableThis(InputType.Main);
            }
        }

        public void DisableAll()
        {
            Debug.Log($"Disabling all, current is {_currentInputType}");
            _lastInputType = _currentInputType;
            _currentInputType = null;
            PlayerActions.Disable();
        }

        public enum InputType
        {
            Main,
            Targeting,
            UI
        }

        private InputType? _lastInputType;
        private InputType? _currentInputType;

        public void EnableThis(InputType actions)
        {
            Debug.Log($"Enabling input {actions}");
            _lastInputType = _currentInputType;
            _currentInputType = actions;
            PlayerActions.Disable();
            switch (actions)
            {
                case InputType.Main:
                    PlayerActions.Main.Enable();
                    break;
                case InputType.Targeting:
                    PlayerActions.Targeting.Enable();
                    break;
                case InputType.UI:
                    PlayerActions.UI.Enable();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(actions), actions, null);
            }
        }
    }
}