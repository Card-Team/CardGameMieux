using System;
using System.Collections;
using System.Collections.Generic;
using CardGameEngine;
using CardGameEngine.Cards;
using CardGameEngine.Cards.CardPiles;
using CardGameEngine.EventSystem.Events.CardEvents;
using Script;
using Script.Networking;
using UnityEngine;

public class PileRenderer : MonoBehaviour
{
    public Owner owner;
    public PileType pileType;

    public List<CardRenderer> cards = new List<CardRenderer>();
    private CardPile _cardPile;
    private UnityGame _unityGame;

    // Start is called before the first frame update
    void Start()
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
        game.EventManager.SubscribeToEvent<CardMovePileEvent>(OnCardMovePile, false, true);
    }

    private void OnCardMovePile(CardMovePileEvent evt)
    {
        if (evt.SourcePile != _cardPile) return;

        var cardRenderer = _unityGame.CardRenderers[evt.Card];
        cards.Remove(cardRenderer);
        _unityGame.PileRenderers[evt.DestPile].GrabCard(cardRenderer);
    }

    private void GrabCard(CardRenderer cardRenderer)
    {
        var cardTransform = cardRenderer.transform;
        cardTransform.parent = transform;
        cards.Add(cardRenderer);

        _unityGame.AddToQueue(() => MoveCardAnimation(cardRenderer), owner);
    }

    private IEnumerator MoveCardAnimation(CardRenderer cardRenderer)
    {
        var destination = GetNewCardDestination(cardRenderer);
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

            cards.Add(unityCard);
        }
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