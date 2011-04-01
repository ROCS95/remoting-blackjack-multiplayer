using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardsLibrary
{
    public interface PlayerState
    {
        void UpdatePlayerScreenCallback(Dictionary<String, List<Card>> cardsInPlay);
    }
}