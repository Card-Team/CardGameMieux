using System.Collections.Generic;

namespace Script.Networking.Commands.Extern
{
    public class ChoosePlayerTargetCommand : ExternalCommand
    {
        public int PlayerId { get; set; }

        public override IDictionary<string, string> ToDict(UnityGame unityGame)
        {
            return new Dictionary<string, string> { { "PlayerId", PlayerId.ToString() } };
        }
    }
}