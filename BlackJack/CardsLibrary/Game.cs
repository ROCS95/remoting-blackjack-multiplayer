using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackJackLibrary
{
    public class Game : MarshalByRefObject
    {
        private Shoe shoe = new Shoe( 3 );
        private Dictionary<String, Player> players = new Dictionary<String, Player>();
        private Dictionary<String, ICallback> callbacks = new Dictionary<String, ICallback>();
        private Player currentPlayer = null;
        public ICallback CallBack = null;
        public int PlayerNum;
        public void Join( string name, ICallback callback )
        {
            try
            {
                Player newPlayer = new Player( name );
                players.Add( newPlayer.Name, newPlayer );
                callbacks.Add( name, callback );

                //initial join, start play between dealer and player 1
                if( players.Count == 1 )
                {
                    currentPlayer = newPlayer;
                    startGame();
                } //else wait for current players turn to be over

                Console.WriteLine( "Player: {0} has just joined the game!", newPlayer.Name );

                updateAllClients();
            }
            catch( Exception )
            {

            }
        }

        private void startGame()
        {
            players.Add( "Dealer", new Player( "Dealer" ) );
            dealDealerCards(2);
        }

        public void Hit()
        {
            dealCards( 1 );
            updateAllClients();
        }

        public void Ready( int bet, string name )
        {
            currentPlayer = getPlayer( name );
            dealCards( 2 );
            updateAllClients();
        }

        public Player getPlayer( string id )
        {
            return players[id];
        }

        private void dealDealerCards( int nCards )
        {
            players["Dealer"].State.CardsInPlay.AddRange( drawMultiple( nCards ) );
            //reset count
            players["Dealer"].State.CardTotal = 0;
            //get count of current cards
            foreach( Card card in players["Dealer"].State.CardsInPlay )
            {
                //check if card is ace
                players["Dealer"].State.CardTotal += card.Value;

                if( card.Rank == Card.RankID.Ace )
                    players["Dealer"].State.AceCount++;
                if( players["Dealer"].State.CardTotal > 21 && players["Dealer"].State.AceCount > 0 )
                {
                    players["Dealer"].State.CardTotal -= 10;
                    players["Dealer"].State.AceCount--;
                }
                else if( players["Dealer"].State.CardTotal == 21 )
                {
                    //end current hand

                    //award wager amount * 3

                    //move to next player

                    //if no more players

                    //finishDealersHand();
                }
            }

        }
        private void dealCards( int nCards )
        {
            currentPlayer.State.CardsInPlay.AddRange( drawMultiple( nCards ) );
            //reset count
            currentPlayer.State.CardTotal = 0;
            //get count of current cards
            foreach( Card card in currentPlayer.State.CardsInPlay )
            {
                //check if card is ace
                currentPlayer.State.CardTotal += card.Value;

                if( card.Rank == Card.RankID.Ace )
                    currentPlayer.State.AceCount++;
                if( currentPlayer.State.CardTotal > 21 && currentPlayer.State.AceCount > 0 )
                {
                    currentPlayer.State.CardTotal -= 10;
                    currentPlayer.State.AceCount--;
                }
                else if( currentPlayer.State.CardTotal == 21 )
                {
                    //end current hand

                    //award wager amount * 3

                    //move to next player

                    //if no more players

                    //finishDealersHand();
                }
            }
        }

        public void Stay()
        {
            currentPlayer.State.Status = StatusType.Done;
        }

        // Helper methods
        private List<Card> drawMultiple( int nCards )
        {
            List<Card> cards = new List<Card>();
            for( int i = 0; i < nCards; i++ )
                cards.Add( shoe.Draw() );

            return cards;
        }

        private void updateAllClients()
        {
            foreach( String key in players.Keys )
                if( !key.Equals( "Dealer" ) )
                    callbacks[key].PlayerUpdate( players );
        }

    }
}





