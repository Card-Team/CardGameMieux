using System;
using System.Collections;
using System.Collections.Generic;
using CardGameEngine.EventSystem;
using CardGameEngine.EventSystem.Events;
using CardGameEngine.GameSystems;
using Script.Networking;
using TMPro;
using UnityEngine;

public class ActionPointManager : MonoBehaviour, IEventSubscriber
{
    public bool forLocal;

    private Player _player;

    private int _curPoints;
    private int _maxPoints;
    private TextMeshPro _texte;

    private void Awake()
    {
        Debug.Log("oui");
        _texte = GetComponent<TextMeshPro>();
    }
    

    private void RefreshText()
    {
        _texte.text = $"PA : {_curPoints}/{_maxPoints}";
    }

    public void Subscribe(EventManager eventManager)
    {
        
        eventManager.SubscribeToEvent<ActionPointsEditEvent>(OnPAChange, false, true);
        eventManager.SubscribeToEvent<MaxActionPointsEditEvent>(OnMaxPaChange, false, true);
        
        _player = forLocal ? UnityGame.LocalGamePlayer : UnityGame.LocalGamePlayer.OtherPlayer;
        _curPoints = _player.ActionPoints.Value;
        _maxPoints = _player.MaxActionPoints.Value;
        RefreshText();
    }

    public void OnPAChange(ActionPointsEditEvent editEvent)
    {
        if (!Equals(editEvent.Player, _player)) return;
        _curPoints = editEvent.NewPointCount;
        RefreshText();
    }

    public void OnMaxPaChange(MaxActionPointsEditEvent maxActionPointsEditEvent)
    {
        if (!Equals(maxActionPointsEditEvent.Player, _player)) return;

        _maxPoints = maxActionPointsEditEvent.NewMaxPointCount;
        RefreshText();
    }
}