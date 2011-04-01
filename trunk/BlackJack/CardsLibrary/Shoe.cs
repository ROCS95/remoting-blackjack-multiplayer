/*
 * Program:         CardsLibrary.dll
 * Module:          Shoe.cs
 * Author:          T. Haworth
 * Date:            February 24, 2011
 * Description:     Defines the Shoe type that implements a collection of Card 
 *                  objects simulating a casino-style "shoe" consisting 
 *                  of multiple decks (usually 5) of standard playing cards.
 * Version Info:    Modified to allow the Shoe class to be offered as a  service
 *                  via .NET Remoting. The Draw() method once again returns a Card 
 *                  object reference instead of a String. Also some "tracing" code
 *                  has been added to the Shoe's Draw() and Shuffle() methods.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackJackLibrary
{
    public class Shoe : MarshalByRefObject
    {
        // Protected member variables
        protected int numDecks = 1;
        protected RandomList<Card> cards = new RandomList<Card>();
        protected int cardIdx;

        // C'tor
        public Shoe()
        {
            repopulate();
        }

        // Can only use the default c'tor with a wellknown (server-activated) type!
        public Shoe(int nDecks)
        {
            numDecks = nDecks;
            repopulate();
        }

        // Public methods and "properties"

        public void Shuffle()
        {
            Console.WriteLine("Shuffling: {0} decks", numDecks);
            cardIdx = 0;
            cards.Shuffle();
        }

        public Card Draw()
        {
            if (cardIdx == cards.Count)
                throw new System.IndexOutOfRangeException("The shoe is empty. Please reset.");
            
            Console.WriteLine("Deal: {0}", cards[cardIdx].Name);

            return cards[cardIdx++];
        }

        public int NumDecks
        {
            get 
            { 
                return numDecks; 
            }
            set 
            {
                if (numDecks != value)
                {
                    numDecks = value;
                    repopulate();
                }
            }
        }

        public int NumCards
        {
            get { return cards.Count - cardIdx; } 
        }

        // Helper methods

        protected void repopulate()
        {
            // Remove "old" car
            cards.Clear();

            // Populate with new cards
            for( int d = 0; d < numDecks; d++)
                for (Card.SuitID s = Card.SuitID.Clubs; s <= Card.SuitID.Spades; s++)
                    for (Card.RankID r = Card.RankID.Ace; r <= Card.RankID.Two; r++)
                        cards.Add(new Card(s, r));

            // Reset cards index and randomize the collection
            Shuffle();
        }

    } // end class Shoe

} // end namespace CardsLibrary
