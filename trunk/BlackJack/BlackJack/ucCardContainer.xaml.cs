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

namespace BlackJack
{
    /// <summary>
    /// Interaction logic for ucCardContainer.xaml
    /// </summary>
    public partial class ucCardContainer : UserControl
    {
        private bool isSet_;
        public ucCardContainer()
        {
            InitializeComponent();

        }  

        public void SetCard( Card card ) {
            string tvSuit = card.Suit.ToString().Substring(0, 1).ToLower();
            string tvRank = "";
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
            ImageBrush brush = new ImageBrush();
            BitmapImage cardIm = new BitmapImage();
            cardIm.BeginInit();
            cardIm.StreamSource = Assembly.GetExecutingAssembly().GetManifestResourceStream( "BlackJackClient.img." + tvSuit + tvRank + ".png" );
            cardIm.EndInit();

            brush.ImageSource = cardIm;
            
            canvas1.Background = brush;

            isSet_ = true;
        }

        public bool IsSet
        {
            get { return isSet_; }
            set { ; }
        }

        public void Clear()
        {
            isSet_ = false;
            canvas1.Background = null;
        }
    }
}
