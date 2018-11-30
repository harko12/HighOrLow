using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HarkoGames.EventSystem
{
    public class GameEventManager : ScriptableObject
    {
        [MenuItem("Assets/Harko Games/Events/Create/EventManager")]
        public static void CreateAsset()
        {
            var newManager = ScriptableObjectUtility.CreateAsset<GameEventManager>("Assets/HarkoGames/EventSystem");
        }

    }

}
