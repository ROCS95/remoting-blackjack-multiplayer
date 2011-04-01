using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlackJackLibrary;

namespace BlackJack
{
    public class Callback : MarshalByRefObject, ICallback
    {
        private MainWindow wnd;

        public Callback(MainWindow w)
        {
            wnd = w;
        }

        public void PlayerUpdate(PlayerState info)
        {
            wnd.UpdateClientWindow(info);
        }
    }
}
