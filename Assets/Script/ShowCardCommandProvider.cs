using System;
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

        private void Start()
        {
            _unityGame = FindObjectOfType<UnityGame>();
            NetworkedGame.RegisterCommandProvider<ShowCardFalseCommand>(this);
            _cardPicker = FindObjectOfType<CardPickerDisplay>();
        }

        private bool _wasCachee = false;

        protected override void DoAction()
        {
            var card = ((ShowCardFalseData)InfoStruct).Card;
            var cardRenderer = _unityGame.CardRenderers[card];
            Debug.Log("DisplayPicker");
            _wasCachee = cardRenderer.faceCachee;
            if (cardRenderer.faceCachee)
            {
                cardRenderer.Flip();
            }
            _cardPicker.DisplayPicker(
            new []{cardRenderer}
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
            NetworkedGame.DoLocalAction(new ShowCardFalseCommand());
        }
    }
}