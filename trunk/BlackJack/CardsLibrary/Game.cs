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

        public ICallback CallBack = null;

        public Game()
        {
            players.Add(new Player("Dealer"));
            getPlayer( "Dealer" ).Status = PlayerStatusType.Ready;
        }

        public void Join( string name, ICallback callback )
        {
            try
            {
                Player newPlayer = new Player( name );
                players.Insert( players.Count-1, newPlayer );
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
            currentPlayer.Status = PlayerStatusType.Ready;
            currentPlayer.Bank -= bet;
            currentPlayer.CurrentBet = bet;

            foreach( Player p in players )
            {
                if( p.Status != PlayerStatusType.Ready )
                    blnPlayersReady = false;
            }

            if( blnPlayersReady )
            {
                players[0].Status = PlayerStatusType.Playing;
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
            currentPlayer.Status = PlayerStatusType.Done;
            if( currentPlayer != players[players.Count - 1] )
                players[players.IndexOf( currentPlayer ) + 1].Status = PlayerStatusType.Playing;

            updateAllClients();

            if( getPlayer( "Dealer" ).Status == PlayerStatusType.Playing )
            {
                currentPlayer = getPlayer( "Dealer" );

                finishDealerHand();

                currentPlayer.Status = PlayerStatusType.Done;
                updateAllClients();
                determinePayouts();
                updateAllClients();
            }
        }

        public void DoubleDown( string name )
        {
            currentPlayer = getPlayer( name );
            dealCards( 1 );
            currentPlayer.CurrentBet *= 2;
            updateAllClients();
            currentPlayer.Status = PlayerStatusType.Done;
        }

        // Helper methods
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
                        else
                            p.HandStatus = HandStatusType.Bust;
                    }
                    else if( p.CardTotal == 21 )
                    {
                        //end current hand
                        p.Status = PlayerStatusType.Done;
                        p.HandStatus = HandStatusType.BlackJack;
                        if( currentPlayer != players[players.Count - 1] )
                        {
                            players[players.IndexOf( currentPlayer ) + 1].Status = PlayerStatusType.Playing;
                            updateAllClients();
                        }
                        if( players[players.IndexOf( currentPlayer ) + 1].Name.Equals( "Dealer" ) )
                        {
                            currentPlayer = getPlayer( "Dealer" );
                            finishDealerHand();
                            updateAllClients();
                            determinePayouts();
                            updateAllClients();
                        }
                    }
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
                        currentPlayer.Status = PlayerStatusType.Done;
                        if( currentPlayer != players[players.Count - 1] )
                        {
                            players[players.IndexOf( currentPlayer ) + 1].Status = PlayerStatusType.Playing;
                            updateAllClients();
                        }
                        if( players[players.IndexOf( currentPlayer ) + 1].Name.Equals("Dealer"))
                        {
                            currentPlayer = getPlayer( "Dealer" );
                            finishDealerHand();
                            currentPlayer.Status = PlayerStatusType.Done;
                            updateAllClients();
                            determinePayouts();
                            updateAllClients();
                        }
                    }

                }
                else if( currentPlayer.CardTotal == 21 )
                {
                    //end current hand
                    currentPlayer.Status = PlayerStatusType.Done;
                    if( currentPlayer != players[players.Count - 1] )
                    {
                        players[players.IndexOf( currentPlayer ) + 1].Status = PlayerStatusType.Playing;
                        updateAllClients();
                    }
                    if( players[players.IndexOf( currentPlayer ) + 1].Name.Equals( "Dealer" ) )
                    {
                        currentPlayer = getPlayer( "Dealer" );
                        finishDealerHand();
                        currentPlayer.Status = PlayerStatusType.Done;
                        updateAllClients();
                        determinePayouts();
                        updateAllClients();
                    }
                }
            }
        }

        private void finishDealerHand()
        {
            if( currentPlayer.CardTotal < 17 )
            {
                dealCards( 1 );
                finishDealerHand();
            }
        }

        private void determinePayouts()
        {
            //Determine Payouts
            foreach( Player p in players )
            {
                if( p.Name != "Dealer" )
                {
                    if( p.HandStatus == HandStatusType.BlackJack )
                    {
                        p.Bank += p.CurrentBet * 3;
                        p.CurrentBet = 0;
                    }
                    else if( p.HandStatus == HandStatusType.Bust )
                    {
                        p.CurrentBet = 0;
                    }
                    else if( currentPlayer.CardTotal <= 21 )
                    {
                        if( p.CardTotal > currentPlayer.CardTotal )
                        {
                            p.HandStatus = HandStatusType.Winner;
                            p.Bank += p.CurrentBet * 2;
                            p.CurrentBet = 0;
                        }
                        else if( p.CardTotal < currentPlayer.CardTotal )
                        {
                            p.HandStatus = HandStatusType.Loser;
                            p.CurrentBet = 0;
                        }
                        else if( p.CardTotal == currentPlayer.CardTotal )
                        {
                            p.HandStatus = HandStatusType.Push;
                            p.Bank += p.CurrentBet;
                            p.CurrentBet = 0;
                        }
                    }
                    else if( currentPlayer.CardTotal > 21 )
                    {
                        if( p.CardTotal <= 21 )
                        {
                            p.Bank += p.CurrentBet * 2;
                            p.CurrentBet = 0;
                        }
                    }
                }
            }
        }

    }
}





