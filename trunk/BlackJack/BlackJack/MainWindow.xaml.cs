﻿using System;
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

        /*
        List<ucSmallCardContainer> dealerContainers = new List<ucSmallCardContainer>();
        List<ucCardContainer> playerContainers = new List<ucCardContainer>();
        List<ucOtherPlayerHand> otherPlayerHandContainers = new List<ucOtherPlayerHand>();

        List<List<ucSmallCardContainer>> otherPlayerHands = new List<List<ucSmallCardContainer>>();
        List<ucSmallCardContainer> playerContainers1 = new List<ucSmallCardContainer>();
        List<ucSmallCardContainer> playerContainers2 = new List<ucSmallCardContainer>();
        List<ucSmallCardContainer> playerContainers3 = new List<ucSmallCardContainer>();
        List<ucSmallCardContainer> playerContainers4 = new List<ucSmallCardContainer>();
        */

        // Create a reference to a blackjack Game object
        private Game game;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                // Load the remoting configuration file
                RemotingConfiguration.Configure( "BlackJackClient.exe.config", false );

                // Activate a game object

                //game = ( Game )Activator.GetObject( typeof( Game ), "http://localhost:9999/game.binary" );

                game = ( Game )Activator.GetObject( typeof( Game ), "tcp://localhost:12222/game.binary" );

            }
            catch( Exception ex )
            {
                MessageBox.Show( ex.Message );
            }

            btnReady.IsEnabled = false;
            btnHit.IsEnabled = false;
            btnStay.IsEnabled = false;

            /*
            //add card containers to list
            playerContainers.Add( mainHand.card1 );
            playerContainers.Add( mainHand.card2 );
            playerContainers.Add( mainHand.card3 );
            playerContainers.Add( mainHand.card4 );
            playerContainers.Add( mainHand.card5 );

            //dealer cards
            dealerContainers.Add( DealerHand.cDlrCrd1 );
            dealerContainers.Add( DealerHand.cDlrCrd2 );
            dealerContainers.Add( DealerHand.cDlrCrd3 );
            dealerContainers.Add( DealerHand.cDlrCrd4 );
            dealerContainers.Add( DealerHand.cDlrCrd5 );

            //other player hand
            playerContainers1.Add( PlayerContainer1.Card1 );
            playerContainers1.Add( PlayerContainer1.Card2 );
            playerContainers1.Add( PlayerContainer1.Card3 );
            playerContainers1.Add( PlayerContainer1.Card4 );
            playerContainers1.Add( PlayerContainer1.Card5 );

            playerContainers2.Add( PlayerContainer2.Card1 );
            playerContainers2.Add( PlayerContainer2.Card2 );
            playerContainers2.Add( PlayerContainer2.Card3 );
            playerContainers2.Add( PlayerContainer2.Card4 );
            playerContainers2.Add( PlayerContainer2.Card5 );

            playerContainers3.Add( PlayerContainer3.Card1 );
            playerContainers3.Add( PlayerContainer3.Card2 );
            playerContainers3.Add( PlayerContainer3.Card3 );
            playerContainers3.Add( PlayerContainer3.Card4 );
            playerContainers3.Add( PlayerContainer3.Card5 );

            playerContainers4.Add( PlayerContainer4.Card1 );
            playerContainers4.Add( PlayerContainer4.Card2 );
            playerContainers4.Add( PlayerContainer4.Card3 );
            playerContainers4.Add( PlayerContainer4.Card4 );
            playerContainers4.Add( PlayerContainer4.Card5 );


            //add other player hands to list
            otherPlayerHands.Add( playerContainers1 );
            otherPlayerHands.Add( playerContainers2 );
            otherPlayerHands.Add( playerContainers3 );
            otherPlayerHands.Add( playerContainers4 );

            //add other players containers to list
            otherPlayerHandContainers.Add( PlayerContainer1 );
            otherPlayerHandContainers.Add( PlayerContainer2 );
            otherPlayerHandContainers.Add( PlayerContainer3 );
            otherPlayerHandContainers.Add( PlayerContainer4 );
            */

        }


        private void btnJoin_Click( object sender, RoutedEventArgs e )
        {
            if( txtJoin.Text.Equals( "" ) )
            {
                MessageBox.Show( "Please Enter a User Name" );
            }
            else
            {
                game.Join( txtJoin.Text, new Callback( this ) );
                lblPlayerName.Content = txtJoin.Text;
                txtBank.Text = Convert.ToString( game.getPlayer(lblPlayerName.Content.ToString()).State.Bank );
                txtJoin.IsEnabled = false;
                btnJoin.IsEnabled = false;
                txtBid.IsEnabled = true;
                DealerHand.lblDealerCount.Content = Convert.ToString(game.getPlayer( "Dealer" ).State.CardTotal);
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

            game.Ready( Convert.ToInt32( txtBid.Text ), txtJoin.Text );
            mainHand.lblCount.Content = game.getPlayer( lblPlayerName.Content.ToString() ).State.CardTotal;
            txtBank.Text = Convert.ToString( game.getPlayer( lblPlayerName.Content.ToString() ).State.Bank );

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
            /*
            foreach( ucCardContainer uc in playerContainers )
            {
                uc.Clear();
            }

            foreach( ucSmallCardContainer uc in dealerContainers )
            {
                uc.Clear();
            }
            */

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
            game.Hit();
            mainHand.lblCount.Content = game.getPlayer( lblPlayerName.Content.ToString() ).State.CardTotal;
        }

        private void btnStay_Click( object sender, RoutedEventArgs e )
        {
            btnHit.IsEnabled = false;
            btnStay.IsEnabled = false;
            btnReady.IsEnabled = true;
            btnDoubleDown.IsEnabled = false;
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
            DealerHand.card2.SetCard( dlrSecondCard, true );
            dlrCount += dlrSecondCard.Value;
            DealerHand.lblDealerCount.Content = dlrCount;

            if( dlrCount < 17 )
            {
                //dlrCardHit( dealerContainers );
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

                btnReady.IsEnabled = true;
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
            //plrCardHit( playerContainers );
            btnHit.IsEnabled = false;
            btnStay.IsEnabled = false;
            btnDoubleDown.IsEnabled = false;
            //btnSplit.IsEnabled = false;
            finishDealersHand();

        }

        private delegate void ClientUpdateDelegate( List<Player> players );

        public void UpdateClientWindow( List<Player> players )
        {
            txtJoin.Dispatcher.BeginInvoke( new ClientUpdateDelegate( updateClientWindow ), players );
        }

        private void updateClientWindow( List<Player> players )
        {

            /*
            //clear hand
            clearCards();

            //update players' hand
            int otherPlayerIndex = 0;

            foreach( Player player in players )
            {
                int cardNum = 0;
                string pName = player.Name;

                if( !pName.Equals( txtJoin.Text ) && !pName.Equals( "Dealer" ) )
                {
                    otherPlayerIndex++;
                    //if( otherPlayerIndex != 0 )
                        //otherPlayerHandContainers[otherPlayerIndex - 1].lblPlrName.Content = game.getPlayer( txtJoin.Text ).Name;
                }

                if( pName.Equals( txtJoin.Text ) )
                {
                    mainHand.lblCount.Content = player.State.CardTotal;
                    
                }
                foreach( Card card in player.State.CardsInPlay )
                {
                    if( pName.Equals( txtJoin.Text ) ) //main player
                    {
                        playerContainers[cardNum++].SetCard( card );
                    }
                    else if( pName.Equals( "Dealer" ) ) //dealer
                    {
                        dealerContainers[cardNum++].SetCard( card, cardNum == 1 );
                    }
                    else //other players on screen
                    {
                        otherPlayerHands[otherPlayerIndex-1][cardNum++].SetCard( card, false );
                    }
                }
            }
            */

            clearCards();

            int otherPlayerCount = 0;
            ucOtherPlayerHand otherPlayerHand;

            foreach( Player player in players )
            {
                if( player.Name.Equals( txtJoin.Text ) )
                {
                    mainHand.lblCount.Content = player.State.CardTotal;

                    for( int i = 0; i != player.State.CardsInPlay.Count; ++i )
                    {
                        ( mainHand.FindName( "card" + ( i + 1 ) ) as ucCardContainer ).SetCard( player.State.CardsInPlay[i] );
                    }
                }
                else if( player.Name.Equals( "Dealer" ) )
                {
                    DealerHand.lblDealerCount.Content = player.State.CardTotal;

                    for( int i = 0; i != player.State.CardsInPlay.Count; ++i )
                    {
                        if( i == 0 )
                        {
                            ( DealerHand.FindName( "card" + ( i + 1 ) ) as ucSmallCardContainer ).SetCard( player.State.CardsInPlay[i], true );
                        }
                        else
                        {
                            ( DealerHand.FindName( "card" + ( i + 1 ) ) as ucSmallCardContainer ).SetCard( player.State.CardsInPlay[i] );
                        }
                    }
                }
                else
                {
                    ++otherPlayerCount;
                    otherPlayerHand = ( this.FindName( "PlayerContainer" + otherPlayerCount ) ) as ucOtherPlayerHand;
                    otherPlayerHand.lblPlrName.Content = player.Name;

                    for( int i = 0; i != player.State.CardsInPlay.Count; ++i )
                    {
                        ( otherPlayerHand.FindName( "card" + ( i + 1 ) ) as ucSmallCardContainer ).SetCard( player.State.CardsInPlay[i] );
                    }
                }
            }
        }

        private void txtBid_TextChanged( object sender, TextChangedEventArgs e )
        {
            int bet;
            if( int.TryParse( txtBid.Text, out bet ) )
                btnReady.IsEnabled = true;
        }
    }
}