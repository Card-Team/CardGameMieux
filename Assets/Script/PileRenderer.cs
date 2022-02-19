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

        RefreshPile();
        game.EventManager.SubscribeToEvent<CardMovePileEvent>(OnCardMovePile, false, true);
    }

    private void OnCardMovePile(CardMovePileEvent evt)
    {
        if (evt.SourcePile != _cardPile) return;
        
        //TODO bouger
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