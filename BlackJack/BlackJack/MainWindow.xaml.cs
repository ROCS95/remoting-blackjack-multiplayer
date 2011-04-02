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
        int plrCount = 0; 
        int dlrCount = 0;
        Card dlrSecondCard;
        bool isAce = false;

        List<ucSmallCardContainer> dealerContainers = new List<ucSmallCardContainer>();
        List<ucCardContainer> playerContainers = new List<ucCardContainer>();

        // Create a reference to a blackjack Game object
        private Game game;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                // Load the remoting configuration file
                RemotingConfiguration.Configure("BlackJackClient.exe.config", false);

                // Activate a game object
                game = (Game)Activator.GetObject(typeof(Game), "tcp://localhost:12222/game.binary");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
  
            btnReady.IsEnabled = false;
            btnHit.IsEnabled = false;
            btnStay.IsEnabled = false;

            //add card containers to list
            playerContainers.Add( mainHand.card1 );
            playerContainers.Add( mainHand.card2 );
            playerContainers.Add( mainHand.card3 );
            playerContainers.Add( mainHand.card4 );
            playerContainers.Add( mainHand.card5 );

            dealerContainers.Add( DealerHand.cDlrCrd1 );
            dealerContainers.Add( DealerHand.cDlrCrd2 );
            dealerContainers.Add( DealerHand.cDlrCrd3 );
            dealerContainers.Add( DealerHand.cDlrCrd4 );
            dealerContainers.Add( DealerHand.cDlrCrd5 );

        }


        private void btnJoin_Click( object sender, RoutedEventArgs e )
        {
            if( txtJoin.Text.Equals( "" ) )
            {
                MessageBox.Show( "Please Enter a User Name" );
            }
            else
            {
                game.Join(txtJoin.Text, new Callback(this));
                lblPlayerName.Content = txtJoin.Text;
                txtJoin.IsEnabled = false;
                btnReady.IsEnabled = true;
            }
        }

        private void btnReady_Click( object sender, RoutedEventArgs e )
        {
            //clear all cards
            clearCards();
            plrCount = 0;
            dlrCount = 0;
            mainHand.lblCount.Content = plrCount;
            DealerHand.lblDealerCount.Content = dlrCount;

            game.Ready();

            //plrCount += card.Value + card2.Value;

            ////set dealers hand
            //Card dlrCard = shoe.Draw();
            //DealerHand.cDlrCrd1.SetCard( dlrCard, false );
            //dlrSecondCard = shoe.Draw();
            //DealerHand.cDlrCrd2.SetCard( dlrSecondCard, true );
            //DealerHand.lblDealer.Visibility = Visibility.Visible;

            //dlrCount = dlrCard.Value;
            //DealerHand.lblDealerCount.Content = dlrCount;

            ////check if card is ace
            //if( card.Rank == Card.RankID.Ace )
            //    isAce = true;
            //if( plrCount > 21 && isAce )
            //{
            //    plrCount -= 10;
            //    isAce = false;
            //}
            //else if( plrCount == 21 )
            //{
            //    MessageBox.Show( "Blackjack!" );
            //    btnReady.IsEnabled = true;
            //    btnHit.IsEnabled = false;
            //    btnStay.IsEnabled = false;

            //    finishDealersHand();
            //}

            ////if( card.Value == card2.Value && card.Rank == card2.Rank )
            ////{
            ////    btnSplit.IsEnabled = true;
            ////}

            ////btnSplit.IsEnabled = true;

            //mainHand.lblCount.Content = plrCount;
            btnReady.IsEnabled = false;
            btnHit.IsEnabled = true;
            btnStay.IsEnabled = true;
            btnDoubleDown.IsEnabled = true;
        }

        private void clearCards()
        {
            foreach( ucCardContainer uc in playerContainers )
            {
                uc.Clear();
            }

            foreach( ucSmallCardContainer uc in dealerContainers )
            {
                uc.Clear();
            }
        }

        private void btnHit_Click( object sender, RoutedEventArgs e )
        {
            game.Hit();
            //plrCardHit( plrContainers );
        }

        private void btnStay_Click( object sender, RoutedEventArgs e )
        {
            btnHit.IsEnabled = false;
            btnStay.IsEnabled = false;

            finishDealersHand();
            btnReady.IsEnabled = true;
        }

        void plrCardHit( List<ucCardContainer> containers )
        {
            ////check if card is ace
            //if( card.Rank == Card.RankID.Ace )
            //    isAce = true;
            //if( plrCount > 21 )
            //{
            //    if( isAce )
            //    {
            //        plrCount -= 10;
            //        isAce = false;
            //    }
            //    else
            //    {
            //        mainHand.lblCount.Content = plrCount;
            //        MessageBox.Show( "You bust!" );
            //        btnHit.IsEnabled = false;
            //        btnStay.IsEnabled = false;
            //        finishDealersHand();
            //    }
            //}
            //mainHand.lblCount.Content = plrCount;
        }

        void dlrCardHit( List<ucSmallCardContainer> containers )
        {
            //Card card = shoe.Draw();
            //dlrCount += card.Value;

            ////check which container to put card in
            //foreach( ucSmallCardContainer uc in containers )
            //{
            //    if( !uc.IsSet )
            //    {
            //        uc.SetCard( card, false );
            //        break;
            //    }
            //}

            ////check if card is ace
            //if( card.Rank == Card.RankID.Ace )
            //    isAce = true;
            //if( dlrCount > 21 )
            //{
            //    if( isAce )
            //    {
            //        dlrCount -= 10;
            //        DealerHand.lblDealerCount.Content = dlrCount;
            //        isAce = false;
            //    }
            //    else
            //    {
            //        DealerHand.lblDealerCount.Content = dlrCount;
            //        MessageBox.Show( "Dealer busts!" );
            //    }
            //}
            //DealerHand.lblDealerCount.Content = dlrCount;
        }

        void finishDealersHand()
        {
            DealerHand.cDlrCrd2.SetCard( dlrSecondCard, false );
            dlrCount += dlrSecondCard.Value;
            DealerHand.lblDealerCount.Content = dlrCount;

            if( dlrCount < 17 )
            {
                dlrCardHit( dealerContainers );
                finishDealersHand();
            }
            else
            {
                if( plrCount <= 21 && dlrCount <= 21 )
                {
                    if( plrCount == dlrCount )
                    {
                        MessageBox.Show( "Push" );
                    }
                    else if( dlrCount < plrCount )
                    {
                        MessageBox.Show( "Winner!" );
                    }
                    else
                    {
                        MessageBox.Show( "Dealer Wins!" );
                    }
                }
                else if( plrCount <= 21 && dlrCount > 21 )
                {
                    MessageBox.Show( "Winner!" );
                }
                else
                {
                    MessageBox.Show( "No Winner!" );
                }

            }
        }

        //private void btnSplit_Click( object sender, RoutedEventArgs e )
        //{
        //    mainHand.Visibility = Visibility.Hidden;

        //    splitHand1.card1 = mainHand.card1;
        //    Card card = shoe.Draw();
        //    splitHand1.card2.SetCard( card );

        //    splitHand2.card2 = mainHand.card2;
        //    Card card2 = shoe.Draw();
        //    splitHand2.card2.SetCard( card2 );

        //    splitHand1.Visibility = Visibility.Visible;
        //    splitHand2.Visibility = Visibility.Visible;

        //}

        private void btnDoubleDown_Click( object sender, RoutedEventArgs e )
        {
            plrCardHit( playerContainers );
            btnHit.IsEnabled = false;
            btnStay.IsEnabled = false;
            btnDoubleDown.IsEnabled = false;
            //btnSplit.IsEnabled = false;
            finishDealersHand();

        }

        private delegate void ClientUpdateDelegate(Dictionary<String, Player> players);

        public void UpdateClientWindow(Dictionary<String, Player> players)
        {
            txtJoin.Dispatcher.BeginInvoke(new ClientUpdateDelegate(updateClientWindow), players);
        }

        private void updateClientWindow(Dictionary<String, Player> players)
        {
            //container card number
            int cardNum = 0;

            //clear hand
            clearCards();

            //update players' hand
            foreach (Player player in players.Values)
            {
                string pName = player.Name;
                foreach (Card card in player.State.CardsInPlay)
                {
                    if (pName.Equals(txtJoin.Text)) //main player
                    {
                        playerContainers[cardNum++].SetCard(card);
                    }
                    else if (pName.Equals("Dealer")) //dealer
                    {
                        dealerContainers[cardNum++].SetCard(card, cardNum == 1);
                    }
                    else //other players on screen
                    {

                    }
                }
            }
            
        }
    }
}
