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
using CardsLibrary;
using System.Reflection;

namespace BlackJack
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

        public void SetCard( Card card, bool isSecondCard )
        {
            ImageBrush brush = new ImageBrush();
            BitmapImage cardIm = new BitmapImage();

            if( isSecondCard )
            {
                cardIm.BeginInit();   
                cardIm.StreamSource =Assembly.GetExecutingAssembly().GetManifestResourceStream( "BlackJack.img." + "b2fv.png" );
                cardIm.EndInit();

            }
            else
            {
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
                cardIm.StreamSource = Assembly.GetExecutingAssembly().GetManifestResourceStream( "BlackJack.img." + tvSuit + tvRank + ".png" );
                cardIm.EndInit();


            }
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

        private void UserControl_Loaded( object sender, RoutedEventArgs e )
        {

        }
    }
}
