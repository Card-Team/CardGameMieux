using System;
using System.Collections;
using System.Collections.Generic;
using Script.Networking;
using Script.Networking.Commands.Extern;
using UnityEngine;

namespace Script
{
    public class CardChooseBetweenCommandProvider : CommandProviderBehaviour
    {
        public CardRenderer cardRendererPrefab;

        private void Start()
        {
            NetworkedGame.RegisterCommandProvider<ChooseBetweenCardsCommand>(this);
            _cardPickerDisplay = FindObjectOfType<CardPickerDisplay>();
        }

        private List<CardRenderer> _cards = new List<CardRenderer>();
        private CardPickerDisplay _cardPickerDisplay;

        protected override void DoAction()
        {
            Debug.Log("Choose between");
            var data = (ChooseBetweenCardData)this.InfoStruct;

            foreach (var card in data.CardList)
            {
                var cardRenderer = Instantiate(cardRendererPrefab);
                cardRenderer.transform.localScale = UnityGame.CardScale;
                cardRenderer.transform.localPosition = new Vector3(0, 30);
                cardRenderer.Card = card;
                NetworkedGame.VirtualTracking[card.Id] = card;
                _cards.Add(cardRenderer);
            }

            _cardPickerDisplay.DisplayPicker(
                _cards,
                "Choissez parmis ces cartes",
                OnPick
            );
        }

        private void OnPick(CardRenderer obj)
        {
            StartCoroutine(DestroyLater(new List<CardRenderer>(_cards)));

            _cards.Clear();
            var chooseBetweenCardsCommand = new ChooseBetweenCardsCommand()
            {
                CardId = obj.Card.Id
            };
            NetworkedGame.DoLocalAction(chooseBetweenCardsCommand);
        }

        private IEnumerator DestroyLater(List<CardRenderer> cardRenderers)
        {
            yield return new WaitForSeconds(4);
            foreach (var cardRenderer in cardRenderers)
            {
                NetworkedGame.VirtualTracking.Remove(cardRenderer.Card.Id);
                Destroy(cardRenderer);
            }
        }
    }
}