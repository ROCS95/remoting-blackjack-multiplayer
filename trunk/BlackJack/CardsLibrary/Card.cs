/*
 * Program:         CardsLibrary.dll
 * Module:          Card.cs
 * Date:            February 24, 2011
 * Author:          T. Haworth
 * Description:     Represents a standard playing card.
 * Version Info:    Modified by adding the [Serializable] attribute which allows 
 *                  the Card class to be used remotely. The marshaling type
 *                  is marshal-by-value
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackJackLibrary
{
    [Serializable]
    public class Card
    {
        public enum SuitID
        {
            Clubs, Diamonds, Hearts, Spades
        };

        public enum RankID
        {
            Ace, King, Queen, Jack, Ten, Nine, Eight, Seven,
            Six, Five, Four, Three, Two
        };

        // The Card's state

        protected SuitID    suit;
        protected RankID    rank;
        protected int       value;

        // 'C'tor

        public Card(SuitID s, RankID r)
        {
            suit = s;
            rank = r;

            switch (rank)
            {
                case RankID.Ace:
                    value = 11;
                    break;
                case RankID.King:
                case RankID.Queen:
                case RankID.Jack:
                case RankID.Ten:
                    value = 10;
                    break;
                case RankID.Nine:
                    value = 9;
                    break;
                case RankID.Eight:
                    value = 8;
                    break;
                case RankID.Seven:
                    value = 7;
                    break;
                case RankID.Six:
                    value = 6;
                    break;
                case RankID.Five:
                    value = 5;
                    break;
                case RankID.Four:
                    value = 4;
                    break;
                case RankID.Three:
                    value = 3;
                    break;
                case RankID.Two:
                    value = 2;
                    break;
            }
        }

        // Accessor Methods

        public SuitID Suit
        {
            get { return suit; }
        }

        public RankID Rank
        {
            get { return rank; }
        }

        public string Name
        {
            get 
            {
                return rank.ToString() + " of " + suit.ToString();
            }
        }
        public int Value {
            get
            {
                return value;
            }
        }

    } // end class Card

} // end namespace CardsLibrary
