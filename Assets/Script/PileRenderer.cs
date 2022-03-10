using System;
using System.Collections;
using System.Collections.Generic;
using CardGameEngine;
using CardGameEngine.Cards;
using CardGameEngine.Cards.CardPiles;
using CardGameEngine.EventSystem;
using CardGameEngine.EventSystem.Events.CardEvents;
using Script;
using Script.Networking;
using UnityEngine;

public class PileRenderer : MonoBehaviour, IEventSubscriber
{
    public Owner owner;
    public PileType pileType;

    public bool cartesCachées = true;

    public List<CardRenderer> cards = new List<CardRenderer>();
    private CardPile _cardPile;
    private UnityGame _unityGame;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        _unityGame = FindObjectOfType<UnityGame>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void GrabPile(Game game)
    {
        var player = this.owner switch
        {
            Owner.Player1 => game.Player1,
            Owner.Player2 => game.Player2,
            _ => throw new ArgumentOutOfRangeException()
        };

        _cardPile = pileType switch
        {
            PileType.Hand => player.Hand,
            PileType.Deck => player.Deck,
            PileType.Discard => player.Discard,
            _ => throw new ArgumentOutOfRangeException()
        };

        _unityGame.PileRenderers.Add(_cardPile, this);

        RefreshPile();
    }

    protected virtual void OnCardMovePile(CardMovePileEvent evt)
    {
        Debug.Log("CardMovePile aaaaaaa");
        if (evt.SourcePile != _cardPile) return;
        Debug.Log("CardMovePile a moi");
        
        var cardRenderer = _unityGame.CardRenderers[evt.Card];
        cardRenderer.fond.color = Color.white; //todo peut etre a enlever quand l'effet sera bougé sur un autre gameobject que le fond
        cards.Remove(cardRenderer);
        _unityGame.PileRenderers[evt.DestPile].GrabCard(cardRenderer);
    }

    private void GrabCard(CardRenderer cardRenderer)
    {
        var cardTransform = cardRenderer.transform;
        cardTransform.parent = transform;
        var destination = GetNewCardDestination(cardRenderer);
        cards.Add(cardRenderer);

        _unityGame.AddToQueue(() => MoveCardAnimation(cardRenderer,destination), owner);
    }

    private IEnumerator MoveCardAnimation(CardRenderer cardRenderer, Vector2 destination)
    {
        Debug.Log(destination);
        var start = (Vector2) cardRenderer.transform.localPosition;
        var pourcentage = 0.0f;

        while ((Vector2) cardRenderer.transform.localPosition != destination)
        {
            var newPos = Vector2.Lerp(start, destination, pourcentage);
            cardRenderer.transform.localPosition = newPos;
            pourcentage += 0.02f;
            yield return new WaitForEndOfFrame();
        }

        OnCardArrived(cardRenderer);
    }

    protected virtual Vector2 GetNewCardDestination(CardRenderer cardRenderer)
    {
        return Vector2.zero;
    }

    protected virtual void OnCardArrived(CardRenderer cardRenderer)
    {
        if (cartesCachées != cardRenderer.faceCachee)
        {
            cardRenderer.Flip();
        }
        var oldPos = cardRenderer.transform.position;
        oldPos.z = -(transform.position.z + cards.IndexOf(cardRenderer));
        cardRenderer.transform.position = oldPos;
    }

    private void RefreshPile()
    {
        foreach (Card card in _cardPile)
        {
            var unityCard = _unityGame.CardRenderers[card];
            var unityCardTransform = unityCard.transform;
            unityCardTransform.parent = transform;
            unityCardTransform.localPosition = Vector3.zero;
            unityCard.gameObject.SetActive(true);
            if (cartesCachées != unityCard.faceCachee)
            {
                unityCard.Flip();
            }
            cards.Add(unityCard);
        }
    }

    public void Subscribe(SyncEventWrapper eventManager)
    {
        
        eventManager.SubscribeToEvent<CardMovePileEvent>(OnCardMovePile, false, true);
    }
}

public enum PileType
{
    Hand,
    Deck,
    Discard
}

public enum Owner
{
    Player1,
    Player2
}