/*Title: BlackJackLibrary
* Page: Player.cs
* Author: Brooke Thrower and Jeramie Hallyburton
* Description: Tracks the player information
* Created: Mar. 22, 2011
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackJackLibrary
{
    //game statuses
    public enum PlayerStatusType { Waiting, Betting, Ready, Playing, Done };
    public enum HandStatusType { None, Winner, Push, Loser, BlackJack, Bust };

    [Serializable]
    public class Player
    {
        //set defaults for player
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
