﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackJackLibrary
{
    public interface ICallback
    {
        void PlayerUpdate(Dictionary<String, Player> players);
    }
}