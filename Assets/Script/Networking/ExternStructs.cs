using System.Collections.Generic;
using CardGameEngine.Cards;
using CardGameEngine.GameSystems;

namespace Script.Networking
{
    public class GameEndedFalseData : ExternData
    {
        public Player Winner { get; set; }
    }

    public class ShowCardFalseData : ExternData
    {
        public Card Card { get; set; }
    }

    public class ExternData
    {
    }

    public class ChooseCardTargetData : ExternData
    {
        public string TargetName;
        public List<Card> CardList;
    }

    public class ChooseBetweenCardData : ExternData
    {
        public List<Card> CardList;
    }

    public class ChoosePlayerTargetData : ExternData
    {
        public string TargetName;
    }
}