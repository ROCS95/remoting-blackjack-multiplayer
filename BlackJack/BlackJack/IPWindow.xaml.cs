﻿/*Title: BlackJackClient
* Page: IPWindow.xaml.cs
* Author: Brooke Thrower and Jeramie Hallyburton
* Description: Gets the server address
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
using System.Windows.Shapes;

namespace BlackJackClient
{
    /// <summary>
    /// Interaction logic for ipWindow.xaml
    /// </summary>
    public partial class IPWindow : Window
    {
        public IPWindow()
        {
            InitializeComponent();
        }
        //set server address
        public string Address
        {
            get { return (string)this.DataContext; }
            set { this.DataContext = value; }
        }
        //set results of ok button
        private void btnOK_Click( object sender, RoutedEventArgs e )
        {
            Address = txtIP.Text;
            this.DialogResult = true;
        }
    }
}
