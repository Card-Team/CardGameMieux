using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
using UnityEditor;
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

        private ConcurrentDictionary<Type, TaskCompletionSource<ExternalCommand>> _waitedExternalCommands =
            new ConcurrentDictionary<Type, TaskCompletionSource<ExternalCommand>>();

        private int _randomSeed;

        [CanBeNull] public SetUpGameCommand MyConfiguration { get; private set; }
        [CanBeNull] public SetUpGameCommand OtherPlayerConfiguration { get; private set; }


        private NetworkMode acceptFrom = NetworkMode.Server;
        private MainRenderer _localMainRenderer;
        private Thread _gameProcessThread;


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
            else if (NetworkGameState == NetworkGameState.READY)
            {
                StartGame();
            }
        }

        private void StartGame()
        {
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
            _localMainRenderer = FindObjectsOfType<MainRenderer>()
                .First(r => r.owner == UnityGame.LocalPlayer);
            NetworkGameState = NetworkGameState.PLAYING;
            _gameProcessThread = new Thread(() =>
            {
                while (NetworkGameState == NetworkGameState.PLAYING || 
                       NetworkGameState == NetworkGameState.READY || 
                       NetworkGameState == NetworkGameState.STARTING)
                {
                    if (NetworkGameState == NetworkGameState.STARTING)
                    {
                        Game.StartGame();
                        NetworkGameState = NetworkGameState.PLAYING;
                    }
                    lock (this)
                    {
                        // Debug.Log($"On lock pour les commandes {Thread.CurrentThread.Name}");
                        if(NetworkGameState == NetworkGameState.PLAYING )
                            ProcessGameCommands();
                        ProcessGameThreadActions();
                        // Debug.Log($"On a process les commandes et les actions{Thread.CurrentThread.Name}");
                        // Debug.Log($"On va pulse {Thread.CurrentThread.Name}");
                        Monitor.PulseAll(this);
                        // Debug.Log($"On a pulse, dodo {Thread.CurrentThread.Name}");
                        try
                        {
                            Monitor.Wait(this);
                        }
                        catch (ThreadInterruptedException)
                        {
                            _interrupted = true;
                            Debug.Log("Thread interrompu, on arette");
                            return;
                        }
                        // Debug.Log($"Le reveil {Thread.CurrentThread.Name}");
                    }
                    // Debug.Log($"Sorti du verrou {Thread.CurrentThread.Name}");
                }
            });
            _gameProcessThread.Name = "GameThread";
            _gameProcessThread.Start();
            
            try
            {
                EventRegistration?.Invoke(Game);
                lock (this)
                {
                    NetworkGameState = NetworkGameState.STARTING;
                    Monitor.PulseAll(this);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Exception dans EventRegistration/GameInit");
                Debug.LogException(e);
            }
        }

        private ConcurrentQueue<GameCommand> _gameCommands = new ConcurrentQueue<GameCommand>();

        private void ProcessGameCommands()
        {
            foreach (var curCommand in _gameCommands)
            {
                Debug.Log("on process le packet");
                try
                {
                    if (ProcessAction(curCommand))
                    {
                        //si on a process on enleve
                        _gameCommands.TryDequeue(out _);
                    }
                    else
                    {
                        //si on a pas ce qu'il faut pour process on attend
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Erreure lors du processing du paquet {curCommand}");
                    Debug.LogError(e);
                    _gameCommands.TryDequeue(out _);
                }
            }
        }

        public readonly ConcurrentQueue<(TaskCompletionSource<bool> actionTask, Action action)> GameThreadQueue =
            new ConcurrentQueue<(TaskCompletionSource<bool> actionTask, Action action)>();

        private void ProcessGameThreadActions()
        {
            while (GameThreadQueue.TryDequeue(out var elem))
            {
                var (task, action) = elem;
                try
                {
                    action();
                    task.SetResult(true);
                }
                catch (Exception e)
                {
                    Debug.LogError("Erreure lors d'une action sur le thread de jeu");
                    Debug.LogError(e);
                }
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

            MyConfiguration = new SetUpGameCommand() { name = ownName, deck = ownDeck };

            if (_networkManager.IsServer)
            {
                MyConfiguration.randomSeed = new Random().Next();
                _randomSeed = MyConfiguration.randomSeed;
                _networkManager.Send(
                    new SetUpGameCommand() { name = ownName, deck = ownDeck, randomSeed = _randomSeed });
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
                AddCommandToQueue(packet);
            }
        }

        private void AddCommandToQueue(GameCommand command)
        {
            lock (this)
            {
                if (command is ExternalCommand externalCommand)
                {
                    //on verifie les attendeurs
                    lock (this)
                    {
                        if (_waitedExternalCommands.TryRemove(command.GetType(), out var value))
                        {
                            value.SetResult(externalCommand);
                        }
                    }
                }
                else
                {
                    _gameCommands.Enqueue(command);
                }

                Monitor.PulseAll(this);
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
                AddCommandToQueue(command);
                _networkManager.Send(command);
            }
        }

        private volatile bool _interrupted;

        public T WaitForTaskWithPolling<T>(TaskCompletionSource<T> completionSource, bool onGameThread)
        {
            if (_interrupted)
            {
                throw new ThreadInterruptedException();
            }
            // Debug.Log($"On rentre dans WaitForTaskWithPolling {Thread.CurrentThread.Name}");
            lock (this)
            {
                while (true)
                {
                    // Debug.Log($"On pulse all {Thread.CurrentThread.Name}");
                    Monitor.PulseAll(this);
                    switch (completionSource.Task.Status)
                    {
                        case TaskStatus.Canceled:
                            throw new InvalidOperationException("External command task was cancelled");
                        case TaskStatus.Created:
                        case TaskStatus.Running:
                        case TaskStatus.WaitingForActivation:
                        case TaskStatus.WaitingForChildrenToComplete:
                        case TaskStatus.WaitingToRun:
                            // Debug.Log($"la tache est pas finie {Thread.CurrentThread.Name}");
                            if (onGameThread)
                            {
                                // Debug.Log($"GameThread, on process {Thread.CurrentThread.Name}");
                                ProcessGameThreadActions();
                            }

                            // Debug.Log($"Dodo {Thread.CurrentThread.Name}");
                            try
                            {
                                Monitor.Wait(this);
                            }
                            catch (ThreadInterruptedException)
                            {
                                _interrupted = true;
                                throw;
                            }

                            // Debug.Log($"Debout{Thread.CurrentThread.Name}");
                            break;
                        case TaskStatus.Faulted:
                            if (completionSource.Task.Exception != null)
                                throw completionSource.Task.Exception;
                            else throw new InvalidOperationException();
                        case TaskStatus.RanToCompletion:
                            return completionSource.Task.Result;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        public T WaitForExternalCommand<T>() where T : ExternalCommand
        {
            if (_waitedExternalCommands.ContainsKey(typeof(T)))
                throw new InvalidOperationException(
                    $"Il y a déja une attente pour une commande de type {typeof(T)}");
            var commandTask = new TaskCompletionSource<ExternalCommand>();
            _waitedExternalCommands[typeof(T)] = commandTask;

            return (T)WaitForTaskWithPolling(commandTask, true);
        }
        
        public T RunOnGameThread<T>(Func<Game,T> action)
        {
            var actionTask = new TaskCompletionSource<bool>();
            var res = default(T);
            GameThreadQueue.Enqueue((actionTask, () => { res = action(Game); }));

            WaitForTaskWithPolling(actionTask, false);

            return res;
        }


        public Card ResolveCard(int objCardId)
        {
            return objCardId < 0 ? null : Game.Player1.Cards.Concat(Game.Player2.Cards).First(c => c.Id == objCardId);
        }

        public Player ResolvePlayer(int playerId)
        {
            Debug.Log($"Resolving playerid {playerId}");
            if (Game.Player1.Id == playerId)
            {
                Debug.Log("its first");
                return Game.Player1;
            }
            else if (Game.Player2.Id == playerId)
            {
                Debug.Log("its second");
                return Game.Player2;
            }
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

        private void OnDisable()
        {
            NetworkGameState = NetworkGameState.NOT_CONNECTED;
            _gameProcessThread?.Interrupt();
            _interrupted = true;
        }
    }
}