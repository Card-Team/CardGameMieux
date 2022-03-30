using System;
using System.Collections;
using System.Collections.Generic;
using CardGameEngine;
using CardGameEngine.Cards;
using CardGameEngine.Cards.CardPiles;
using CardGameEngine.EventSystem.Events.CardEvents;
using Script;
using Script.Networking;
using TMPro;
using UnityEngine;

public class PileRenderer : MonoBehaviour, IEventSubscriber
{
    public Owner owner;
    public PileType pileType;
    public TextMeshPro countText;

    public bool cartesCachées = true;

    public List<CardRenderer> cards = new List<CardRenderer>();
    protected CardPile CardPile;
    protected UnityGame UnityGame;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        UnityGame = FindObjectOfType<UnityGame>();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public virtual void Subscribe(SyncEventWrapper eventManager)
    {
        eventManager.SubscribeToEvent<CardMovePileEvent>(OnCardMovePile, false, true);
    }

    public void GrabPile(Game game)
    {
        var player = owner switch
        {
            Owner.Player1 => game.Player1,
            Owner.Player2 => game.Player2,
            _ => throw new ArgumentOutOfRangeException()
        };

        CardPile = pileType switch
        {
            PileType.Hand => player.Hand,
            PileType.Deck => player.Deck,
            PileType.Discard => player.Discard,
            _ => throw new ArgumentOutOfRangeException()
        };

        UnityGame.PileRenderers.Add(CardPile, this);
        if (UnityGame.LocalPlayer != owner) countText.transform.localRotation = Quaternion.Euler(0, 0, 180);
        RefreshPile();
    }

    protected virtual void OnCardMovePile(CardMovePileEvent evt)
    {
        if (evt.SourcePile != CardPile) return;

        var cardRenderer = UnityGame.CardRenderers[evt.Card];
        cards.Remove(cardRenderer);
        UnityGame.PileRenderers[evt.DestPile].GrabCard(cardRenderer,evt.DestIndex);
        UpdateCount();
    }

    private void GrabCard(CardRenderer cardRenderer, int destIndex)
    {
        var cardTransform = cardRenderer.transform;
        cardTransform.parent = transform;
        var destination = GetNewCardDestination(cardRenderer);
        Debug.Log($"moving to z : {destination}");
        cards.Insert(destIndex,cardRenderer);

        UnityGame.AddToQueue(() => MoveCardAnimation(cardRenderer, destination), owner);
    }

    private IEnumerator MoveCardAnimation(CardRenderer cardRenderer, Vector3 destination)
    {
        yield return MoveCardInTime(cardRenderer, destination, 0.2f, OnCardArrived);

        ;
    }

    public static IEnumerator MoveCardInTime(CardRenderer cardRenderer, Vector3 destination, float time
        , Action<CardRenderer> onFinish)
    {
        cardRenderer.Hover = false;
        var start = cardRenderer.transform.localPosition;

        float tempsParcouru = 0;

        while (tempsParcouru < time)
        {
            cardRenderer.transform.localPosition = Vector3.Lerp(start, destination, tempsParcouru / time);
            tempsParcouru += Time.deltaTime;
            yield return null;
        }

        cardRenderer.transform.localPosition = destination;
        Debug.Log($"finishmove : {cardRenderer.transform.position}");
        onFinish(cardRenderer);
    }

    protected virtual Vector3 GetNewCardDestination(CardRenderer cardRenderer)
    {
        return new Vector3(0,0,-(CardPile.Count + 1));
    }

    protected virtual void OnCardArrived(CardRenderer cardRenderer)
    {
        if (cartesCachées != cardRenderer.faceCachee) cardRenderer.Flip();
        cardRenderer.RefreshPrecondition(true);
        UpdateCount();
        cardRenderer.transform.SetSiblingIndex(CardPile.IndexOf(cardRenderer.Card));
    }

    protected virtual void UpdateCount()
    {
        countText.text = cards.Count.ToString();
    }

    private void RefreshPile()
    {
        foreach (Card card in CardPile)
        {
            var unityCard = UnityGame.CardRenderers[card];
            var unityCardTransform = unityCard.transform;
            unityCardTransform.parent = transform;
            var localPosition = new Vector3(0,0,0);
            Debug.Log($"initial z {localPosition}");
            unityCardTransform.localPosition = localPosition;
            unityCard.gameObject.SetActive(true);
            if (cartesCachées != unityCard.faceCachee) unityCard.Flip();
            unityCard.transform.SetSiblingIndex(CardPile.IndexOf(unityCard.Card));
            cards.Add(unityCard);
        }

        UpdateCount();
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