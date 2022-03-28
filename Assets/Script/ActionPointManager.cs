using CardGameEngine.EventSystem.Events;
using CardGameEngine.GameSystems;
using Script.Networking;
using TMPro;
using UnityEngine;

public class ActionPointManager : MonoBehaviour, IEventSubscriber
{
    public bool forLocal;

    private int _curPoints;
    private int _maxPoints;

    private Player _player;
    private TextMeshPro _texte;

    private void Awake()
    {
        _texte = GetComponent<TextMeshPro>();
    }

    public void Subscribe(SyncEventWrapper eventManager)
    {
        eventManager.SubscribeToEvent<ActionPointsEditEvent>(OnPAChange, false, true);
        eventManager.SubscribeToEvent<MaxActionPointsEditEvent>(OnMaxPaChange, false, true);

        _player = forLocal ? UnityGame.LocalGamePlayer : UnityGame.LocalGamePlayer.OtherPlayer;
        _curPoints = _player.ActionPoints.Value;
        _maxPoints = _player.MaxActionPoints.Value;
        RefreshText();
    }


    private void RefreshText()
    {
        _texte.text = $"PA : {_curPoints}/{_maxPoints}";
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