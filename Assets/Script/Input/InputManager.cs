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

        private PlayerActions _playerActions;

        private PlayerActions.IMainActions _myTurnInputManager;
        private PlayerActions.ITargetingActions _targetingActions;

        private void Awake()
        {
            _myTurnInputManager = GetComponent<PlayerActions.IMainActions>();
            _targetingActions = GetComponent<PlayerActions.ITargetingActions>();
            _playerActions = new PlayerActions();
            _playerActions.Main.SetCallbacks(_myTurnInputManager);
            _playerActions.Targeting.SetCallbacks(_targetingActions);
            _playerActions.Main.Disable();
            _playerActions.Targeting.Disable();
        }

        private void OnEnable()
        {
            _playerActions.Enable();
        }

        private void OnDisable()
        {
            _playerActions.Disable();
        }

        public void Subscribe(SyncEventWrapper eventManager)
        {
            eventManager.SubscribeToEvent<StartTurnEvent>(OnStartTurn, false, true);
            eventManager.SubscribeToEvent<EndTurnEvent>(OnEndTurn, false, true);
        }

        private bool _wasMainEnabled = false;

        public void OnAfterTarget()
        {
            Debug.Log("Targeting ended");
            _playerActions.Targeting.Disable();
            if (_wasMainEnabled) _playerActions.Main.Enable();
        }

        public void OnTarget()
        {
            Debug.Log("Targeting started");
            _playerActions.Targeting.Enable();
            _wasMainEnabled = _playerActions.Main.enabled;
            _playerActions.Main.Disable();
        }

        private void OnEndTurn(EndTurnEvent evt)
        {
            _playerActions.Main.Disable();
        }

        public void OnStartTurn(StartTurnEvent evt)
        {
            if (Equals(evt.Player, UnityGame.LocalGamePlayer))
            {
                _playerActions.Main.Enable();
            }
        }
    }
}