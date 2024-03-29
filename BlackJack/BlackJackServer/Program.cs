﻿/*Title: BlackJackServer
* Page: Program.cs
* Author: Brooke Thrower and Jeramie Hallyburton
* Description: Server information
* Created: Mar. 22, 2011
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;

namespace BlackJackServer
{
    class Program
    {
        static void Main(string[] args)
        {
            RemotingConfiguration.Configure("BlackJackServer.exe.config", false);


            // Keep the server running until <Enter> is pressed
            Console.WriteLine("BlackJack server is running. Press <Enter> to quit.");
            Console.ReadLine();
        }
    }
}
