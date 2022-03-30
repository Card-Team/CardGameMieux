using CardGameEngine.EventSystem.Events.GameStateEvents;
using Script.Networking;
using UnityEngine;
using UnityEngine.UI;

public class TourArrow : MonoBehaviour, IEventSubscriber
{
    private Animation _animation;
    public Button endButton;

    // Start is called before the first frame update
    private void Start()
    {
        _animation = GetComponent<Animation>();
    }


    public void Subscribe(SyncEventWrapper eventManager)
    {
        eventManager.SubscribeToEvent<StartTurnEvent>(OnStartTurn, false, true);
        eventManager.SubscribeToEvent<EndTurnEvent>(OnEndTurn, false, true);
    }

    private void OnEndTurn(EndTurnEvent evt)
    {
        if (Equals(evt.Player, UnityGame.LocalGamePlayer))
        {
            _animation.Stop();
            endButton.gameObject.SetActive(false);
        }
    }

    private void OnStartTurn(StartTurnEvent evt)
    {
        if (Equals(evt.Player, UnityGame.LocalGamePlayer))
        {
            _animation.Play();
            endButton.gameObject.SetActive(true);
            
        }
    }
}