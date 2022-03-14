using System;
using System.Linq;
using Script.Networking;
using Script.Networking.Commands.Extern;
using UnityEngine;

namespace Script
{
    public class CardTargetCommandProvider : CommandProviderBehaviour
    {
        private CardPickerDisplay _cardPickerDisplay;
        private UnityGame _unityGame;
        private void Start()
        {
            NetworkedGame.RegisterCommandProvider<ChooseCardTargetCommand>(this);
            _unityGame = FindObjectOfType<UnityGame>();
            _cardPickerDisplay = FindObjectOfType<CardPickerDisplay>();
        }

        protected override void DoAction()
        {
            Debug.Log("DoAction");
            var infoStruct = (ChooseCardTargetData) InfoStruct;
            _cardPickerDisplay.DisplayPicker(
                infoStruct.CardList.Select(c => WithLocation(_unityGame.CardRenderers[c])).ToList(),
                infoStruct.TargetName,
                OnPick
            );
        }

        private void OnPick(CardRenderer cardRenderer)
        {
            var chooseCardTargetCommand = new ChooseCardTargetCommand { CardId = cardRenderer.Card.Id };
            NetworkedGame.DoLocalAction(chooseCardTargetCommand);
        }

        private (CardRenderer, string) WithLocation(CardRenderer cardRenderer)
        {
            var pile = _unityGame.PileRenderers.Select(p => p.Value)
                .FirstOrDefault(f => f.cards.Contains(cardRenderer));

            if (pile == null) return (cardRenderer, "Inconnu");
            string texte = pile.pileType switch
            {
                PileType.Deck => "Deck",
                PileType.Discard => "Défausse",
                PileType.Hand => "Main",
                _ => throw new ArgumentOutOfRangeException()
            };
            texte += $"[{pile.cards.IndexOf(cardRenderer)}] (" + (UnityGame.LocalPlayer == pile.owner ? "Moi" : "Adv") +
                     ")";

            return (cardRenderer, texte);
        }
    }
}