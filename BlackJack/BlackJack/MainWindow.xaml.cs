/*
 *Hey Jer! Got most done tonight But Im getting tired and lossing focus haha so I pass this of to you for now 
 * I have betting, player turns ect working atm.  But We need to figure out how to start a new game
 * We also need to do popups to tell the player what if the won or not, i already do this if the get BlackJack though
 * as it is not an end game thing.  I added a HandStatus to the player to set if their hand was a winner loser ect
 * so we could use that to determine what message to pop up I am just unsure at the moment the best way to do it.  Another smallish thing
 * will be to figure out what to do when we hit 5 cards... I looked it up and apparently its not a common rule un BlackJack to win
 * if you hit 5 but we can make it a rule in ours so its easier.Also, I rewrote a bunch of your callback stuff so we dount need to use 
 * the Lists but I left your code there for not just in case you need it.
 * I think thats about everything, I didnt get a change to do the design doc yet either, or many comments. But we can figure something out for that
 * Text me tomorrow if you have any questions.  I hope what I did is okay for now. 
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
namespace BlackJack
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Game game;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                // Load the remoting configuration file
                RemotingConfiguration.Configure( "BlackJackClient.exe.config", false );

                // Activate a game object
                game = ( Game )Activator.GetObject( typeof( Game ), "tcp://localhost:12222/game.binary" );

            }
            catch( Exception ex )
            {
                MessageBox.Show( ex.Message );
            }
        }


        private void btnJoin_Click( object sender, RoutedEventArgs e )
        {
            if( txtJoin.Text.Equals( "" ) )
            {
                MessageBox.Show( "Please Enter a Username" );
            }
            else
            {
                game.Join( txtJoin.Text, new Callback( this ) );
                txtBank.Text = Convert.ToString( game.getPlayer(txtJoin.Text).Bank );
                txtJoin.IsEnabled = false;
                btnJoin.IsEnabled = false;
                txtBid.IsEnabled = true;
            }
        }

        private void btnReady_Click( object sender, RoutedEventArgs e )
        {
            //clear all cards
            clearCards();
            game.Ready( Convert.ToInt32( txtBid.Text ), txtJoin.Text );
            txtBank.Text = Convert.ToString( game.getPlayer( txtJoin.Text ).Bank );          
        }

        private void clearCards()
        {
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

        private void btnHit_Click( object sender, RoutedEventArgs e )
        {
            game.Hit( txtJoin.Text );
        }

        private void btnStay_Click( object sender, RoutedEventArgs e )
        {
            game.Stay( txtJoin.Text );
            btnHit.IsEnabled = false;
            btnStay.IsEnabled = false;
            btnDoubleDown.IsEnabled = false;
        }

        private void btnDoubleDown_Click( object sender, RoutedEventArgs e )
        {
            game.DoubleDown( txtJoin.Text);
            txtBid.Text = game.getPlayer( txtJoin.Text ).CurrentBet.ToString();
            btnHit.IsEnabled = false;
            btnStay.IsEnabled = false;
            btnDoubleDown.IsEnabled = false;
            //finishDealersHand();

        }

        private delegate void ClientUpdateDelegate( List<Player> players );

        public void UpdateClientWindow( List<Player> players )
        {
            txtJoin.Dispatcher.BeginInvoke( new ClientUpdateDelegate( updateClientWindow ), players );
        }

        private void updateClientWindow( List<Player> players )
        {

            clearCards();

            int otherPlayerCount = 0;
            ucOtherPlayerHand otherPlayerHand;
            bool blnAllPlayersDone = true;

            foreach( Player player in players )
            {
                if( player.Name.Equals( txtJoin.Text ) )
                {
                    for( int i = 0; i != player.CardsInPlay.Count; ++i )
                    {
                        ( mainHand.FindName( "card" + ( i + 1 ) ) as ucCardContainer ).SetCard( player.CardsInPlay[i] );
                    }
                    if( player.CardTotal > 0 )
                        mainHand.lblCount.Content = player.CardTotal;
                    if( player.Status == PlayerStatusType.Playing )
                    {
                        btnHit.IsEnabled = true;
                        btnStay.IsEnabled = true;
                        btnDoubleDown.IsEnabled = true;
                        btnReady.IsEnabled = false;
                    }
                    else if (player.Status == PlayerStatusType.Waiting)
                    {
                        btnHit.IsEnabled = false;
                        btnStay.IsEnabled = false;
                        btnDoubleDown.IsEnabled = false;
                    }
                    else
                    {
                        btnHit.IsEnabled = false;
                        btnStay.IsEnabled = false;
                        btnDoubleDown.IsEnabled = false;
                        btnReady.IsEnabled = !String.IsNullOrEmpty( txtBid.Text );
                    }

                    if( player.HandStatus == HandStatusType.BlackJack )
                    {
                        MessageBox.Show( "BLACKJACK!" );
                    }
                    else if( player.HandStatus == HandStatusType.Bust )
                    {
                        MessageBox.Show( "You Bust!" );
                    }
                    txtBank.Text = player.Bank.ToString();
                }
                else if( player.Name.Equals( "Dealer" ) )
                {
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
                else
                {
                    ++otherPlayerCount;
                    otherPlayerHand = ( this.FindName( "PlayerContainer" + otherPlayerCount ) ) as ucOtherPlayerHand;
                    otherPlayerHand.Visibility = Visibility.Visible;
                    otherPlayerHand.lblPlrName.Content = player.Name;
                    otherPlayerHand.lblBank.Content = player.Bank;
                    otherPlayerHand.lblBid.Content = player.CurrentBet;

                    for( int i = 0; i != player.CardsInPlay.Count; ++i )
                    {
                        ( otherPlayerHand.FindName( "card" + ( i + 1 ) ) as ucSmallCardContainer ).SetCard( player.CardsInPlay[i] );
                    }
                }

                //Check for End of Game
                if( player.Status != PlayerStatusType.Done )
                {
                    blnAllPlayersDone = false;
                }
            }

            if( blnAllPlayersDone )
                finishGame();
        }

        private void finishGame()
        {
            DealerHand.card1.SetCard( game.getPlayer("Dealer").CardsInPlay[0]);
            DealerHand.lblDealerCount.Content = game.getPlayer( "Dealer" ).CardTotal;
        }

        private void txtBid_TextChanged( object sender, TextChangedEventArgs e )
        {
            int bet;
            if( int.TryParse( txtBid.Text, out bet ) )
                btnReady.IsEnabled = true;
        }
    }
}