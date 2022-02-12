namespace Script.Networking.Commands.Extern
{
    public class ChainTurnCommand : ExternalCommand
    {
        public int CardID { get; set; } //négatif si jamais le joueur cancel
    }
}