using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CardGameEngine;
using CardGameEngine.Cards;
using CardGameEngine.Cards.CardPiles;
using UnityEngine;

namespace Script.Networking
{
    public class UnityGame : MonoBehaviour
    {
        [SerializeField] private CardRenderer cardRenderer;
        public Dictionary<Card, CardRenderer> CardRenderers;
        public Dictionary<CardPile, PileRenderer> PileRenderers;

        private Queue<Func<IEnumerator>> _player1AnimQueue = new Queue<Func<IEnumerator>>();
        private Queue<Func<IEnumerator>> _player2AnimQueue = new Queue<Func<IEnumerator>>();


        // Start is called before the first frame update
        void Start()
        {
            var network = FindObjectOfType<NetworkedGame>();
            network.EventRegistration = GameInit;
            network.SetUpNetworkGame("Raoult", new List<string>() {"pistolet"});
        }

        private void GameInit(Game game)
        {
            CardRenderers = new Dictionary<Card, CardRenderer>();
            PileRenderers = new Dictionary<CardPile, PileRenderer>();

            foreach (var card in game.Player1.Cards.Concat(game.Player2.Cards))
            {
                var cardObjet = Instantiate(cardRenderer);
                cardObjet.Card = card;
                cardObjet.gameObject.SetActive(false);
                CardRenderers.Add(card, cardObjet);
            }

            foreach (var pileRenderer in FindObjectsOfType<PileRenderer>())
            {
                pileRenderer.GrabPile(game);
            }
        }

        public void AddToQueue(Func<IEnumerator> action, Owner owner)
        {
            var queue = owner switch
            {
                Owner.Player1 => _player1AnimQueue,
                Owner.Player2 => _player2AnimQueue,
                _ => throw new ArgumentOutOfRangeException()
            };

            queue.Enqueue(action);
            if (queue.Count == 1)
            {
                StartCoroutine(EmptyQueue(queue));
            }
        }

        private IEnumerator EmptyQueue(Queue<Func<IEnumerator>> queue)
        {
            while (queue.Count > 0)
            {
                var action = queue.Peek();
                yield return StartCoroutine(action());
                queue.Dequeue();
            }
        }
    }
}