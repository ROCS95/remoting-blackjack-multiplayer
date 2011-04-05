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
        private bool isGameInPlay = false;
        private bool isRoundFinished = false;

        public bool IsRoundFinished
        {
            get
            {
                return isRoundFinished;
            }
            set
            {
                ;
            }
        }
        public ICallback CallBack = null;

        public Game()
        {
            players.Add( new Player( "Dealer" ) );
            getPlayer( "Dealer" ).Status = PlayerStatusType.Ready;
        }

        public int GetPlayerCount() { return players.Count; }
        public void Join( string name, ICallback callback )
        {
            foreach( Player p in players )
            {
                if( name.Equals( p.Name ) )
                {
                    throw new Exception( "Player name already exists" );
                }
            }

            if( players.Count < 6 )
            {
                Player newPlayer = new Player( name );
                players.Insert( 0, newPlayer );
                callbacks.Add( name, callback );

                if( isGameInPlay )
                {
                    newPlayer.Status = PlayerStatusType.Done;
                    newPlayer.isNewPlayer = true;
                }
                Console.WriteLine( "Player {0} has just joined the game!", newPlayer.Name );
                updateAllClients();
            }
            else
            {
                Console.WriteLine( "Game is full {0} could not join.", name );
                throw new Exception( "This Game is full" );
            }
        }

        public void Hit( string name )
        {
            currentPlayer = getPlayer( name );
            Console.WriteLine( "Player {0} has hit:", name );
            dealCards( 1 );
            updateAllClients();
        }

        public void Ready( int bet, string name )
        {
            isGameInPlay = true;
            isRoundFinished = false;

            //check if shoe is low
            if( shoe.NumCards < players.Count * 5 )
                shoe.Shuffle();
            Player dealer = getPlayer( "Dealer" );
            if( dealer.Status != PlayerStatusType.Ready )
            {
                dealer.Status = PlayerStatusType.Ready;
                dealer.CardsInPlay.Clear();
            }
            currentPlayer = getPlayer( name );
            currentPlayer.Status = PlayerStatusType.Ready;
            currentPlayer.HandStatus = HandStatusType.None;
            currentPlayer.CardsInPlay.Clear();
            currentPlayer.Bank -= bet;
            currentPlayer.CurrentBet = bet;

            foreach( Player p in players )
            {
                if( p.Status != PlayerStatusType.Ready )
                    blnPlayersReady = false;
                if( p.isNewPlayer )
                    p.isNewPlayer = false;
                p.HandStatus = HandStatusType.None;
                p.CardsInPlay.Clear();
                p.CardTotal = 0;
            }

            if( blnPlayersReady )
            {
                players[0].Status = PlayerStatusType.Playing;
                Console.WriteLine( "Player {0} is now Playing.", players[0].Name );
                dealStartingPlayerCards();
            }
            else
            {
                blnPlayersReady = true;
            }
            updateAllClients();

        }

        public Player getPlayer( string name )
        {
            foreach( Player player in players )
            {
                if( name.Equals( player.Name ) )
                    return player;
            }
            return new Player( null );
        }

        public void Stay( string name )
        {
            Console.WriteLine( "Player {0} stays", name );
            currentPlayer = getPlayer( name );
            switchToNextPlayer();
        }

        public void DoubleDown( string name )
        {
            currentPlayer = getPlayer( name );
            dealCards( 1 );
            Console.WriteLine( "Player: {0} has doubled down:", name );
            currentPlayer.Bank -= currentPlayer.CurrentBet;
            currentPlayer.CurrentBet *= 2;
            switchToNextPlayer();
        }

        public void removePlayer( string name )
        {
            try
            {
                if( getPlayer(name) != null )
                {
                    if( getPlayer( name ).Status == PlayerStatusType.Playing )
                    {
                        switchToNextPlayer();
                    }
                    callbacks.Remove( name );
                    players.Remove( getPlayer( name ) );
                    Console.WriteLine( "Player {0} has left the game", name );
                }
            }
            catch( Exception ex )
            {
                Console.WriteLine( "Error: " + ex );
            }
            updateAllClients();
        }

        // Helper methods
        private List<Card> drawMultiple( int nCards, string name )
        {
            List<Card> cards = new List<Card>();
            for( int i = 0; i < nCards; i++ )
            {
                cards.Add( shoe.Draw() );
                Console.WriteLine( "Deal: {0} to {1}", cards[i].Name, getPlayer( name ).Name );
            }

            return cards;
        }

        private void updateAllClients()
        {
            foreach( Player player in players )
                if( !player.Name.Equals( "Dealer" ) )
                    callbacks[player.Name].PlayerUpdate( players );
        }

        private void dealStartingPlayerCards()
        {
            foreach( Player p in players )
            {
                p.CardsInPlay.AddRange( drawMultiple( 2, p.Name ) );
                //reset count
                p.CardTotal = 0;

                int aceCount = 0;
                //get count of current cards
                foreach( Card card in p.CardsInPlay )
                {
                    p.CardTotal += card.Value;
                    //check if card is ace
                    if( card.Rank == Card.RankID.Ace )
                        aceCount++;

                    if( p.CardTotal > 21 )
                    {
                        if( aceCount > 0 )
                        {
                            p.CardTotal -= 10;
                            aceCount--;
                        }
                        else
                            p.HandStatus = HandStatusType.Bust;
                    }
                    else if( p.CardTotal == 21 )
                    {
                        //end current hand
                        p.Status = PlayerStatusType.Done;
                        p.HandStatus = HandStatusType.BlackJack;
                        if( p.Name.Equals( "Dealer" ) )
                        {
                            currentPlayer = getPlayer( "Dealer" );
                            foreach( Player player in players )
                            {
                                player.Status = PlayerStatusType.Done;
                                if( player.HandStatus != HandStatusType.BlackJack && !player.isNewPlayer)
                                    player.HandStatus = HandStatusType.Loser;
                            }
                            isRoundFinished = true;
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
                        currentPlayer.HandStatus = HandStatusType.Bust;
                        if( !currentPlayer.Name.Equals( "Dealer" ) )
                        {
                            //mark next player in list to play
                            switchToNextPlayer();
                        }
                    }

                }
                else if( currentPlayer.CardTotal == 21 )
                {
                    currentPlayer.HandStatus = HandStatusType.Winner;
                    //end current hand
                    currentPlayer.Status = PlayerStatusType.Done;
                    if( !currentPlayer.Name.Equals( "Dealer" ) )
                    {
                        //mark next player in list to play
                        switchToNextPlayer();
                    }
                }
            }

            if( currentPlayer.CardsInPlay.Count == 5 )
            {
                //end current hand
                currentPlayer.Status = PlayerStatusType.Done;
                if( !currentPlayer.Name.Equals( "Dealer" ) )
                {
                    //mark next player in list to play
                    switchToNextPlayer();
                }
            }
        }

        private void finishDealerHand()
        {
            isRoundFinished = true;
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
                if( p.Name != "Dealer" && p.isNewPlayer == false )
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
                            p.HandStatus = HandStatusType.Winner;
                            p.Bank += p.CurrentBet * 2;
                            p.CurrentBet = 0;
                        }
                        else
                        {
                            p.HandStatus = HandStatusType.Loser;
                        }
                    }
                }
            }
        }

        private void switchToNextPlayer( )
        {
            currentPlayer.Status = PlayerStatusType.Done;
            if( currentPlayer != players[players.Count - 1] )
            {
                //mark next player in list to play
                for( ; ; )
                {
                    if( players[players.IndexOf( currentPlayer ) + 1].Status == PlayerStatusType.Ready )
                    {
                        players[players.IndexOf( currentPlayer ) + 1].Status = PlayerStatusType.Playing;
                        Console.WriteLine( "Player {0} is now Playing.", players[players.IndexOf( currentPlayer ) + 1].Name );
                        break;
                    }
                    else
                    {
                        currentPlayer = players[players.IndexOf( currentPlayer ) + 1];
                    }
                }

            }

            updateAllClients();

            if( getPlayer( "Dealer" ).Status == PlayerStatusType.Playing )
            {
                currentPlayer = getPlayer( "Dealer" );

                finishDealerHand();
                currentPlayer.Status = PlayerStatusType.Done;
                determinePayouts();
                updateAllClients();
            }
        }

    }
}





