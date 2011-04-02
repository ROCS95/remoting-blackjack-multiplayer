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
        private Player currentPlayer = null;
        public ICallback CallBack = null;

        public void Join(string name, ICallback callback)
        {
            try
            {
                Player newPlayer = new Player(name);
                players.Add(newPlayer.Name, newPlayer);
                callbacks.Add(name, callback);

                //initial join, start play between dealer and player 1
                if (players.Count == 1)
                {
                    currentPlayer = newPlayer;
                    startGame();
                } //else wait for current players turn to be over

                Console.WriteLine("Player: {0} has just joined the game!", newPlayer.Name);

                updateAllClients();
            }
            catch (Exception)
            {
                
            }
        }

        private void startGame()
        {
            players.Add("Dealer", new Player( "Dealer" ));
            players["Dealer"].State.CardsInPlay.AddRange(drawMultiple(2));
        }

        public void Hit()
        {
            currentPlayer.State.CardsInPlay.Add(shoe.Draw());
            updateAllClients();
        }

        public void Ready()
        {
            currentPlayer.State.CardsInPlay.AddRange(drawMultiple(2));
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
                if( !key.Equals("Dealer") )
                    callbacks[key].PlayerUpdate(players);
        }

    }
}
