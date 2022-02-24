using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using CardGameEngine;
using CardGameEngine.Cards;
using CardGameEngine.EventSystem.Events.GameStateEvents;
using CardGameEngine.GameSystems;
using CardGameEngine.GameSystems.Effects;
using JetBrains.Annotations;
using Network;
using Network.Packets;
using Script.Debugging;
using Script.Networking.Commands;
using Script.Networking.Commands.Extern;
using UnityEngine;
using Random = System.Random;

namespace Script.Networking
{
    public partial class NetworkedGame : MonoBehaviour
    {
        public Game Game { get; private set; }

        private NetworkManager _networkManager;

        public NetworkGameState NetworkGameState { get; private set; }

        public Action<Game> EventRegistration;

        private ConcurrentDictionary<Type, CommandProviderBehaviour> _commandProviderBehaviours =
            new ConcurrentDictionary<Type, CommandProviderBehaviour>();

        private ConcurrentDictionary<Type, Action<ExternalCommand>> _waitedExternalCommands =
            new ConcurrentDictionary<Type, Action<ExternalCommand>>();

        private int _randomSeed;

        [CanBeNull] public SetUpGameCommand MyConfiguration { get; private set; }
        [CanBeNull] public SetUpGameCommand OtherPlayerConfiguration { get; private set; }

        private ConcurrentQueue<GameCommand> _gameCommands = new ConcurrentQueue<GameCommand>();

        private NetworkMode acceptFrom = NetworkMode.Server;


        private void Awake()
        {
            FindObjectOfType<DebugConsole>().Init();
            _networkManager = GetComponent<NetworkManager>();
        }

        public void SetUpNetworkGame(NetworkConfiguration networkConfiguration, string ownName, List<string> ownDeck)
        {
            NetworkGameState = NetworkGameState.NOT_CONNECTED;
            _networkManager.SetupNetworking(networkConfiguration, () =>
            {
                Debug.Log("Connection established");
                _networkManager.AddPacketHandler<SetUpGameCommand>(OnReceiveSetUp);
                _networkManager.AddPacketHandler<GameCommand>(ReceiveRemoteAction);
                NetworkGameState = NetworkGameState.SETTING_UP;
                BeginSetUp(ownName, ownDeck);
            });
        }

        private void OnReceiveSetUp(SetUpGameCommand packet, Connection connection)
        {
            if (_networkManager.IsClient)
            {
                Debug.Log("on prend le seed du serveur");
                _randomSeed = packet.randomSeed;
            }

            Debug.Log("Set up recu");
            OtherPlayerConfiguration = packet;
        }

        private void Update()
        {
            if (NetworkGameState == NetworkGameState.SETTING_UP) // donc il a pas la notre
            {
                if (OtherPlayerConfiguration != null && _networkManager.IsClient)
                {
                    //si on est le client qui vient de recevoir du serv, on lui balance
                    _networkManager.Send(MyConfiguration);
                }

                if (OtherPlayerConfiguration != null && MyConfiguration != null)
                {
                    //on a les deux, est pret
                    CreateGame();
                    NetworkGameState = NetworkGameState.READY;
                }
            }

            if (NetworkGameState == NetworkGameState.READY)
            {
                try
                {
                    EventRegistration?.Invoke(Game);
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception dans EventRegistration/GameInit");
                    Debug.LogException(e);
                }

                Game.EventManager.SubscribeToEvent<EndTurnEvent>(e =>
                {
                    acceptFrom = acceptFrom switch
                    {
                        NetworkMode.Client => NetworkMode.Server,
                        NetworkMode.Server => NetworkMode.Client,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    //on attend des paquets de l'autre si le tour a changé
                }, postEvent: true);
                // on commence
                Game.StartGame();
                NetworkGameState = NetworkGameState.PLAYING;
                new Thread(() =>
                {
                    while (NetworkGameState == NetworkGameState.PLAYING)
                    {
                        // on va prendre les actions une par une
                        if (_gameCommands.TryPeek(out var curCommand))
                        {
                            if (ProcessAction(curCommand))
                            {
                                //si on a process on enleve
                                _gameCommands.TryDequeue(out curCommand);
                            }
                            else
                            {
                                //si on a pas ce qu'il faut pour process on attend
                            }
                        }

                        Thread.Sleep(1000); //durée entre les paquets
                    }
                }).Start();
            }
        }

        private void CreateGame()
        {
            //on va dire que le serveur est j1
            var j1Deck = _networkManager.IsServer ? MyConfiguration!.deck : OtherPlayerConfiguration!.deck;
            var j2Deck = _networkManager.IsClient ? OtherPlayerConfiguration!.deck : MyConfiguration!.deck;
            try
            {
                Game = new Game(Application.streamingAssetsPath + "/EffectsScripts/",
                    new NetworkedExternCallbacks(_randomSeed, this), j1Deck, j2Deck);
            }
            catch (InvalidEffectException exception)
            {
                Debug.LogError($"Invalid effect exception : {exception.Message}");
                if (exception.InnerException != null)
                    Debug.LogException(exception.InnerException);
                if (Application.isEditor)
                    Debug.Break();
                else
                {
                    Application.Quit();
                }
            }
        }

        // protocole
        // hote -> SetUpGame (son nom, son deck)
        // client -> SetUpGame (son nom, son deck)
        // 
        private void BeginSetUp(string ownName, List<string> ownDeck)
        {
            if (NetworkGameState != NetworkGameState.SETTING_UP || MyConfiguration != null)
            {
                Debug.LogError("Trying to setup game after creation");
                return;
            }

            MyConfiguration = new SetUpGameCommand() {name = ownName, deck = ownDeck};

            if (_networkManager.IsServer)
            {
                MyConfiguration.randomSeed = new Random().Next();
                _randomSeed = MyConfiguration.randomSeed;
                _networkManager.Send(new SetUpGameCommand() {name = ownName, deck = ownDeck, randomSeed = _randomSeed});
            }
        }


        //explications
        // Déja ca fonctionne que en ipv6 je crois ?
        // pas sur mais pas le temps de test la, quqnd ca fonctionnait pas le port etait pas bon aussi
        // 
        //
        // Les deux doivent RegisterKnownTypes, mais je crois que ca le fait tout seul dans les containers
        // Le CLIENT, doit ajouter un listener avec PacketHandler et un objet
        // Le SERVEUR doit utiliser StaticPacketHandler
        // Quand le client envoit un paquet, il doit utiliser l'objet associé (pk ????)

        // Les paquets héritent RequestPacket
        // et potentiellement un responsePacket, avec une annotation pour dire il correspond a quel request
        // les champs doivent etre des PROPRIETES
        // sinon ca marche c cool

        private void ReceiveRemoteAction(GameCommand packet, Connection connection)
        {
            Debug.Log($"J'ai recu un paket");
            if ((acceptFrom == NetworkMode.Client && _networkManager.IsClient) ||
                acceptFrom == NetworkMode.Server && _networkManager.IsServer)
            {
                Debug.LogError(
                    "On a pas l'air d'etre synchronisé, un paquet de l'autre a été recu mais on en attendait pas");
                return;
            }
            else
            {
                _gameCommands.Enqueue(packet);
            }
        }


        public void DoLocalAction(GameCommand command)
        {
            Debug.Log($"Action locale qu'il faut dupliquer");
            if ((acceptFrom == NetworkMode.Client && _networkManager.IsServer) ||
                acceptFrom == NetworkMode.Server && _networkManager.IsClient)
            {
                Debug.LogError("On a pas l'air d'etre synchronisé, on attendait un paquet distant");
                return;
            }
            else
            {
                _gameCommands.Enqueue(command);
                _networkManager.Send(command);
            }
        }

        public bool IsLocalPlayer(Player player)
        {
            return Game.Player1 == player;
        }

        public void WaitFor<T>(Action<T> action) where T : ExternalCommand
        {
            _waitedExternalCommands[typeof(T)] = (e) => action((T) e);
        }

        public Card ResolveCard(int objCardId)
        {
            return objCardId < 0 ? null : Game.Player1.Cards.Concat(Game.Player2.Cards).First(c => c.Id == objCardId);
        }

        public Player ResolvePlayer(int playerId)
        {
            if (Game.Player1.Id == playerId)
                return Game.Player1;
            else if (Game.Player2.Id == playerId)
                return Game.Player2;
            else
            {
                throw new InvalidOperationException($"Playerid {playerId} does not exist");
            }
        }

        public void WantLocal<T>([CanBeNull] ExternData externStruct = null) where T : GameCommand
        {
            if (_commandProviderBehaviours.TryGetValue(typeof(T), out var provider))
            {
                provider.infoStruct = externStruct;
                provider.isNeeded = true;
            }
        }

        public void RegisterCommandProvider<T>(CommandProviderBehaviour cpb) where T : GameCommand
        {
            _commandProviderBehaviours[typeof(T)] = cpb;
        }
    }
}