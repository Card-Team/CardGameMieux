using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using CardGameEngine;
using CardGameEngine.Cards;
using CardGameEngine.Cards.CardPiles;
using CardGameEngine.EventSystem.Events.CardEvents;
using CardGameEngine.GameSystems;
using ParrelSync;
using UnityEngine;

namespace Script.Networking
{
    public class UnityGame : MonoBehaviour
    {
        [SerializeField] private CardRenderer cardRenderer;
        public Dictionary<Card, CardRenderer> CardRenderers;
        public Dictionary<CardPile, PileRenderer> PileRenderers;

        public GameObject j1;
        public GameObject j2;

        public static Owner LocalPlayer { get; private set; }
        public static Player LocalGamePlayer { get; private set; }

        private Queue<Func<IEnumerator>> _player1AnimQueue = new Queue<Func<IEnumerator>>();
        private Queue<Func<IEnumerator>> _player2AnimQueue = new Queue<Func<IEnumerator>>();
        private NetworkConfiguration _nc;
        private NetworkedGame _network;


        private void Awake()
        {
            _syncEventsManager = FindObjectOfType<SyncEventsManager>();
            var elem = FindObjectOfType<GameSettingsContainer>();
            if (elem != null)
            {
                _nc = new NetworkConfiguration()
                {
                    NetworkMode = elem.NetworkMode,
                    IPAddress = elem.IPAddress,
                    Port = elem.port,
                };
                _ownDeck = new List<string>(elem.playerDeck);
            }
            else
            {
                _nc = new NetworkConfiguration()
                {   
                    NetworkMode =
#if UNITY_EDITOR
                        ClonesManager.IsClone() ? NetworkMode.Client : NetworkMode.Server,
#else
                    NetworkMode.Client,
#endif
                    IPAddress = IPAddress.IPv6Loopback,
                    Port = 1246
                };
                _ownDeck = new List<string> { "pistolet", "carteblanche" };
            }

            if (_nc.NetworkMode == NetworkMode.Client)
            {
                LocalPlayer = Owner.Player2;
                j1.transform.localRotation = Quaternion.Euler(0, 0, 180);
                j2.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                LocalPlayer = Owner.Player1;
            }
        }

        // Start is called before the first frame update
        void OnEnable()
        {
            _network = FindObjectOfType<NetworkedGame>();


            _network.EventRegistration = GameInit;
            _network.SetUpNetworkGame(_nc, "Raoult", _ownDeck);
        }

        private void GameInit(Game game)
        {
            LocalGamePlayer = LocalPlayer == Owner.Player1 ? game.Player1 : game.Player2;
            CardRenderers = new Dictionary<Card, CardRenderer>();
            PileRenderers = new Dictionary<CardPile, PileRenderer>();

            foreach (var card in game.Player1.Cards.Concat(game.Player2.Cards))
            {
                var cardObjet = Instantiate(cardRenderer);
                cardObjet.transform.localScale = new Vector3(0.40f, 0.40f, 1);
                cardObjet.Card = card;
                cardObjet.gameObject.SetActive(false);
                CardRenderers.Add(card, cardObjet);
            }

            foreach (var pileRenderer in FindObjectsOfType<PileRenderer>())
            {
                pileRenderer.GrabPile(game);
            }

            var evtMan = FindObjectOfType<SyncEventsManager>();
            evtMan.EventManager = game.EventManager;

            foreach (var eventSubscriber in FindObjectsOfType<MonoBehaviour>())
            {
                if (eventSubscriber is IEventSubscriber evt)
                {
                    evt.Subscribe(evtMan.SyncEventWrapper);
                }
            }

            // game.EventManager.SubscribeToEvent<CardPlayEvent>(e => Debug.Log($"Card played : {e.Card.Name}"));
        }

        private SyncEventsManager _syncEventsManager;
        private List<string> _ownDeck;

        public T RunOnGameThread<T>(Func<Game, T> func)
        {
            var res = _network.RunOnGameThread(func);
            return res;
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

        public static bool IsLocalPlayer(Player player)
        {
            return Equals(UnityGame.LocalGamePlayer, player);
        }
    }
}