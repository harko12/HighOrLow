using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace HarkoGames.EventSystem
{

    public class GameEvent : ScriptableObject
    {
        public string EventId;
        private List<GameEventListener> listeners = new List<GameEventListener>();
        public UnityEvent Action;

        [MenuItem("Assets/Harko Games/Events/Create/GameEvent")]
        public static void CreateAsset()
        {
            var newEvent = ScriptableObjectUtility.CreateAsset<GameEvent>("Assets/HarkoGames/EventSystem/Events");
        }

        public void Raise(object[] args)
        {
            for (int lcv = listeners.Count - 1; lcv >= 0; lcv--)
            {
                listeners[lcv].OnEventRaised();
            }
        }

        public void RegisterListener(GameEventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnRegisterListener(GameEventListener listener)
        {
            listeners.Remove(listener);
        }
    }
}
