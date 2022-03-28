using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CardGameEngine;
using CardGameEngine.Cards;
using CardGameEngine.Cards.CardPiles;
using CardGameEngine.GameSystems;
using UnityEngine;
#if UNITY_EDITOR
using ParrelSync;
#endif

namespace Script.Networking
{
    public class UnityGame : MonoBehaviour
    {
        public static readonly Vector3 CardScale = new Vector3(0.23f, 0.23f, 1);
        [SerializeField] private CardRenderer cardRenderer;

        public GameObject j1;
        public GameObject j2;
        public NetworkedGame Network;
        public GameObject interFaceObject;
        public GameObject connectingObject;
        public GameObject hostingObject;
        private NetworkConfiguration _nc;
        private List<string> _ownDeck;

        private readonly Queue<Func<IEnumerator>> _player1AnimQueue = new Queue<Func<IEnumerator>>();
        private readonly Queue<Func<IEnumerator>> _player2AnimQueue = new Queue<Func<IEnumerator>>();

        private SyncEventsManager _syncEventsManager;
        public Dictionary<Card, CardRenderer> CardRenderers;
        public Dictionary<CardPile, PileRenderer> PileRenderers;

        public Game Game => Network.Game;

        public static Owner LocalPlayer { get; private set; }
        public static Player LocalGamePlayer { get; private set; }


        private void Awake()
        {
            _syncEventsManager = FindObjectOfType<SyncEventsManager>();
            var elem = FindObjectOfType<GameSettingsContainer>();
            if (elem != null)
            {
                _nc = new NetworkConfiguration
                {
                    NetworkMode = elem.NetworkMode,
                    IPAddress = elem.IPAddress,
                    Port = elem.port
                };
                _ownDeck = new List<string>(elem.playerDeck);
            }
            else
            {
                _nc = new NetworkConfiguration
                {
                    NetworkMode =
#if UNITY_EDITOR
                        ClonesManager.IsClone() ? NetworkMode.Client : NetworkMode.Server,
#else
                    NetworkMode.Client,
#endif
                    IPAddress = IPAddress.Loopback,
                    Port = 1246
                };
                _ownDeck = new List<string>
                {
                    "pistolet", "pistolet", "sabotage", "sabotage", "carteblanche", "carteblanche", "echange",
                    "echange", "augmentation", "augmentation", "pioche_Critique", "pioche_Critique"
                };
            }

            if (_nc.NetworkMode == NetworkMode.Client)
            {
                LocalPlayer = Owner.Player2;
                j1.transform.localRotation = Quaternion.Euler(0, 0, 180);
                j2.transform.localRotation = Quaternion.Euler(0, 0, 0);
                connectingObject.SetActive(true);
            }
            else
            {
                LocalPlayer = Owner.Player1;
                hostingObject.SetActive(true);
            }
        }

        // Start is called before the first frame update
        private void OnEnable()
        {
            Network = FindObjectOfType<NetworkedGame>();


            Network.EventRegistration = GameInit;
            Network.SetUpNetworkGame(_nc, "Raoult", _ownDeck);
        }

        private void GameInit(Game game)
        {
            LocalGamePlayer = LocalPlayer == Owner.Player1 ? game.Player1 : game.Player2;
            CardRenderers = new Dictionary<Card, CardRenderer>();
            PileRenderers = new Dictionary<CardPile, PileRenderer>();

            foreach (var card in game.Player1.Cards.Concat(game.Player2.Cards))
            {
                var cardObjet = Instantiate(cardRenderer);
                cardObjet.transform.localScale = CardScale;
                cardObjet.Card = card;
                cardObjet.gameObject.SetActive(false);
                CardRenderers.Add(card, cardObjet);
            }

            foreach (var pileRenderer in FindObjectsOfType<PileRenderer>()) pileRenderer.GrabPile(game);

            var evtMan = FindObjectOfType<SyncEventsManager>();
            evtMan.EventManager = game.EventManager;

            foreach (var eventSubscriber in FindObjectsOfType<MonoBehaviour>())
                if (eventSubscriber is IEventSubscriber evt)
                    evt.Subscribe(evtMan.SyncEventWrapper);

            interFaceObject.SetActive(true);
            connectingObject.SetActive(false);
            hostingObject.SetActive(false);
            // game.EventManager.SubscribeToEvent<CardPlayEvent>(e => Debug.Log($"Card played : {e.Card.Name}"));
        }


        public T RunOnGameThread<T>(Func<Game, T> func)
        {
            var res = Network.RunOnGameThread(func);
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
            if (queue.Count == 1) StartCoroutine(EmptyQueue(queue));
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
            return LocalGamePlayer == player;
        }

        public static NetworkMode GetSide(Player player)
        {
            return player.Id == 0 ? NetworkMode.Server : NetworkMode.Client;
        }
    }
}