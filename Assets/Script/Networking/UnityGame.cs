using System.Collections.Generic;
using System.Linq;
using CardGameEngine;
using CardGameEngine.Cards;
using TMPro;
using UnityEngine;

namespace Script.Networking
{
    public class UnityGame : MonoBehaviour
    {
        [SerializeField] private CardRenderer cardRenderer;
        public Dictionary<Card, CardRenderer> CardRenderers;


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

    }
}