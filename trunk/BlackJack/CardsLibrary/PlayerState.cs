using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackJackLibrary
{
    [Serializable]
    public class PlayerState
    {
        public List<Card> CardsInPlay = new List<Card>();
        public int AceCount = 0;
        public int Bank = 0;
        public int CurrentBet = 0;
        public int CardTotal = 0;
        public enum StatusType { Betting, Playing, Stay };
    }
}