using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HarkoGames
{
    public static class TimeUtility
    {
        private const string Format_MSSMMM = "{0:0}:{1:00}.{2:000}";

        private const string Format_MSSMM = "{0:0}:{1:00}.{2:00}";

        public static string FormattedTime_MSSMM(float seconds)
        {
            System.TimeSpan t = System.TimeSpan.FromSeconds(seconds);
            int mins = t.Minutes;
            int secs = t.Seconds;
            int milliSecs = (t.Milliseconds / 10);
            return string.Format(Format_MSSMM, mins, secs, milliSecs);
        }
    }
}
