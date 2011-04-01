using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackJackLibrary
{
    public class Game : MarshalByRefObject
    {
        private Shoe shoe;
        private PlayerState pState = new PlayerState();
        private Dictionary<String, ICallback> clientCallbacks = new Dictionary<string, ICallback>();

        public PlayerState Join(string name, ICallback callback)
        {
            try
            {
                clientCallbacks.Add(name, callback);
                return pState;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
