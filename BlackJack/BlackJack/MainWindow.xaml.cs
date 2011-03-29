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
        List<ucCardContainer> plrContainers = new List<ucCardContainer>();
        List<ucSmallCardContainer> dlrContainers = new List<ucSmallCardContainer>();

        private Shoe shoe;
        public MainWindow()
        {
            InitializeComponent();
            btnReady.IsEnabled = false;
            btnHit.IsEnabled = false;
            btnStay.IsEnabled = false;

            //add card containers to list
            plrContainers.Add(container1);
            plrContainers.Add( container2);
            plrContainers.Add( container3 );
            plrContainers.Add( container4 );
            plrContainers.Add( container5 );

            dlrContainers.Add( DealerHand.cDlrCrd1 );
            dlrContainers.Add( DealerHand.cDlrCrd2 );
            dlrContainers.Add( DealerHand.cDlrCrd3 );
            dlrContainers.Add( DealerHand.cDlrCrd4 );
            dlrContainers.Add( DealerHand.cDlrCrd5 );

        }

        private void btnJoin_Click(object sender, RoutedEventArgs e)
        {
            lstPlayers.Items.Add(txtJoin.Text);
            txtJoin.Text = "";
            btnReady.IsEnabled = true;
            shoe = new Shoe(3);
        }

        private void btnReady_Click(object sender, RoutedEventArgs e)
        {
            //clear all cards
            clearCards();
            plrCount = 0;
            lblCount.Content = plrCount;

            Card card = shoe.Draw();
            container1.SetCard(card);
            Card card2 = shoe.Draw();
            container2.SetCard(card2);

            plrCount += card.Value + card2.Value;

            //set dealers hand
            Card dlrCard = shoe.Draw();
            DealerHand.cDlrCrd1.SetCard( dlrCard,false );
            dlrSecondCard = shoe.Draw();
            DealerHand.cDlrCrd2.SetCard( dlrSecondCard,true );
            DealerHand.lblDealer.Visibility = Visibility.Visible;

            dlrCount = dlrCard.Value;
            DealerHand.lblDealerCount.Content = dlrCount;

            //check if card is ace
            if (card.Rank == Card.RankID.Ace)
                isAce = true;
            if( plrCount > 21 && isAce )
            {
                plrCount -= 10;
                isAce = false;
            }
            else if( plrCount == 21 )
            {
                MessageBox.Show("Blackjack!");
            }
            lblCount.Content = plrCount;
            btnReady.IsEnabled = false;
            btnHit.IsEnabled = true;
            btnStay.IsEnabled = true;
        }

        private void btnHit_Click(object sender, RoutedEventArgs e)
        {
            plrCardHit( plrContainers );
        }

        private void btnStay_Click(object sender, RoutedEventArgs e)
        {

            btnReady.IsEnabled = true;
            btnHit.IsEnabled = false;
            btnStay.IsEnabled = false;

            finishDealersHand();
        }

        private void clearCards()
        {
            foreach (ucCardContainer uc in plrContainers)
            {
                uc.Clear();
            }
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
                    lblCount.Content = plrCount;
                    MessageBox.Show( "You bust!" );
                    btnHit.IsEnabled = false;
                    btnStay.IsEnabled = false;
                    btnReady.IsEnabled = true;
                    finishDealersHand();
                }
            }
            lblCount.Content = plrCount;
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
                    uc.SetCard( card,false );
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

            if( dlrCount < 17 )
            {
                dlrCardHit( dlrContainers );
                finishDealersHand();
            }
            else
            {
                if( plrCount <= 21 && plrCount > dlrCount)
                {
                    MessageBox.Show( "Winner!" );
                }
                else
                {
                    MessageBox.Show( "Dealer Wins" );
                }
            }
        }
    }
}
