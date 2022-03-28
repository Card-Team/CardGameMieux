using System.Collections.Generic;
using System.Collections.Immutable;

namespace Script.Networking.Commands.Extern
{
    public abstract class ExternalCommand : GameCommand
    {
        public override IDictionary<string, string> ToDict(UnityGame unityGame)
        {
            return ImmutableDictionary<string, string>.Empty;
        }
    }
}