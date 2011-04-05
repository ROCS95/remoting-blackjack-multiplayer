/*Title: BlackJackClient
* Page: ucSmallCardContainer.xaml.cs
* Author: Brooke Thrower and Jeramie Hallyburton
* Description: User control to display cards
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
using System.IO;
using System.Drawing;
using BlackJackLibrary;
using System.Reflection;

namespace BlackJackClient
{
    /// <summary>
    /// Interaction logic for ucSmallCardContainer.xaml
    /// </summary>
    public partial class ucSmallCardContainer : UserControl
    {
        private bool isSet_;
        public ucSmallCardContainer()
        {
            InitializeComponent();
        }

        /* Method Name: SetCard
         * Purpose: displays the card image in the container
         * Output: None
         * Input: card- the card information, isFaceDown - determines if the card is up or down
         */
        public void SetCard( Card card, bool isFaceDown = false )
        {
            ImageBrush brush = new ImageBrush();
            BitmapImage cardIm = new BitmapImage();

            if( isFaceDown )
            {
                cardIm.BeginInit();   
                cardIm.StreamSource = Assembly.GetExecutingAssembly().GetManifestResourceStream( "BlackJackClient.img." + "b2fv.png" );
                cardIm.EndInit();

            }
            else
            {
                //determine card image based on suit and rank
                string tvSuit = card.Suit.ToString().Substring( 0, 1 ).ToLower();
                string tvRank = "";
                switch( card.Rank )
                {
                    case Card.RankID.Ace:
                        tvRank = "1";
                        break;
                    case Card.RankID.King:
                    case Card.RankID.Queen:
                    case Card.RankID.Jack:
                        tvRank = card.Rank.ToString().Substring( 0, 1 ).ToLower();
                        break;
                    default:
                        tvRank = card.Value.ToString();
                        break;
                }
                cardIm.BeginInit();
                cardIm.StreamSource = Assembly.GetExecutingAssembly().GetManifestResourceStream( "BlackJackClient.img." + tvSuit + tvRank + ".png" );
                cardIm.EndInit();
            }
            //add image to canvas
            brush.ImageSource = cardIm;
            canvas1.Background = brush;
        }
        //clear the card image
        public void Clear()
        {
            canvas1.Background = null;
        }
    }
}
