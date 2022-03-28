using System.Collections.Generic;

namespace Script.Networking.Commands.Extern
{
    public class ChooseBetweenCardsCommand : ExternalCommand
    {
        public int CardId { get; set; }
        
        public override IDictionary<string, string> ToDict(UnityGame unityGame)
        {
            return new Dictionary<string, string> { { "CardId", CardId.ToString() } };
        }
    }
}