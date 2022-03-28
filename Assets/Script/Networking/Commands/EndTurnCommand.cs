using System.Collections.Generic;
using System.Collections.Immutable;
using CardGameEngine;

namespace Script.Networking.Commands
{
    public class EndTurnCommand : GameCommand
    {
        public override IDictionary<string, string> ToDict(UnityGame unityGame)
        {
            return ImmutableDictionary<string, string>.Empty;
        }
    }
}