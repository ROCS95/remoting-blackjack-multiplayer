/*Title: BlackJackLibrary
* Page: Game.cs
* Author: Brooke Thrower and Jeramie Hallyburton
* Description: returns information to the client about the game in play
* Created: Mar. 22, 2011
*/
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

        //bool to track if the round is finished
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

        //set up dealer when the game begins
        public Game()
        {
            players.Add( new Player( "Dealer" ) );
            GetPlayer( "Dealer" ).Status = PlayerStatusType.Ready;
        }

        public bool testConnection()
        {
            return true;
        }

        /* Method Name: Join
         * Purpose: adds the player to the game
         * Output: None
         * Input: string name - name of the player being added, callback
         */
        public void Join( string name, ICallback callback )
        {
            foreach( Player p in players )
            {
                //check if the player name exists in the game or not
                if( name.Equals( p.Name ) )
                {
                    throw new Exception( "Player name already exists" );
                }
            }
            //make sure the game is not full
            if( players.Count < 6 )
            {
                Player newPlayer = new Player( name );
                players.Insert( 0, newPlayer );
                callbacks.Add( name, callback );

                //check if the game is in play
                if( isGameInPlay )
                {
                    newPlayer.Status = PlayerStatusType.Done;
                    newPlayer.isNewPlayer = true;
                }
                //report to server
                Console.WriteLine( "Player {0} has just joined the game.", newPlayer.Name );
                updateAllClients();
            }
            else
            {
                //report to server window if player could not join
                Console.WriteLine( "Game is full {0} could not join.", name );
                throw new Exception( "This Game is full" );
            }
        }

        /* Method Name: Ready
         * Purpose: Track if players are ready for the hand and start hand
         * Output: None
         * Input: int bet - players current bet, string name - current players name
         */
        public void Ready( int bet, string name )
        {
            //reset round finished flag
            isRoundFinished = false;

            //check if shoe is low
            if( shoe.NumCards < players.Count * 5 )
                shoe.Shuffle();
            //check dealers status and update
            Player dealer = GetPlayer( "Dealer" );
            if( dealer.Status != PlayerStatusType.Ready )
            {
                dealer.Status = PlayerStatusType.Ready;
                dealer.CardsInPlay.Clear();
            }
            //update current player information
            currentPlayer = GetPlayer( name );
            currentPlayer.Status = PlayerStatusType.Ready;
            currentPlayer.HandStatus = HandStatusType.None;
            currentPlayer.CardsInPlay.Clear();
            currentPlayer.Bank -= bet;
            currentPlayer.CurrentBet = bet;

            //check if players are ready
            foreach( Player p in players )
            {
                if( p.Status != PlayerStatusType.Ready )
                    blnPlayersReady = false;
                //reset each players hand information
                p.HandStatus = HandStatusType.None;
                p.CardsInPlay.Clear();
                p.CardTotal = 0;
            }
            //check if players are ready for the hand
            if( blnPlayersReady )
            {
                players[0].Status = PlayerStatusType.Playing;
                Console.WriteLine( "Player {0} is now Playing.", players[0].Name );
                isGameInPlay = true;
                dealStartingPlayerCards();
            }
            else
            {
                blnPlayersReady = true;
            }
            updateAllClients();
        }

        /* Method Name: Hit
         * Purpose: Adds a card to the current player
         * Output: None
         * Input: string name - current players name
         */
        public void Hit( string name )
        {
            //set the current player to the player passed in
            currentPlayer = GetPlayer( name );
            //record to the server console
            Console.WriteLine( "Player {0} has hit:", name );
            //add a Card to the current players
            dealCards( 1 );
            updateAllClients();
        }

        /* Method Name: Stay
         * Purpose: Switch to next player
         * Output: None
         * Input: string name - current players name
         */
        public void Stay( string name )
        {
            //record to the server console
            Console.WriteLine( "Player {0} stays", name );
            //set the current player to the player passed in
            currentPlayer = GetPlayer( name );
            switchToNextPlayer();
        }

        /* Method Name: DoubleDown
         * Purpose: Add card to hand and end players turn
         * Output: None
         * Input: string name - current players name
         */
        public void DoubleDown( string name )
        {
            //set the current player to the player passed in
            currentPlayer = GetPlayer( name );
            //record to the server console
            Console.WriteLine( "Player: {0} has doubled down:", name );
            //deal a card
            dealCards( 1 );
            //double the bet
            currentPlayer.Bank -= currentPlayer.CurrentBet;
            currentPlayer.CurrentBet *= 2;
            switchToNextPlayer();
        }

        /* Method Name: GetPlayer
         * Purpose: search for the player in the game
         * Output: None
         * Input: string name - current players name
         */
        public Player GetPlayer( string name )
        {
            //search for the player in the list of players
            foreach( Player player in players )
            {
                if( name.Equals( player.Name ) )
                    return player;
            }
            return new Player( null );
        }

        /* Method Name: RemovePlayer
         * Purpose: Remove the player from a game
         * Output: None
         * Input: string name - name of the player being removed
         */
        public void RemovePlayer( string name )
        {
            try
            {
                //make sure the name isnt blank
                if( name != "" )
                {
                    //if the player is the current player then move to the next player
                    if( GetPlayer( name ).Status == PlayerStatusType.Playing )
                    {
                        switchToNextPlayer();
                    }
                    //remove player from callback
                    callbacks.Remove( name );
                    //remove player from list
                    players.Remove( GetPlayer( name ) );
                    //record to the server console
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

        /* Method Name: drawMultiple
         * Purpose: draws a range of random cards
         * Output: None
         * Input: nCards = the number of cards being drawn, string name - name of the player being removed
         */
        private List<Card> drawMultiple( int nCards, string name )
        {
            List<Card> cards = new List<Card>();
            //draw the range of cards and add to list
            for( int i = 0; i < nCards; i++ )
            {
                cards.Add( shoe.Draw() );
                //record to the server console
                Console.WriteLine( "Deal: {0} to {1}", cards[i].Name, GetPlayer( name ).Name );
            }
            return cards;
        }

        /* Method Name: switchToNextPlayer
         * Purpose: Remove the player from a game
         * Output: None
         * Input: None
         */
        private void switchToNextPlayer()
        {
            currentPlayer.Status = PlayerStatusType.Done;
            if( currentPlayer != players[players.Count - 1] )
            {
                //infinate loop to, mark next player in list to play //should always break out at as dealer is last and will always be ready
                for( ; ; )
                {
                    if( players[players.IndexOf( currentPlayer ) + 1].Status == PlayerStatusType.Ready || players[players.IndexOf( currentPlayer ) + 1].Name.Equals( "Dealer" ) )
                    {
                        players[players.IndexOf( currentPlayer ) + 1].Status = PlayerStatusType.Playing;
                        //record to the server console
                        Console.WriteLine( "Player {0} is now Playing.", players[players.IndexOf( currentPlayer ) + 1].Name );
                        break;
                    }
                    else
                    {
                        //set next player to be the current player
                        currentPlayer = players[players.IndexOf( currentPlayer ) + 1];
                    }
                }
            }
            updateAllClients();
            //finish the dealer hand if they are the only player left
            if( GetPlayer( "Dealer" ).Status == PlayerStatusType.Playing )
            {
                currentPlayer = GetPlayer( "Dealer" );
                finishDealerHand();
                currentPlayer.Status = PlayerStatusType.Done;
                determinePayouts();
                updateAllClients();
            }
        }

        /* Method Name: dealStartingPlayerCards
         * Purpose: deal the first to cards to all the players
         * Output: None
         * Input: None
         */
        private void dealStartingPlayerCards()
        {
            bool isPlayersDone = true;
            foreach( Player p in players )
            {
                //add cards
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
                    //check if card total is over 21(not technically possible)
                    if( p.CardTotal > 21 )
                    {
                        //if there is an ace then set the count to -10 to set ace value to 1
                        if( aceCount > 0 )
                        {
                            p.CardTotal -= 10;
                            aceCount--;
                        }
                        else
                            //set player to bust if they are over
                            p.HandStatus = HandStatusType.Bust;
                    }
                    else if( p.CardTotal == 21 )
                    {
                        //end players hands
                        p.Status = PlayerStatusType.Done;
                        p.HandStatus = HandStatusType.BlackJack;
                    }
                }
                if( p.Status != PlayerStatusType.Done && !p.Name.Equals("Dealer") && !p.isNewPlayer )
                    isPlayersDone = false;    
            }
            if( isPlayersDone )
            {
                currentPlayer = GetPlayer( "Dealer" );
                finishDealerHand();
            }
        }

        /* Method Name: dealCards
         * Purpose: deal a set number of cards to the players
         * Output: None
         * Input: None
         */
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

                //check for possible bust
                if( currentPlayer.CardTotal > 21 )
                {
                    //if card is an ace then set its value to 1
                    if( card.Rank == Card.RankID.Ace )
                    {
                        currentPlayer.CardTotal -= 10;
                    }
                    else
                    {
                        //player bust, move to next player unless its the dealer
                        currentPlayer.Status = PlayerStatusType.Done;
                        currentPlayer.HandStatus = HandStatusType.Bust;
                        if( !currentPlayer.Name.Equals( "Dealer" ) )
                        {
                            switchToNextPlayer();
                        }
                    }

                }
                else if( currentPlayer.CardTotal == 21 )
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

            //can't hit past 5 cards
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

        /* Method Name: finishDealerHand
         * Purpose: Finish the dealers hand
         * Output: None
         * Input: None
         */
        private void finishDealerHand()
        {
            //set round to done
            isRoundFinished = true;
            //add a card to the dealers hand if under 17
            if( currentPlayer.CardTotal < 17 )
            {
                dealCards( 1 );
                finishDealerHand();
            }
        }

        /* Method Name: determinePayouts
         * Purpose: determine results for the round
         * Output: None
         * Input: None
         */
        private void determinePayouts()
        {
            //Determine Payouts
            //currentPlayer is always the dealer
            foreach( Player p in players )
            {
                //dont update dealer or inactive(waiting) players
                //update bank based on handStatus and rest bet
                if( p.Name != "Dealer" && p.isNewPlayer == false )
                {
                    if( currentPlayer.HandStatus == HandStatusType.BlackJack && p.HandStatus != HandStatusType.BlackJack )
                    {
                        p.HandStatus = HandStatusType.Loser;
                        p.CurrentBet = 0;
                    }
                    else
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
                //ensures no negative bank values
                if( p.Bank < 0 )
                    p.Bank = 0;
                //set new players to playing for next round
                if( p.isNewPlayer )
                    p.isNewPlayer = false;
            }
        }

        /* Method Name: updateAllClients
         * Purpose: update all clients with player information
         * Output: None
         * Input: None
         */
        private void updateAllClients()
        {
            //loop through each player to update
            foreach( Player player in players )
                if( !player.Name.Equals( "Dealer" ) )
                    callbacks[player.Name].PlayerUpdate( players );
        }

    }
}
