using System;
using System.Collections.Generic;
using System.Linq;
using CardGameEngine.EventSystem;
using CardGameEngine.EventSystem.Events.CardEvents;
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
        private void Awake()
        {
            _myTurnInputManager = GetComponent<PlayerActions.IMainActions>();
            _playerActions = new PlayerActions();
            _playerActions.Main.SetCallbacks(_myTurnInputManager);
            _playerActions.Main.Disable();
            //todo _playerActions.Target.Disable();
        }

        private void OnEnable()
        {
            _playerActions.Enable();
        }

        private void OnDisable()
        {
            _playerActions.Disable();
        }

        public void Subscribe(EventManager eventManager)
        {
            eventManager.SubscribeToEvent<StartTurnEvent>(OnStartTurn,false,true);
            eventManager.SubscribeToEvent<EndTurnEvent>(OnEndTurn,false,true);
            eventManager.SubscribeToEvent<TargetingEvent>(OnTarget,false,false);
            eventManager.SubscribeToEvent<TargetingEvent>(OnAfterTarget,false,true);
        }

        private void OnAfterTarget(TargetingEvent evt)
        {
            //todo _playerActions.Target.Disable();
        }

        private void OnTarget(TargetingEvent evt)
        {
            //todo _playerActions.Target.Enable();
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