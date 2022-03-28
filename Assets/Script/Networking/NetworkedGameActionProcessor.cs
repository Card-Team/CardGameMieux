using Script.Networking.Commands;
using Sentry;

namespace Script.Networking
{
    public partial class NetworkedGame
    {
        // on continue la meme classe

        private bool ProcessAction(GameCommand curCommand)
        {
            SentrySdk.AddBreadcrumb($"Calcul d'action : {curCommand.GetType()}", "calcul_action",
                curCommand.GetType().Name, curCommand.ToDict(_unityGame));
            // ici on fait les appels sur Game comme il faut
            // normalement que endturn et playcard quoi
            if (curCommand is PlayCardCommand playCardCommand)
            {
                Game.PlayCard(ResolvePlayer(playCardCommand.PlayerId), ResolveCard(playCardCommand.CardId),
                    playCardCommand.Upgrade);
                _inputManager.playFinished = true;
                return true;
            }

            if (curCommand is EndTurnCommand endTurnCommand)
            {
                Game.EndPlayerTurn();
                return true;
            }


            return false;
        }
    }
}