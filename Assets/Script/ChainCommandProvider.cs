using System.Collections.Generic;
using System.Linq;
using CardGameEngine.Cards;
using CardGameEngine.EventSystem.Events.GameStateEvents;
using Script.Networking;
using Script.Networking.Commands.Extern;
using TMPro;
using UnityEngine;

namespace Script
{
    public class ChainCommandProvider : CommandProviderBehaviour, IEventSubscriber
    {
        private UnityGame _unityGame;
        private CardPickerDisplay _cardPicker;
        private MainRenderer _localMainRenderer;
        public TextMeshProUGUI chainWaitText;

        private void Start()
        {
            _unityGame = FindObjectOfType<UnityGame>();
            NetworkedGame.RegisterCommandProvider<ChainTurnCommand>(this);
            _cardPicker = FindObjectOfType<CardPickerDisplay>();
            _localMainRenderer = FindObjectsOfType<MainRenderer>().First(r => r.owner == UnityGame.LocalPlayer);
        }

        protected override void DoAction()
        {
            _localMainRenderer.UpdatePlayable();
            var info = (ChainInfo)InfoStruct;
            chainWaitText.gameObject.SetActive(!info.isLocalChaining);
            if (!info.isLocalChaining)
            {
                return;
            }

            var available = UnityGame.LocalGamePlayer.Hand
                .Where(c => c.ChainMode.Value == ChainMode.MiddleChain
                            || c.ChainMode.Value == ChainMode.StartOrMiddleChain
                            || c.ChainMode.Value == ChainMode.EndChain)
                .Select(c => _unityGame.CardRenderers[c])
                .Where(cr => cr.AssezDePa && cr.PreconditionJouable)
                .ToList();

            if (available.Count == 0)
            {
                // peut pas chainer
                OnCancel();
                return;
            }

            _cardPicker
                .DisplayPicker(available,
                    "Chainage",
                    OnPick,
                    OnCancel);
        }

        private void OnCancel()
        {
            Debug.Log("On peut pas chainer");
            chainWaitText.gameObject.SetActive(false);
            NetworkedGame.DoLocalAction(new ChainTurnCommand() { CardId = -1 });
        }

        private void OnPick(CardRenderer obj)
        {
            chainWaitText.gameObject.SetActive(false);
            NetworkedGame.DoLocalAction(new ChainTurnCommand() { CardId = obj.Card.Id });
        }

        public void Subscribe(SyncEventWrapper eventManager)
        {
            eventManager.SubscribeToEvent<ChainingEvent>(OnChainEnd, postEvent: true);
        }

        private void OnChainEnd(ChainingEvent evt)
        {
            chainWaitText.gameObject.SetActive(false);
        }
    }
}