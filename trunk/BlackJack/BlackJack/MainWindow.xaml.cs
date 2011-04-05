/*Title: BlackJackClient
* Page: MainWindow.xaml.cs
* Author: Brooke Thrower and Jeramie Hallyburton
* Description: The Client used to display a GUI for users to play a basic BlackJack game
* Uses: BackJackLibrary.cs
* Created: Mar. 22, 2011
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BlackJackLibrary;
using System.Runtime.Remoting;
namespace BlackJackClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Game game;
        private bool showClosingMsg = true;
        private bool connected = false;
        public MainWindow()
        {
            InitializeComponent();

            connectToServer();
        }

        private void connectToServer()
        {
            string address = "";

            //display a window to get the address of the server before beginning the game
            IPWindow ipWnd = new IPWindow();
            ipWnd.ShowDialog();

            if( ipWnd.DialogResult == true )
            {
                address = ipWnd.Address;

                try
                {
                    // Load the remoting configuration file
                    RemotingConfiguration.Configure( "BlackJackClient.exe.config", false );

                    // Activate a game object
                    game = ( Game )Activator.GetObject( typeof( Game ), "tcp://" + address + ":12222/game.binary" );

                    connected = game.testConnection();

                    lblStatus.Content = "Please Enter a Name and Click Join";

                }
                catch( Exception )
                {
                    MessageBox.Show( "Could not connect to the server! Please try again later..", "Cannot Connect!", MessageBoxButton.OK, MessageBoxImage.Error );
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }
        }

        //Join the Game on button click
        private void btnJoin_Click( object sender, RoutedEventArgs e )
        {
            //validate that the user put in a name
            if( txtJoin.Text.Equals( "" ) )
            {
                MessageBox.Show( "Please Enter a Username" );
            }
            else
            {
                try
                {
                    //try to join the game
                    game.Join( txtJoin.Text, new Callback( this ) );
                    //get the Bank information of the client and update the window information
                    txtBank.Text = Convert.ToString( game.GetPlayer(txtJoin.Text).Bank );
                    txtJoin.IsEnabled = false;
                    btnJoin.IsEnabled = false;
                    txtBid.IsEnabled = true;
                }
                catch( Exception ex )
                {
                    //determine if the game is full
                    if( ex.Message.Equals("This Game is full") )
                    {
                        if( MessageBox.Show( "The current game is full! Please try again later..", "Full Game!", MessageBoxButton.OK, MessageBoxImage.Error ) == MessageBoxResult.OK )
                        {
                            showClosingMsg = false;
                            this.Close();
                        }
                    }
                    //determine if the name being used exists
                    else if( ex.Message.Equals("Player name already exists" ))
                    {
                        txtJoin.Text = "";
                        MessageBox.Show( "Player name already exists, please choose a different name.", "Name Exists", MessageBoxButton.OK, MessageBoxImage.Error );
                    }
                }
                
            }
        }

        //validate bid on text change
        private void txtBid_TextChanged( object sender, TextChangedEventArgs e )
        {
            //validate that the value is a number with no decimal places
            int bet;
            if( int.TryParse( txtBid.Text, out bet ) )
            {
                if( bet < Convert.ToInt32( txtBank.Text ) && bet > 0 )
                    btnReady.IsEnabled = true;
            }
        }

        //Set player to ready on button click
        private void btnReady_Click( object sender, RoutedEventArgs e )
        {
            //make sure the bet is not greater then the current bank
            if( Convert.ToInt32(txtBank.Text) < Convert.ToInt32(txtBid.Text))
            {
                lblStatus.Content = "You Must Place a Bet Lower Than Your Bank!";
            }
            else if( Convert.ToInt32(txtBid.Text) <= 0)
            {
                lblStatus.Content = "You Must Place a Bet Higher Than Zero!";
            }
            else
            {
                //clear all cards
                clearCards();
                //set player to the ready status and update the client information
                game.Ready( Convert.ToInt32( txtBid.Text ), txtJoin.Text );
                txtBank.Text = Convert.ToString( game.GetPlayer( txtJoin.Text ).Bank );
            }
        }

        //add a card to the current players hand on button click
        private void btnHit_Click( object sender, RoutedEventArgs e )
        {
            //add a card to the current hand
            game.Hit( txtJoin.Text );
        }

        //Do not allow the player to do anything on button click
        private void btnStay_Click( object sender, RoutedEventArgs e )
        {
            //set player to done hand and move to next player
            game.Stay( txtJoin.Text );
        }

        //add a card and do not allow the player to do anything on button click
        private void btnDoubleDown_Click( object sender, RoutedEventArgs e )
        {
            //add a card, set current player to done and move to next player
            game.DoubleDown( txtJoin.Text );
        }

        //determine what to do when closing the window
        private void Window_Closing( object sender, System.ComponentModel.CancelEventArgs e )
        {
            if( connected )
            {
                if( showClosingMsg )
                {
                    //make sure the player wants to leave the game
                    if( MessageBox.Show( "Are you sure you want to quit?", "Closing", MessageBoxButton.YesNo, MessageBoxImage.Question ) == MessageBoxResult.No )
                    {
                        //cancel the closing if they don't want to leave
                        e.Cancel = true;
                        return;
                    }
                }
                //remove the player from the game
                game.RemovePlayer( txtJoin.Text );
            }
        }


        /* Method Name: clearCards
         * Purpose: Removes all cards displayed on the screen
         * Output: None
         * Input: None
         */
        private void clearCards()
        {
            //loop through 6 times to clear each player containers cards
            for( int i = 1; i != 6; ++i )
            {
                ( mainHand.FindName( "card" + i ) as ucCardContainer ).Clear();
                ( DealerHand.FindName( "card" + i ) as ucSmallCardContainer ).Clear();
                ( PlayerContainer1.FindName( "card" + i ) as ucSmallCardContainer ).Clear();
                ( PlayerContainer2.FindName( "card" + i ) as ucSmallCardContainer ).Clear();
                ( PlayerContainer3.FindName( "card" + i ) as ucSmallCardContainer ).Clear();
                ( PlayerContainer4.FindName( "card" + i ) as ucSmallCardContainer ).Clear();
            }
        }

        /* Method Name: hideWindows
         * Purpose: hides all "windows" or (player information containers) on the current window ( deal hand, other players hands, main hand)
         * Output: None
         * Input: None
         */
        private void hideWindows()
        {
            //set all contaioners visibility to hidden
            DealerHand.Visibility = Visibility.Hidden;
            PlayerContainer1.Visibility = Visibility.Hidden;
            PlayerContainer2.Visibility = Visibility.Hidden;
            PlayerContainer3.Visibility = Visibility.Hidden;
            PlayerContainer4.Visibility = Visibility.Hidden;
            mainHand.Visibility = Visibility.Hidden;

        }

        /* Method Name: finishGame
         * Purpose: Update the dealer hand to display the correct total and the first card value
         * Output: None
         * Input: None
         */
        private void finishGame()
        {
            //flip the first card and display the dealer total
            DealerHand.card1.SetCard( game.GetPlayer( "Dealer" ).CardsInPlay[0] );
            DealerHand.lblDealerCount.Content = game.GetPlayer( "Dealer" ).CardTotal;
            //close the game if they  run out of money
            if( game.GetPlayer( txtJoin.Text ).Bank == 0 )
            {
                MessageBox.Show( "You have ran out of money. \n Thank you for Playing.", "Out of Money", MessageBoxButton.OK, MessageBoxImage.Exclamation );
                showClosingMsg = false;
                this.Close();
            }
        }

        /* Method Name: updateClientWindow
         * Purpose: Updates all ClientWindows with the latest information
         * Output: None
         * Input: None
         */
        private delegate void ClientUpdateDelegate( List<Player> players );

        public void UpdateClientWindow( List<Player> players )
        {
            txtJoin.Dispatcher.BeginInvoke( new ClientUpdateDelegate( updateClientWindow ), players );
        }

        private void updateClientWindow( List<Player> players )
        {
            //start with blank screen
            clearCards();
            hideWindows();

            int otherPlayerCount = 0;
            ucOtherPlayerHand otherPlayerHand;

            //loop through each player to update appropriately
            foreach( Player player in players )
            {
                //update main window
                if( player.Name.Equals( txtJoin.Text ) )
                {
                    mainHand.Visibility = Visibility.Visible;
                    //display cards on the screen
                    for( int i = 0; i != player.CardsInPlay.Count; ++i )
                    {
                        ( mainHand.FindName( "card" + ( i + 1 ) ) as ucCardContainer ).SetCard( player.CardsInPlay[i] );
                    }
                    //only display the count if it has been set
                    if( player.CardTotal > 0 )
                        mainHand.lblCount.Content = player.CardTotal;
                    else
                        mainHand.lblCount.Content = "";
                    //disable and enable buttons based on players status
                    if( player.Status == PlayerStatusType.Playing )
                    {
                        lblStatus.Content = "\nYour Turn";
                        btnHit.IsEnabled = true;
                        btnStay.IsEnabled = true;
                        if(player.CardsInPlay.Count < 3)
                            btnDoubleDown.IsEnabled = true;
                        else
                            btnDoubleDown.IsEnabled = false;
                        btnReady.IsEnabled = false;
                        txtBid.IsEnabled = false;
                    }
                    else if (player.Status == PlayerStatusType.Done && player.isNewPlayer)
                    {
                        btnHit.IsEnabled = false;
                        btnStay.IsEnabled = false;
                        btnDoubleDown.IsEnabled = false;
                        txtBid.IsEnabled = false;
                    }
                    else if( player.Status == PlayerStatusType.Done && !player.isNewPlayer )
                    {
                        btnHit.IsEnabled = false;
                        btnStay.IsEnabled = false;
                        btnDoubleDown.IsEnabled = false;
                        if( game.IsRoundFinished )
                        {
                            txtBid.IsEnabled = true;
                            btnReady.IsEnabled = true;
                        }
                    }
                    else if( player.Status == PlayerStatusType.Betting )
                    {
                        btnHit.IsEnabled = false;
                        btnStay.IsEnabled = false;
                        btnDoubleDown.IsEnabled = false;
                        txtBid.IsEnabled = true;
                        txtBid.Background = Brushes.Yellow;
                        lblStatus.Content = "\nPlease Place Your Bet";
                        if( txtBid.Text == "" || txtBid.Text == null )
                            btnReady.IsEnabled = false;
                        else
                            btnReady.IsEnabled = true;
                    }
                    else
                    {
                        btnHit.IsEnabled = false;
                        btnStay.IsEnabled = false;
                        btnDoubleDown.IsEnabled = false;
                        txtBid.IsEnabled = false;
                        btnReady.IsEnabled = false;

                    }
                    //if the player hasn't joined mid session update the labels
                    if( !player.isNewPlayer )
                    {
                        //display their results of the hand if its been set
                        switch( player.HandStatus )
                        {
                            case HandStatusType.BlackJack:
                                lblStatus.Content = "\nBlackJack!\nPlease Place Your Next Bet";
                                break;
                            case HandStatusType.Bust:
                                lblStatus.Content = "\nSorry, You Bust\nPlease Place Your Next Bet";
                                break;
                            case HandStatusType.Winner:
                                lblStatus.Content = "\nWinner, Winner, Chicken Dinner!\nPlease Place Your Next Bet";
                                break;
                            case HandStatusType.Loser:
                                lblStatus.Content = "\nSorry, Dealer Wins\nPlease Place Your Next Bet";
                                break;
                            case HandStatusType.Push:
                                lblStatus.Content = "\nPush\nPlease Place Your Next Bet";
                                break;
                            default:
                                if( player.Status != PlayerStatusType.Playing && player.Status != PlayerStatusType.Betting )
                                    lblStatus.Content = "";
                                break;
                        }
                    }
                    else
                    {
                        //inform new players that the game is in progress
                        lblStatus.Content = "Game Currently in Progress...\nPlease Wait Until the Next Round";
                    }
                    
                    //update the players bank
                    txtBank.Text = player.Bank.ToString();
                }
                    //update all clients dealer information
                else if( player.Name.Equals( "Dealer" ) )
                {
                    DealerHand.Visibility = Visibility.Visible;
                    DealerHand.lblDealerCount.Content = "";
                    //update dealer cards displayed
                    for( int i = 0; i != player.CardsInPlay.Count; ++i )
                    {
                        if( i == 0 )
                        {
                            ( DealerHand.FindName( "card" + ( i + 1 ) ) as ucSmallCardContainer ).SetCard( player.CardsInPlay[i], true );
                        }
                        else
                        {
                            ( DealerHand.FindName( "card" + ( i + 1 ) ) as ucSmallCardContainer ).SetCard( player.CardsInPlay[i] );
                        }
                    }
                }
                    //update other clients windows with current players information
                else
                {
                    ++otherPlayerCount;
                    //determine what container to update and update it
                    otherPlayerHand = ( this.FindName( "PlayerContainer" + otherPlayerCount ) ) as ucOtherPlayerHand;
                    otherPlayerHand.Visibility = Visibility.Visible;
                    otherPlayerHand.lblPlrName.Content = player.Name;
                    otherPlayerHand.lblBank.Content = player.Bank;
                    otherPlayerHand.lblBid.Content = player.CurrentBet;
                    //set the playerss name to aqua if they are the current player
                    if( player.Status == PlayerStatusType.Playing )
                    {
                        otherPlayerHand.lblPlrName.Foreground = Brushes.Aqua;
                    }
                    else
                    {
                        otherPlayerHand.lblPlrName.Foreground = Brushes.White;
                    }
                        
                    //diplay the cards on the screen
                    for( int i = 0; i != player.CardsInPlay.Count; ++i )
                    {
                        ( otherPlayerHand.FindName( "card" + ( i + 1 ) ) as ucSmallCardContainer ).SetCard( player.CardsInPlay[i] );
                    }
                }
            }
            //check if the game is finished
            if( game.IsRoundFinished )
                finishGame();
        }
    }
}