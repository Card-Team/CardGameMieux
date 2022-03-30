using System;
using CardGameEngine.EventSystem.Events.GameStateEvents;
using Script.Networking;
using UnityEngine;

namespace Script.Input
{
    public class InputManager : MonoBehaviour, IEventSubscriber
    {
        public enum InputType
        {
            Main,
            Targeting,
            UI
        }

        public ContactFilter2D cardFilter;
        public volatile bool playFinished;
        private InputType? _currentInputType;

        private InputType? _lastInputType;

        private MyTurnInputManager _myTurnInputManager;
        private TargetingInputManager _targetingActions;

        public PlayerActions PlayerActions;

        private void Awake()
        {
            _myTurnInputManager = GetComponent<MyTurnInputManager>();
            _targetingActions = GetComponent<TargetingInputManager>();
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
                if (_lastInputType != null) EnableThis(_lastInputType.Value);
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
            if (Equals(evt.Player, UnityGame.LocalGamePlayer)) EnableThis(InputType.Main);
        }

        public void DisableAll()
        {
            Debug.Log($"Disabling all, current is {_currentInputType}");
            _lastInputType = _currentInputType;
            _currentInputType = null;
            _myTurnInputManager.Disable();
            _targetingActions.Disable();
            PlayerActions.Disable();
        }

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