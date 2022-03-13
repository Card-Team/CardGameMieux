using CardGameEngine.EventSystem;
using Script.Input;
using Script.Networking.Commands;
using Script.Networking.Commands.Extern;
using UnityEngine;
using Event = CardGameEngine.EventSystem.Events.Event;

namespace Script.Networking
{
    public partial class NetworkedGame
    {
        // on continue la meme classe

        private bool ProcessAction(GameCommand curCommand)
        {
            // ici on fait les appels sur Game comme il faut
            // normalement que endturn et playcard quoi
            if (curCommand is PlayCardCommand playCardCommand)
            {
                Game.PlayCard(ResolvePlayer(playCardCommand.PlayerId), ResolveCard(playCardCommand.CardId),
                    playCardCommand.Upgrade);
                _inputManager.playFinished = true;
                return true;
            } else if (curCommand is EndTurnCommand endTurnCommand)
            {
                Game.EndPlayerTurn();
                return true;
            }

            return false;
        }
    }
}