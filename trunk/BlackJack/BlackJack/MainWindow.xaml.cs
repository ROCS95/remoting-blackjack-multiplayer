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
        public MainWindow()
        {
            InitializeComponent();

            string address = "";

            IPWindow ipWnd = new IPWindow();
            ipWnd.ShowDialog();

            if( ipWnd.DialogResult == true )
            {
                address = ipWnd.Address;
            }
            else
            {
                this.Close();
            }

            try
            {
                // Load the remoting configuration file
                RemotingConfiguration.Configure( "BlackJackClient.exe.config", false );

                // Activate a game object
                game = ( Game )Activator.GetObject( typeof( Game ), "tcp://"+ address +":12222/game.binary" );

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
                try
                {
                    game.Join( txtJoin.Text, new Callback( this ) );
                    txtBank.Text = Convert.ToString( game.getPlayer(txtJoin.Text).Bank );
                    txtJoin.IsEnabled = false;
                    btnJoin.IsEnabled = false;
                    txtBid.IsEnabled = true;
                }
                catch( Exception ex )
                {
                    if( ex.Message.Equals("This Game is full") )
                    {
                        if( MessageBox.Show( "The current game is full! Please try again later..", "Full Game!", MessageBoxButton.OK, MessageBoxImage.Error ) == MessageBoxResult.OK )
                        {
                            showClosingMsg = false;
                            this.Close();
                        }
                    }
                    else if( ex.Message.Equals("Player name already exists" ))
                    {
                        txtJoin.Text = "";
                        MessageBox.Show( "Player name already exists, please choose a different name.", "Name Exists", MessageBoxButton.OK, MessageBoxImage.Error );
                    }
                }
                
            }
        }

        private void btnReady_Click( object sender, RoutedEventArgs e )
        {
            if( Convert.ToInt32(txtBank.Text) < Convert.ToInt32(txtBid.Text))
            {
                lblStatus.Content = "You must place a bet lower then your bank!";
            }
            else
            {
                //clear all cards
                clearCards();
                game.Ready( Convert.ToInt32( txtBid.Text ), txtJoin.Text );
                txtBank.Text = Convert.ToString( game.getPlayer( txtJoin.Text ).Bank );
                btnReady.IsEnabled = false;
                btnDoubleDown.IsEnabled = true;
            }
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
            btnDoubleDown.IsEnabled = false;
        }

        private void btnStay_Click( object sender, RoutedEventArgs e )
        {
            game.Stay( txtJoin.Text );
            btnHit.IsEnabled = false;
            btnStay.IsEnabled = false;
            btnDoubleDown.IsEnabled = false;
        }

        private void hideWindows()
        {
            DealerHand.Visibility = Visibility.Hidden;
            PlayerContainer1.Visibility = Visibility.Hidden;
            PlayerContainer2.Visibility = Visibility.Hidden;
            PlayerContainer3.Visibility = Visibility.Hidden;
            PlayerContainer4.Visibility = Visibility.Hidden;
            mainHand.Visibility = Visibility.Hidden;

        }

        private void btnDoubleDown_Click( object sender, RoutedEventArgs e )
        {
            game.DoubleDown( txtJoin.Text);
            txtBid.Text = game.getPlayer( txtJoin.Text ).CurrentBet.ToString();
            btnHit.IsEnabled = false;
            btnStay.IsEnabled = false;
            btnDoubleDown.IsEnabled = false;
        }

        private delegate void ClientUpdateDelegate( List<Player> players );

        public void UpdateClientWindow( List<Player> players )
        {
            txtJoin.Dispatcher.BeginInvoke( new ClientUpdateDelegate( updateClientWindow ), players );
        }

        private void updateClientWindow( List<Player> players )
        {

            clearCards();
            hideWindows();

            int otherPlayerCount = 0;
            ucOtherPlayerHand otherPlayerHand;

            foreach( Player player in players )
            {
                if( player.Name.Equals( txtJoin.Text ) )
                {
                    mainHand.Visibility = Visibility.Visible;
                    for( int i = 0; i != player.CardsInPlay.Count; ++i )
                    {
                        ( mainHand.FindName( "card" + ( i + 1 ) ) as ucCardContainer ).SetCard( player.CardsInPlay[i] );
                    }
                    if( player.CardTotal > 0 )
                        mainHand.lblCount.Content = player.CardTotal;
                    else
                        mainHand.lblCount.Content = "";
                    if( player.Status == PlayerStatusType.Playing && player.CardsInPlay.Count < 3 )
                    {
                        btnHit.IsEnabled = true;
                        btnStay.IsEnabled = true;
                        btnDoubleDown.IsEnabled = true;
                        btnReady.IsEnabled = false;
                        txtBid.IsEnabled = true;
                    }
                    else if (player.Status == PlayerStatusType.Done && player.isNewPlayer)
                    {
                        btnHit.IsEnabled = false;
                        btnStay.IsEnabled = false;
                        btnDoubleDown.IsEnabled = false;
                        txtBid.IsEnabled = false;
                    }
                    else
                    {
                        btnHit.IsEnabled = false;
                        btnStay.IsEnabled = false;
                        btnDoubleDown.IsEnabled = false;
                        txtBid.IsEnabled = true;
                        btnReady.IsEnabled = !String.IsNullOrEmpty( txtBid.Text ) && player.Status != PlayerStatusType.Ready;
                    }

                    switch (player.HandStatus)
                    {
                        case HandStatusType.BlackJack:
                            lblStatus.Content = "BlackJack!";
                            break;
                        case HandStatusType.Bust:
                            lblStatus.Content = "You Bust, Sorry!";
                            if( player.Bank == 0 )
                            {
                                if( MessageBox.Show( "You have ran out of money.  Thank you for Playing.", "Out of Money!", MessageBoxButton.OK, MessageBoxImage.Exclamation ) == MessageBoxResult.OK)
                                {
                                    showClosingMsg = false;
                                    this.Close();
                                }
                            }
                            break;
                        case HandStatusType.Winner:
                            lblStatus.Content = "You Win This Hand!";
                            break;
                        case HandStatusType.Loser:
                            lblStatus.Content = "You Lose, Sorry!";
                            if( player.Bank == 0 )
                            {
                                if( MessageBox.Show( "You have ran out of money.  Thank you for Playing.", "Out of Money!", MessageBoxButton.OK, MessageBoxImage.Exclamation ) == MessageBoxResult.OK )
                                {
                                    showClosingMsg = false;
                                    this.Close();
                                }
                            }
                            break;
                        case HandStatusType.Push:
                            lblStatus.Content = "Push, Nobody wins";
                            break;
                        default:
                            if( player.isNewPlayer )
                                lblStatus.Content = "Game currently in progress. Please wait until next round";
                            else
                                lblStatus.Content = "";
                            break;
                    }
                    
                    txtBank.Text = player.Bank.ToString();
                }
                else if( player.Name.Equals( "Dealer" ) )
                {
                    DealerHand.Visibility = Visibility.Visible;
                    DealerHand.lblDealerCount.Content = "";
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
            }

            if( game.IsRoundFinished )
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
            {
                if( bet < Convert.ToInt32( txtBank.Text ) )
                    btnReady.IsEnabled = true;
            }
        }

        private void Window_Closing( object sender, System.ComponentModel.CancelEventArgs e )
        {
            if( showClosingMsg )
            {
                if( MessageBox.Show( "Are you sure you want to quit?", "Closing", MessageBoxButton.YesNo, MessageBoxImage.Question ) == MessageBoxResult.No )
                {
                    return;
                }
            }
            game.removePlayer( txtJoin.Text );
            this.Close();
        }
    }
}