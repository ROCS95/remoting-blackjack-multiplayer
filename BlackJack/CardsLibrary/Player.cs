using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackJackLibrary
{
    public enum StatusType { Betting, Ready, Playing, Done };

    [Serializable]
    public class Player
    {
        public string Name = "";
        //public PlayerState State = null;
        public List<Card> CardsInPlay = new List<Card>();
        public int Bank = 0;
        public int CurrentBet = 0;
        public int CardTotal = 0;

        public StatusType Status;

        public Player( string name )
        {
            //State = new PlayerState();
            Status = StatusType.Betting;
            Name = name;
            Bank = 500;

        }
    }
}
