using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackJackLibrary
{
    public enum PlayerStatusType { Waiting, Betting, Ready, Playing, Done };
    public enum HandStatusType { None, Winner, Push, Loser, BlackJack, Bust };

    [Serializable]
    public class Player
    {
        public string Name = "";
        public List<Card> CardsInPlay = new List<Card>();
        public int Bank = 0;
        public int CurrentBet = 0;
        public int CardTotal = 0;
        public bool isNewPlayer = false;


        public PlayerStatusType Status;

        public HandStatusType HandStatus;

        public Player( string name )
        {
            Status = PlayerStatusType.Betting;
            HandStatus = HandStatusType.None;
            Name = name;
            Bank = 500;

        }
    }
}
