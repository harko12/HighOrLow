using Apex.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HighOrLow.AI
{
    public sealed class PlayerDataContext : IAIContext
    {
        public PlayerDataContext(GamePlayer p)
        {
            myPlayer = p;
        }

        public GamePlayer myPlayer
        {
            get;
            private set;
        }
    }
}
