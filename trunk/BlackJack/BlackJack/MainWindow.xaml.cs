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
using CardsLibrary;
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
        
        List<ucSmallCardContainer> dlrContainers = new List<ucSmallCardContainer>();
        List<ucCardContainer> plrContainers = new List<ucCardContainer>();

        private Shoe shoe;

        public MainWindow()
        {
            InitializeComponent();
            btnReady.IsEnabled = false;
            btnHit.IsEnabled = false;
            btnStay.IsEnabled = false;

            //add card containers to list
            plrContainers.Add( mainHand.card1 );
            plrContainers.Add( mainHand.card2 );
            plrContainers.Add( mainHand.card3 );
            plrContainers.Add( mainHand.card4 );
            plrContainers.Add( mainHand.card5 );

            dlrContainers.Add( DealerHand.cDlrCrd1 );
            dlrContainers.Add( DealerHand.cDlrCrd2 );
            dlrContainers.Add( DealerHand.cDlrCrd3 );
            dlrContainers.Add( DealerHand.cDlrCrd4 );
            dlrContainers.Add( DealerHand.cDlrCrd5 );

        }

        private void btnJoin_Click( object sender, RoutedEventArgs e )
        {
            if( txtJoin.Text.Equals( "" ) )
            {
                MessageBox.Show( "Please Enter a User Name" );
            }
            else
            {
                lblPlayerName.Content = txtJoin.Text;
                txtJoin.IsEnabled = false;
                btnReady.IsEnabled = true;
                shoe = new Shoe( 3 );
                txtBank.Text = "500";
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

            Card card = shoe.Draw();
            mainHand.card1.SetCard( card );
            Card card2 = shoe.Draw();
            mainHand.card2.SetCard( card2 );

            plrCount += card.Value + card2.Value;

            //set dealers hand
            Card dlrCard = shoe.Draw();
            DealerHand.cDlrCrd1.SetCard( dlrCard, false );
            dlrSecondCard = shoe.Draw();
            DealerHand.cDlrCrd2.SetCard( dlrSecondCard, true );
            DealerHand.lblDealer.Visibility = Visibility.Visible;

            dlrCount = dlrCard.Value;
            DealerHand.lblDealerCount.Content = dlrCount;

            //check if card is ace
            if( card.Rank == Card.RankID.Ace )
                isAce = true;
            if( plrCount > 21 && isAce )
            {
                plrCount -= 10;
                isAce = false;
            }
            else if( plrCount == 21 )
            {
                MessageBox.Show( "Blackjack!" );
                btnReady.IsEnabled = true;
                btnHit.IsEnabled = false;
                btnStay.IsEnabled = false;

                finishDealersHand();
            }

            if( card.Value == card2.Value && card.Rank == card2.Rank )
            {
                btnSplit.IsEnabled = true;
            }

            btnSplit.IsEnabled = true;

            mainHand.lblCount.Content = plrCount;
            btnReady.IsEnabled = false;
            btnHit.IsEnabled = true;
            btnStay.IsEnabled = true;
            btnDoubleDown.IsEnabled = true;
        }

        private void clearCards()
        {
            foreach( ucCardContainer uc in plrContainers )
            {
                uc.Clear();
            }

            foreach( ucSmallCardContainer uc in dlrContainers )
            {
                uc.Clear();
            }
        }

        private void btnHit_Click( object sender, RoutedEventArgs e )
        {
            plrCardHit( plrContainers );
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
            Card card = shoe.Draw();
            plrCount += card.Value;

            //check which container to put card in
            foreach( ucCardContainer uc in containers )
            {
                if( !uc.IsSet )
                {
                    uc.SetCard( card );
                    break;
                }
            }

            //check if card is ace
            if( card.Rank == Card.RankID.Ace )
                isAce = true;
            if( plrCount > 21 )
            {
                if( isAce )
                {
                    plrCount -= 10;
                    isAce = false;
                }
                else
                {
                    mainHand.lblCount.Content = plrCount;
                    MessageBox.Show( "You bust!" );
                    btnHit.IsEnabled = false;
                    btnStay.IsEnabled = false;
                    finishDealersHand();
                }
            }
            mainHand.lblCount.Content = plrCount;
        }

        void dlrCardHit( List<ucSmallCardContainer> containers )
        {
            Card card = shoe.Draw();
            dlrCount += card.Value;

            //check which container to put card in
            foreach( ucSmallCardContainer uc in containers )
            {
                if( !uc.IsSet )
                {
                    uc.SetCard( card, false );
                    break;
                }
            }

            //check if card is ace
            if( card.Rank == Card.RankID.Ace )
                isAce = true;
            if( dlrCount > 21 )
            {
                if( isAce )
                {
                    dlrCount -= 10;
                    DealerHand.lblDealerCount.Content = dlrCount;
                    isAce = false;
                }
                else
                {
                    DealerHand.lblDealerCount.Content = dlrCount;
                    MessageBox.Show( "Dealer busts!" );
                }
            }
            DealerHand.lblDealerCount.Content = dlrCount;
        }

        void finishDealersHand()
        {
            DealerHand.cDlrCrd2.SetCard( dlrSecondCard, false );
            dlrCount += dlrSecondCard.Value;
            DealerHand.lblDealerCount.Content = dlrCount;

            if( dlrCount < 17 )
            {
                dlrCardHit( dlrContainers );
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

        private void btnSplit_Click( object sender, RoutedEventArgs e )
        {
            mainHand.Visibility = Visibility.Hidden;

            splitHand1.card1 = mainHand.card1;
            Card card = shoe.Draw();
            splitHand1.card2.SetCard( card );

            splitHand2.card2 = mainHand.card2;
            Card card2 = shoe.Draw();
            splitHand2.card2.SetCard( card2 );

            splitHand1.Visibility = Visibility.Visible;
            splitHand2.Visibility = Visibility.Visible;

        }

        private void btnDoubleDown_Click( object sender, RoutedEventArgs e )
        {
            plrCardHit( plrContainers );
            btnHit.IsEnabled = false;
            btnStay.IsEnabled = false;
            btnDoubleDown.IsEnabled = false;
            btnSplit.IsEnabled = false;
            finishDealersHand();

        }
    }
}
