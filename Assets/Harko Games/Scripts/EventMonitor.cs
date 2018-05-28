using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HarkoGames
{

    public static class EventMonitor
    {
        private static Dictionary<string, int> RunningEvents;

        private static Dictionary<string, int> GetRunningEvents()
        {
            if (RunningEvents == null)
            {
                RunningEvents = new Dictionary<string, int>();
            }
            return RunningEvents;
        }

        private static int GetEventCount(string eventId)
        {
            var events = GetRunningEvents();
            int count = 0;
            events.TryGetValue(eventId, out count);

            return count;
        }

        private static void AdjustEventCount(string eventId, bool increment)
        {
            var events = GetRunningEvents();
            int count = 0;
            if (!events.TryGetValue(eventId, out count))
            {
                events.Add(eventId, count);
            }

            var newCount = Mathf.Clamp((increment ? ++count : --count), 0, 9999);
            events[eventId] = newCount;
        }

        public static bool IsRunning(string eventId)
        {
            var val = GetEventCount(eventId);
            return val > 0;
        }

        public static void StartEvent(string eventId)
        {
            AdjustEventCount(eventId, true);
        }

        public static void EndEvent(string eventId)
        {
            AdjustEventCount(eventId, false);
        }
    }
}