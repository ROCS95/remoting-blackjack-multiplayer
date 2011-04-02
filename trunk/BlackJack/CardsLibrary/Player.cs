using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackJackLibrary
{
    [Serializable]
    public class Player
    {
        public string Name = "";
        public PlayerState State = null;
        public int playerNum = 0;
        public Player( string name )
        {
            State = new PlayerState();
            Name = name;
            State.Bank = 500;

        }
    }
}
