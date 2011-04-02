using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackJackLibrary
{
    public class Game : MarshalByRefObject
    {
        private Shoe shoe = new Shoe( 3 );
        private PlayerState pState = new PlayerState();
        private Dictionary<String, ICallback> clientCallbacks = new Dictionary<string, ICallback>();

        public PlayerState Join(string name, ICallback callback)
        {
            try
            {
                clientCallbacks.Add(name, callback);
                Console.WriteLine("Player: " + name + " has just joined the game!");
                pState.CardsInPlay.Add(name, new List<Card>());
                //updateAllClients();
                return pState;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void Hit()
        {
            pState.CardsInPlay["Jer"].Add(shoe.Draw());
            updateAllClients();
        }

        public void Ready()
        {
            pState.CardsInPlay["Jer"].AddRange(drawMultiple(2));
            updateAllClients();
        }

        // Helper methods
        private List<Card> drawMultiple(int nCards)
        {
            List<Card> cards = new List<Card>();
            for( int i = 0; i < nCards; i++)
                cards.Add(shoe.Draw());

            return cards;
        }

        private void updateAllClients()
        {
            foreach (ICallback callback in clientCallbacks.Values)
                callback.PlayerUpdate(pState);
        }

    }
}
