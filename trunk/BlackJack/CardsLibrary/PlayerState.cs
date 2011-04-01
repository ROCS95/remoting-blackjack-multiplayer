﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackJackLibrary
{
    [Serializable]
    public class PlayerState
    {
        public List<Card> cards = new List<Card>();
        //public Dictionary<String, List<Card>> cardsInPlay = new Dictionary<string, List<Card>>();
        public int Bank = 0;
        public int CurrentBet = 0;
        public int CardTotal = 0;
        
        public enum StatusType { Betting, Playing, Stay };
    }
}