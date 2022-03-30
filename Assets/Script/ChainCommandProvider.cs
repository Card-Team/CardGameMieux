using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
        public TextMeshProUGUI chainWaitText;
        private CardPickerDisplay _cardPicker;
        private MainRenderer _localMainRenderer;
        private UnityGame _unityGame;
        public Transform chainDisplayer;
        private CardRenderer _copy;

        private void Start()
        {
            _unityGame = FindObjectOfType<UnityGame>();
            NetworkedGame.RegisterCommandProvider<ChainTurnCommand>(this);
            _cardPicker = FindObjectOfType<CardPickerDisplay>();
            _localMainRenderer = FindObjectsOfType<MainRenderer>().First(r => r.owner == UnityGame.LocalPlayer);
        }

        public void Subscribe(SyncEventWrapper eventManager)
        {
            eventManager.SubscribeToEvent<ChainingEvent>(OnChainEnd, postEvent: true);
        }

        protected override void DoAction()
        {
            if(_copy != null)
                Destroy(_copy.gameObject);
            _localMainRenderer.UpdatePlayable();
            var info = (ChainInfo)InfoStruct;
            chainWaitText.gameObject.SetActive(!info.isLocalChaining);
            if (!info.isLocalChaining) return;

            var available = UnityGame.LocalGamePlayer.Hand
                .Where(c => c.ChainMode.Value == ChainMode.MiddleChain
                            || c.ChainMode.Value == ChainMode.StartOrMiddleChain
                            || c.ChainMode.Value == ChainMode.EndChain)
                .Where(c => !_unityGame.Game.ChainStack.Contains(c))
                .Select(c => _unityGame.CardRenderers[c])
                .Where(cr => cr.AssezDePa && cr.PreconditionJouable)
                .ToList();

            if (available.Count == 0)
            {
                // peut pas chainer
                OnCancel();
                return;
            }

            var theCard = _unityGame.Game.ChainStack.Peek();
            var latest = Instantiate(_unityGame.CardRenderers[theCard]);
            latest.Card = theCard;
            latest.faceCachee = true;   
            latest.Flip();
            latest.transform.parent = chainDisplayer;
            latest.transform.localPosition = Vector3.zero;
            latest.transform.localRotation = Quaternion.identity;
            _copy = latest;
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
            NetworkedGame.DoLocalAction(new ChainTurnCommand { CardId = -1 });
            Destroy(_copy.gameObject);
        }

        private void OnPick(CardRenderer obj)
        {
            chainWaitText.gameObject.SetActive(false);
            NetworkedGame.DoLocalAction(new ChainTurnCommand { CardId = obj.Card.Id });
            Destroy(_copy.gameObject);
        }

        private void OnChainEnd(ChainingEvent evt)
        {
            chainWaitText.gameObject.SetActive(false);
        }
    }
}