using Apex.AI;
using Apex.AI.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighOrLow.AI
{
    public sealed class PlayerDataContextProvider : MonoBehaviour, IContextProvider
    {
        private PlayerDataContext _context;

        public IAIContext GetContext(Guid aiId)
        {
            return _context;
        }

        private void OnEnable()
        {
            var playerData = HighLowGame.GetInstance().Getplayer();
            _context = new PlayerDataContext(playerData);
        }
    }
}
