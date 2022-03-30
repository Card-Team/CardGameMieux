using System.Collections.Generic;
using CardGameEngine.EventSystem.Events;
using CardGameEngine.EventSystem.Events.CardEvents;
using CardGameEngine.EventSystem.Events.CardEvents.PropertyChange;
using CardGameEngine.EventSystem.Events.GameStateEvents;
using CardGameEngine.GameSystems;
using Script.Networking;
using Sentry;
using TMPro;
using UnityEngine;
using Event = CardGameEngine.EventSystem.Events.Event;

namespace Script
{
    public class EventDisplayer : MonoBehaviour, IEventSubscriber
    {
        public TMP_InputField eventPanel;

        private NetworkedGame _networkedGame;

        public List<string> Events { get; } = new List<string>();

        private void Start()
        {
            _networkedGame = FindObjectOfType<NetworkedGame>();
            eventPanel.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.RightControl))
            {
                eventPanel.gameObject.SetActive(!eventPanel.gameObject.activeSelf);
                eventPanel.caretPosition = eventPanel.text.Length;
            }
        }

        public void Subscribe(SyncEventWrapper eventManager)
        {
            // Carte monte de niveau
            eventManager.SubscribeToEvent<CardLevelChangeEvent>(OnCardLevelChange, postEvent: true);

            // Carte jouée puis effet exécuté
            eventManager.SubscribeToEvent<CardPlayEvent>(OnCardPlay, postEvent: false);
            eventManager.SubscribeToEvent<CardEffectPlayEvent>(OnCardEffectPlay, postEvent: false);
            eventManager.SubscribeToEvent<CardEffectPlayEvent>(OnPostCardEffectPlay, postEvent: true,
                evenIfCancelled: true);
            eventManager.SubscribeToEvent<CardPlayEvent>(OnPostCardPlay, postEvent: true);

            // Déplacement d'une carte
            eventManager.SubscribeToEvent<CardMovePileEvent>(OnCardChangePile, postEvent: true);

            // Mise en amélioration d'une carte 
            eventManager.SubscribeToEvent<CardMarkUpgradeEvent>(OnCardMarkedUpgrade, postEvent: true);

            eventManager.SubscribeToEvent<CardUnMarkUpgradeEvent>(OnCardRemovedMarkedUpgrade, postEvent: true);

            // Bouclage du deck
            eventManager.SubscribeToEvent<DeckLoopEvent>(OnDeckLoop, postEvent: true);

            eventManager.SubscribeToEvent<ChainingEvent>(OnChainStart);
            eventManager.SubscribeToEvent<ChainingEvent>(OnChainEnd, postEvent: true);

            // Nombre de point d'action
            eventManager.SubscribeToEvent<ActionPointsEditEvent>(OnActionPointsEdit, postEvent: true);

            // Nombre max de points d'actions
            eventManager.SubscribeToEvent<MaxActionPointsEditEvent>(OnMaxActionPointsEdit, postEvent: true);
            eventManager.SubscribeToEvent<StartTurnEvent>(OnStartTurn, postEvent: true);
        }

        private void OnStartTurn(StartTurnEvent evt)
        {
            WriteEvent<StartTurnEvent>(
                $"----------------C'est au tour de {GetPlayerName(evt.Player)} !----------------");
        }

        private void OnChainStart(ChainingEvent evt)
        {
            WriteEvent<ChainingEvent>(
                $"{GetPlayerName(evt.Chainer)} chaine ! (niveau de chaine : {_networkedGame.Game.ChainCounter})");
        }

        private void OnChainEnd(ChainingEvent evt)
        {
            WriteEvent<ChainingEvent>(
                $"{GetPlayerName(evt.Chainer)} termine sa chaine ! (niveau de chaine : {_networkedGame.Game.ChainCounter})");
        }

        private void OnMaxActionPointsEdit(MaxActionPointsEditEvent evt)
        {
            WriteEvent<MaxActionPointsEditEvent>(
                $"la limite de <color=\"green\">points d'action</color> de {GetPlayerName(evt.Player)} est maintenant de <b>{evt.NewMaxPointCount}</b>");
        }

        private string GetPlayerName(Player evtPlayer)
        {
            return $"Player{evtPlayer.Id + 1}";
        }

        private void OnActionPointsEdit(ActionPointsEditEvent evt)
        {
            WriteEvent<ActionPointsEditEvent>(
                $"{GetPlayerName(evt.Player)} a désormais <b>{evt.NewPointCount}</b> <color=\"green\">points d'action (sur {evt.Player.MaxActionPoints.Value})</color>");
        }

        private void OnCardMarkedUpgrade(CardMarkUpgradeEvent evt)
        {
            WriteEvent<CardMarkUpgradeEvent>($"<i>{evt.Card.Name}</i> est prête à se faire améliorer");
        }

        private void OnCardRemovedMarkedUpgrade(CardUnMarkUpgradeEvent evt)
        {
            WriteEvent<CardUnMarkUpgradeEvent>($"<i>{evt.Card.Name}</i> <b>ne sera pas améliorée</b>");
        }

        private void OnDeckLoop(DeckLoopEvent evt)
        {
            WriteEvent<DeckLoopEvent>(
                $"Le deck de <b>{GetPlayerName(evt.Player)}</b> a bouclé et contient maintenant <b>{evt.Player.Deck.Count}</b> cartes");
        }

        private void OnCardChangePile(CardMovePileEvent evt)
        {
            var players = new List<Player>
            {
                UnityGame.LocalGamePlayer,
                UnityGame.LocalGamePlayer.OtherPlayer
            };

            foreach (var player in players)
                // Main → Défausse = Défausser/Améliorer
                if (evt.SourcePile == player.Hand && evt.DestPile == player.Discard)
                    WriteEvent<CardMovePileEvent>(
                        $"La carte <i>{evt.Card.Name}</i> de <b>{GetPlayerName(player)}</b> se dirige vers <color=\"red\">la défausse</color>");

                // Deck → Main = Pioche
                else if (evt.SourcePile == player.Deck && evt.DestPile == player.Hand)

                    if (Equals(player, UnityGame.LocalGamePlayer))
                        // Tu pioches
                        WriteEvent<CardMovePileEvent>($"Vous venez de piocher <i>{evt.Card.Name}</i>");
                    else
                        // L'adversaire pioche
                        WriteEvent<CardMovePileEvent>($"<b>{GetPlayerName(player)}</b> vient de piocher une carte");
        }

        private void OnCardPlay(CardPlayEvent evt)
        {
            var virtualString = evt.Card.IsVirtual ? " <color=\"blue\">Virtuelle</color>" : "";
            WriteEvent<CardPlayEvent>(
                $"<i>{evt.Card.Name}</i>{virtualString} vient d'être jouée par <b>{GetPlayerName(evt.WhoPlayed)}</b>");
        }

        private void OnCardEffectPlay(CardEffectPlayEvent evt)
        {
            var virtualString = evt.Card.IsVirtual ? " <color=\"blue\">Virtuelle</color>" : "";
            WriteEvent<CardEffectPlayEvent>(
                $"L'effet de <i>{evt.Card.Name}</i>{virtualString} vient d'être activé par <b>{GetPlayerName(evt.WhoPlayed)}</b>");
        }

        private void OnPostCardEffectPlay(CardEffectPlayEvent evt)
        {
            var virtualString = evt.Card.IsVirtual ? " <color=\"blue\">Virtuelle</color>" : "";
            if (evt.Cancelled)
                WriteEvent<CardEffectPlayEvent>(
                    $"L'effet de <i>{evt.Card.Name}</i>{virtualString} <color=\"red\"> a été annulé</color>");

            WriteEvent<CardEffectPlayEvent>(
                $"L'effet de <i>{evt.Card.Name}</i>{virtualString} a terminé son execution");
        }

        private void WriteEvent<T>(string evt) where T : Event
        {
            Events.Add(evt);
            eventPanel.text += evt + "\n";
            SentrySdk.AddBreadcrumb(evt, "evenements", typeof(T).Name);
        }

        public string DumpEvents()
        {
            return string.Join("\n", Events);
        }

        public void ClearEvents()
        {
            Events.Clear();
        }

        private void OnPostCardPlay(CardPlayEvent evt)
        {
            WriteEvent<CardPlayEvent>($"La carte <i>{evt.Card.Name}</i> a fini d'être jouée");
        }

        private void OnCardLevelChange(CardLevelChangeEvent evt)
        {
            // Niveau monte
            if (evt.NewValue > evt.OldValue)
                WriteEvent<CardLevelChangeEvent>(evt.Card.IsMaxLevel
                    ? $"<i>{evt.Card.Name}</i> est maintenant au <color=\"blue\">niveau max ({evt.Card.CurrentLevel})</color>"
                    : $"<i>{evt.Card.Name}</i> est maintenant au <color=\"blue\">niveau {evt.Card.CurrentLevel}/{evt.Card.MaxLevel}</color>");

            // Niveau baisse
            else
                WriteEvent<CardLevelChangeEvent>(
                    $"<i>{evt.Card.Name}</i> est redescendu au <color=\"blue\">niveau {evt.NewValue}</color>");
        }
    }
}