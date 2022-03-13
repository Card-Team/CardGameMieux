using System.Collections;
using System.Collections.Generic;
using CardGameEngine.EventSystem;
using CardGameEngine.EventSystem.Events.GameStateEvents;
using Script.Networking;
using UnityEngine;

public class TourArrow : MonoBehaviour, IEventSubscriber
{
    private SpriteRenderer _spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Subscribe(SyncEventWrapper eventManager)
    {
        eventManager.SubscribeToEvent<StartTurnEvent>(OnStartTurn, false, true);
        _spriteRenderer.enabled = true;
        if (UnityGame.LocalPlayer == Owner.Player2)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
    }

    private void OnStartTurn(StartTurnEvent evt)
    {
        int angle = 90;
        if (Equals(evt.Player, UnityGame.LocalGamePlayer))
        {
            angle *= -1;
        }

        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}