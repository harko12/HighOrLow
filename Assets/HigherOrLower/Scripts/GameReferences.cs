using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HighOrLow
{
    public class GameReferences : MonoBehaviour
    {
        public static GameReferences instance { get; private set; }

        private void Awake()
        {
            instance = this;
        }

        public GameEventManager gameEvents;
    }
}
