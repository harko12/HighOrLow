using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HarkoGames
{
    public class FuzzyValueTester : MonoBehaviour
    {
        public FuzzyValue TestTarget;
        public float TestValue;
        [HideInInspector]
        public Dictionary<string, float> TestResults;

        public void Test()
        {
            TestResults = TestTarget.Evaluate(TestValue);
        }
    }
}
