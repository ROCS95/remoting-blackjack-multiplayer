using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackJackLibrary
{
    public class Game : MarshalByRefObject
    {
        private Shoe shoe = new Shoe( 3 );
        private Dictionary<String, Player>      players     = new Dictionary<String, Player>();
        private Dictionary<String, ICallback>   callbacks   = new Dictionary<String, ICallback>();
        public ICallback CallBack = null;

        public void Join(string name, ICallback callback)
        {
            try
            {
                Player newPlayer = new Player(name);
                players.Add(newPlayer.Name, newPlayer);
                callbacks.Add(name, callback);

                Console.WriteLine("Player: " + newPlayer.Name + " has just joined the game!");

                updateAllClients();
            }
            catch (Exception)
            {
                
            }
        }

        public void Hit()
        {
            players["Jer"].State.CardsInPlay.Add(shoe.Draw());
            updateAllClients();
        }

        public void Ready()
        {
            players["Jer"].State.CardsInPlay.AddRange(drawMultiple(2));
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
            foreach (String key in players.Keys)
                callbacks[key].PlayerUpdate(players);
        }

    }
}
