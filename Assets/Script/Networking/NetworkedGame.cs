using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
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
using MoonSharp.Interpreter;
using Network;
using Network.Packets;
using Script.Debugging;
using Script.Input;
using Script.Networking.Commands;
using Script.Networking.Commands.Extern;
using Sentry;
using TMPro;
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


        private InputManager _inputManager;
        public NetworkMode AcceptFrom { get; set; } = NetworkMode.Server;
        private MainRenderer _localMainRenderer;
        private Thread _gameProcessThread;


        private void Awake()
        {
            _unityGame = FindObjectOfType<UnityGame>();
            errorUtils = FindObjectOfType<ErrorUtils>();
            _inputManager = FindObjectOfType<InputManager>();
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
                    if(NetworkGameState == NetworkGameState.SETTING_UP)
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
                AcceptFrom = AcceptFrom switch
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
                        if (_interrupted) return;
                        // Debug.Log($"On lock pour les commandes {Thread.CurrentThread.Name}");
                        if (NetworkGameState == NetworkGameState.PLAYING)
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
                            Monitor.PulseAll(this);
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
                SentrySdk.AddBreadcrumb($"Appel de EventRegistration/GameInit");
                if (e is LuaException le)
                {
                    errorUtils.toPrint.Enqueue(le);
                }
            }
        }

        private ConcurrentQueue<GameCommand> _gameCommands = new ConcurrentQueue<GameCommand>();

        private void ProcessGameCommands()
        {
            foreach (var curCommand in _gameCommands)
            {
                // Debug.Log("on process le packet");
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
                    if (e is LuaException le)
                    {
                        errorUtils.toPrint.Enqueue(le);
                    }

                    Debug.LogError(e);
                    _gameCommands.TryDequeue(out _);
                    _gameProcessThread.Interrupt();
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
                catch (ScriptRuntimeException e)
                {
                    Debug.LogError("Erreure lors d'une action sur le thread de jeu");
                    errorUtils.toPrint.Enqueue(e);
                    Debug.LogError(e);
                    task.SetException(e);
                }
                catch (Exception e)
                {
                    Debug.LogError("Erreure lors d'une action sur le thread de jeu");
                    SentrySdk.CaptureException(e);
                    Debug.LogError(e);
                    task.SetException(e);
                }
            }
        }

        private void CreateGame()
        {
            //on va dire que le serveur est j1
            var j1Deck = _networkManager.IsServer ? MyConfiguration!.deck : OtherPlayerConfiguration!.deck;
            var j2Deck = _networkManager.IsClient ? OtherPlayerConfiguration!.deck : MyConfiguration!.deck;

            //verification des decks


            if (DeckIsInvalid(MyConfiguration?.deck, "Vous"))
            {
                this.NetworkGameState = NetworkGameState.NOT_CONNECTED;
                return;
            }

            if (DeckIsInvalid(OtherPlayerConfiguration?.deck, "Adversaire"))
            {
                this.NetworkGameState = NetworkGameState.NOT_CONNECTED;
                return;
            }

            SentrySdk.StartSession();
            SentrySdk.ConfigureScope(s =>
            {
                s.Contexts["Game"] = new GameScope()
                {
                    J1Deck = j1Deck,
                    J2Deck = j2Deck,
                    CurrentSide = _networkManager.IsClient ? NetworkMode.Client.ToString() : NetworkMode.Server.ToString(),
                    CurrentPlayer = (int)(UnityGame.LocalPlayer) + 1,
                    RandomSeed = _randomSeed,
                };
            });
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
                // if (Application.isEditor)
                //     Debug.Break();
                // else
                // {
                //     Application.Quit();
                // }
                SentrySdk.CaptureException(exception);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                ErrorMessage("Une erreure est survenue lors de la création de la partie", "");
                this.NetworkGameState = NetworkGameState.NOT_CONNECTED;
                SentrySdk.CaptureException(e);
            }
        }

        private class GameScope
        {
            public List<string> J1Deck { get; set; }
            public List<string> J2Deck { get; set; }
            public string CurrentSide { get; set; }
            public int CurrentPlayer { get; set; }
            public int RandomSeed { get; set; }
        }

        private bool DeckIsInvalid(IReadOnlyCollection<string> deckToCheck, string playerName)
        {
            var allCards = Directory.EnumerateFiles(Application.streamingAssetsPath + "/EffectsScripts/Card")
                .Select(Path.GetFileNameWithoutExtension).ToImmutableHashSet();
            var invalid = deckToCheck?.FirstOrDefault(c => !allCards.Contains(c));
            if (invalid != null)
            {
                ErrorMessage($"carte invalide : {invalid}", playerName);
                return true;
            }

            var forbidden = deckToCheck?.FirstOrDefault(c => c.StartsWith("_"));
            if (forbidden != null)
            {
                ErrorMessage($"carte interdite : {forbidden}", playerName);
                return true;
            }

            if (deckToCheck?.Count != 12)
            {
                ErrorMessage($"nombre de cartes différent de 12, actuel : {deckToCheck?.Count}", playerName);
                return true;
            }
            

            var frequency = deckToCheck?.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
            var toomuch = deckToCheck?.FirstOrDefault(c => frequency[c] > 2);
            if (toomuch != null)
            {
                ErrorMessage($"carte présente plus de 2 fois : {toomuch}", playerName);
                return true;
            }

            return false;
        }

        private void ErrorMessage(string errorMessage, string playerName)
        {
            errorText.SetText(
                string.IsNullOrWhiteSpace(playerName)
                    ? errorMessage
                    : $"{playerName} : {errorMessage}"
            );
            errorText.gameObject.SetActive(true);
            _inputManager.EnableThis(InputManager.InputType.UI);
            var unityGame = FindObjectOfType<UnityGame>();
            unityGame.hostingObject.SetActive(false);
            unityGame.connectingObject.SetActive(false);
        }

        public TextMeshProUGUI errorText;


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
            // Debug.Log($"J'ai recu un paket");
            if ((AcceptFrom == NetworkMode.Client && _networkManager.IsClient) ||
                AcceptFrom == NetworkMode.Server && _networkManager.IsServer)
            {
                Debug.LogWarning(
                    "On a pas l'air d'etre synchronisé, un paquet de l'autre a été recu mais on en attendait pas");
            }

            AddCommandToQueue(packet);
            SentrySdk.AddBreadcrumb($"Action Distante : {packet.GetType()}","actions",packet.GetType().Name,packet.ToDict(_unityGame));
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
                        else
                        {
                            Debug.Log("Commande arrivée trop vite, en attente");
                            _tooSoonExternalCommands[command.GetType()] = externalCommand;
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
            // Debug.Log($"Action locale qu'il faut dupliquer");
            if ((AcceptFrom == NetworkMode.Client && _networkManager.IsServer) ||
                AcceptFrom == NetworkMode.Server && _networkManager.IsClient)
            {
                Debug.LogWarning("On a pas l'air d'etre synchronisé, on attendait un paquet distant");
                return;
            }
            else
            {
                SentrySdk.AddBreadcrumb($"Action Locale : {command.GetType()}","actions",command.GetType().Name,command.ToDict(_unityGame));
                AddCommandToQueue(command);
                _networkManager.Send(command);
            }
        }

        private volatile bool _interrupted;
        public ErrorUtils errorUtils;
        private Dictionary<Type, ExternalCommand> _tooSoonExternalCommands = new Dictionary<Type, ExternalCommand>();
         private UnityGame _unityGame;

        public T WaitForTaskWithPolling<T>(TaskCompletionSource<T> completionSource, bool onGameThread)
        {
            // Debug.Log($"On rentre dans WaitForTaskWithPolling {Thread.CurrentThread.Name}");
            lock (this)
            {
                while (true)
                {
                    if (_interrupted)
                    {
                        throw new ThreadInterruptedException();
                    }

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

            if (_tooSoonExternalCommands.ContainsKey(typeof(T)))
            {
                var elem = _tooSoonExternalCommands[typeof(T)];
                _tooSoonExternalCommands.Remove(typeof(T));
                return (T)elem;
            }

            var commandTask = new TaskCompletionSource<ExternalCommand>();
            _waitedExternalCommands[typeof(T)] = commandTask;

            return (T)WaitForTaskWithPolling(commandTask, true);
        }

        public T RunOnGameThread<T>(Func<Game, T> action)
        {
            var actionTask = new TaskCompletionSource<bool>();
            var res = default(T);
            GameThreadQueue.Enqueue((actionTask, () => { res = action(Game); }));

            WaitForTaskWithPolling(actionTask, false);

            return res;
        }

        public Card ResolveCard(int objCardId)
        {
            return Game.AllCards.First(c => c.Id == objCardId);
        }

        public Player ResolvePlayer(int playerId)
        {
            if (Game.Player1.Id == playerId)
            {
                return Game.Player1;
            }
            else if (Game.Player2.Id == playerId)
            {
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
                provider.InfoStruct = externStruct;
                provider.IsNeeded = true;
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