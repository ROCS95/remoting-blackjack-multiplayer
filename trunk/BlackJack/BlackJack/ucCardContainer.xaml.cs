/*Title: BlackJackClient
* Page: ucCardContainer.xaml.cs
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
using BlackJackLibrary;
using System.Reflection;
using System.IO;
using System.Drawing;

namespace BlackJackClient
{
    /// <summary>
    /// Interaction logic for ucCardContainer.xaml
    /// </summary>
    public partial class ucCardContainer : UserControl
    {
        private bool isSet_;
        public delegate void SetCardDelegate( Card card );
        public SetCardDelegate SetCard;
        public ucCardContainer()
        {
            InitializeComponent();
            SetCard = new SetCardDelegate(setCard);

        }
        /* Method Name: SetCard
         * Purpose: displays the card image in the container
         * Output: None
         * Input: card- the card information
         */
        public void setCard( Card card ) {
            string tvSuit = card.Suit.ToString().Substring(0, 1).ToLower();
            string tvRank = "";
            //determine card image based on rank and suit of card
            switch (card.Rank)
            {
                case Card.RankID.Ace:
                    tvRank = "1";
                    break;
                case Card.RankID.King:
                case Card.RankID.Queen:
                case Card.RankID.Jack:
                    tvRank = card.Rank.ToString().Substring(0, 1).ToLower();
                    break;
                default:
                    tvRank = card.Value.ToString();
                    break;
            }
            //set image to canvas background
            ImageBrush brush = new ImageBrush();
            BitmapImage cardIm = new BitmapImage();
            cardIm.BeginInit();
            cardIm.StreamSource = Assembly.GetExecutingAssembly().GetManifestResourceStream( "BlackJackClient.img." + tvSuit + tvRank + ".png" );
            cardIm.EndInit();
            brush.ImageSource = cardIm;           
            canvas1.Background = brush;
        }

        public void Clear()
        {
            canvas1.Background = null;
        }
    }
}
