using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardsLibrary
{
    public interface IUpdatesFromServer
    {
        void UpdatePlayerScreenCallback(Dictionary<String, List<Card>> cardsInPlay);
    }
}