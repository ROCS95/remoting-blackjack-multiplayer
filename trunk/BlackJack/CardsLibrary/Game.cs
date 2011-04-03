using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackJackLibrary
{
    public class Game : MarshalByRefObject
    {
        private Shoe shoe = new Shoe( 3 );
        private List<Player> players = new List<Player>();
        private Dictionary<String, ICallback> callbacks = new Dictionary<String, ICallback>();
        private Player currentPlayer = null;
        private bool blnPlayersReady = true;
        private bool blnPlayersDone = true;

        public ICallback CallBack = null;


        public void Join( string name, ICallback callback )
        {
            try
            {
                Player newPlayer = new Player( name );
                players.Add( newPlayer );
                callbacks.Add( name, callback );

                //initial join, start play between dealer and player 1
                if( players.Count == 1 )
                {
                    currentPlayer = newPlayer;
                } //else wait for current players turn to be over

                Console.WriteLine( "Player: {0} has just joined the game!", newPlayer.Name );

                updateAllClients();
            }
            catch( Exception ex)
            {
                Console.WriteLine( "Error:" + ex );
            }
        }

        public void Hit( string name )
        {
            currentPlayer = getPlayer( name );
            dealCards( 1 );
            updateAllClients();
        }

        public void Ready( int bet, string name )
        {
            currentPlayer = getPlayer( name );
            currentPlayer.Status = StatusType.Ready;
            currentPlayer.Bank -= bet;
            currentPlayer.CurrentBet = bet;

            foreach( Player p in players )
            {
                if( p.Status != StatusType.Ready )
                    blnPlayersReady = false;
            }

            if( blnPlayersReady )
            {
                players[0].Status = StatusType.Playing;
                startGame();
                dealStartingPlayerCards( );
                updateAllClients();
            }
            else
            {
                blnPlayersReady = true;
            }

        }

        public Player getPlayer( string name )
        {
            foreach( Player player in players )
            {
                if ( name.Equals( player.Name ) )
                    return player;
            }
            return new Player( null );
        }

        public void Stay( string name )
        {
            currentPlayer = getPlayer( name );
            currentPlayer.Status = StatusType.Done;
            if( currentPlayer != players[players.Count - 1] )
                players[players.IndexOf( currentPlayer )+1].Status = StatusType.Playing;

            updateAllClients();

            if( getPlayer( "Dealer" ).Status == StatusType.Playing )
            {
                currentPlayer = getPlayer( "Dealer" );

                //Finish Dealer Hand

                //Switch Dealer to Done
                currentPlayer.Status = StatusType.Done;

                //Finish Game (Determine Payouts / Start next Hand)
                

            }
            

        }

        // Helper methods

        private void startGame()
        {
            players.Add( new Player( "Dealer" ) );
            dealDealerCards( 2 );
        }

        private List<Card> drawMultiple( int nCards, string name )
        {
            List<Card> cards = new List<Card>();
            for( int i = 0; i < nCards; i++ )
            {
                cards.Add( shoe.Draw() );
                    Console.WriteLine( "Deal: {0} to {1}", cards[i].Name, getPlayer(name).Name );
            }

            return cards;
        }

        private void updateAllClients()
        {
            foreach( Player player in players )
                if( !player.Name.Equals( "Dealer" ) )
                    callbacks[ player.Name ].PlayerUpdate( players );
        }

        private void dealStartingPlayerCards()
        {
            foreach( Player p in players )
            {
                if( !p.Name.Equals( "Dealer" ) )
                {
                    p.CardsInPlay.AddRange( drawMultiple( 2, p.Name ) );
                    //reset count
                    p.CardTotal = 0;
                    //get count of current cards
                    foreach( Card card in p.CardsInPlay )
                    {
                        //check if card is ace
                        p.CardTotal += card.Value;

                        if( p.CardTotal > 21 )
                        {
                            if( card.Rank == Card.RankID.Ace )
                                p.CardTotal -= 10;
                        }
                        else if( p.CardTotal == 21 )
                        {
                            //end current hand
                            p.Status = StatusType.Done;

                            //award wager amount * 3
                            p.Bank += p.CurrentBet * 3;
                        }
                    }
                }
            }
        }

        private void dealDealerCards( int nCards )
        {
            Player dealer = getPlayer( "Dealer" );
            dealer.CardsInPlay.AddRange( drawMultiple( nCards, dealer.Name ) );
            //reset count
            dealer.CardTotal = 0;
            //get count of current cards
            foreach( Card card in dealer.CardsInPlay )
            {

                dealer.CardTotal += card.Value;

                //check if card is ace
                if( dealer.CardTotal > 21 )
                {
                    if( card.Rank == Card.RankID.Ace )
                        dealer.CardTotal -= 10;
                }
                else if( dealer.CardTotal == 21 )
                {
                    //end current hand

                    //determine winners
                }
            }

        }

        private void dealCards( int nCards )
        {
            currentPlayer.CardsInPlay.AddRange( drawMultiple( nCards, currentPlayer.Name ) );
            //reset count
            currentPlayer.CardTotal = 0;
            //get count of current cards
            foreach( Card card in currentPlayer.CardsInPlay )
            {
                //check if card is ace
                currentPlayer.CardTotal += card.Value;

                if( currentPlayer.CardTotal > 21 )
                {
                    if( card.Rank == Card.RankID.Ace )
                    {
                        currentPlayer.CardTotal -= 10;
                    }
                    else
                    {
                        currentPlayer.Status = StatusType.Done;
                        if( currentPlayer != players[players.Count - 1] )
                            players[players.IndexOf( currentPlayer ) + 1].Status = StatusType.Playing;
                    }

                }
                else if( currentPlayer.CardTotal == 21 )
                {
                    //end current hand
                    currentPlayer.Status = StatusType.Done;
                    if( currentPlayer != players[players.Count - 1] )
                        players[players.IndexOf( currentPlayer ) + 1].Status = StatusType.Playing;
                }
            }
        }

    }
}





