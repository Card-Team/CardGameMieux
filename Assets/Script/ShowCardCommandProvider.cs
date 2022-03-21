using System;
using System.Collections.Generic;
using System.Linq;
using Script.Networking;
using Script.Networking.Commands.Extern;
using UnityEngine;

namespace Script
{
    public class ShowCardCommandProvider : CommandProviderBehaviour
    {
        private UnityGame _unityGame;
        private CardPickerDisplay _cardPicker;
        public CardRenderer cardRendererPrefab;

        private void Start()
        {
            _unityGame = FindObjectOfType<UnityGame>();
            NetworkedGame.RegisterCommandProvider<ShowCardFalseCommand>(this);
            _cardPicker = FindObjectOfType<CardPickerDisplay>();
        }

        private bool _wasCachee = false;

        private CardRenderer virtualCard = null;

        protected override void DoAction()
        {
            var card = ((ShowCardFalseData)InfoStruct).Card;
            CardRenderer cardRenderer;
            if (!_unityGame.CardRenderers.ContainsKey(card))
            {
                var crd = Instantiate(cardRendererPrefab);
                crd.transform.localScale = UnityGame.CardScale;
                crd.transform.localPosition = new Vector3(0, 30);
                crd.Card = card;
                virtualCard = crd;
                cardRenderer = crd;
            }
            else
            {
                cardRenderer = _unityGame.CardRenderers[card];
            }

            Debug.Log("DisplayPicker");
            _wasCachee = cardRenderer.faceCachee;
            if (cardRenderer.faceCachee)
            {
                cardRenderer.Flip();
            }

            _cardPicker.DisplayPicker(
                new[] { cardRenderer }
                    .Select(_cardPicker.WithLocation)
                    .ToList()
                ,
                "Révélation",
                OnPick
            );
        }

        private void OnPick(CardRenderer obj)
        {
            if (_wasCachee)
            {
                obj.Flip();
            }

            if (virtualCard != null)
            {
                StartCoroutine(CardChooseBetweenCommandProvider.DestroyLater(new List<CardRenderer>() { virtualCard }));
                Destroy(virtualCard.gameObject);
                virtualCard = null;
            }

            NetworkedGame.DoLocalAction(new ShowCardFalseCommand());
        }
    }
}