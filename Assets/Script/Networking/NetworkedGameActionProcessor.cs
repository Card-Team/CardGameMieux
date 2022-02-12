using Script.Networking.Commands;
using Script.Networking.Commands.Extern;

namespace Script.Networking
{
    public partial class NetworkedGame
    {
        // on continue la meme classe

        private bool ProcessAction(GameCommand curCommand)
        {
            if (curCommand is ExternalCommand externalCommand)
            {
                //on verifie les attendeurs
                if (_waitedExternalCommands.TryGetValue(curCommand.GetType(), out var value))
                {
                    value(externalCommand);
                    return true;
                }
                return false;
            }

            // ici on fait les appels sur Game comme il faut
            // normalement que endturn et playcard quoi
            if (curCommand is PlayCardCommand playCardCommand)
            {
                Game.PlayCard(ResolvePlayer(playCardCommand.PlayerId), ResolveCard(playCardCommand.CardId),
                    playCardCommand.Upgrade);
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