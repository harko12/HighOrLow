using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HighOrLow
{

    [CustomEditor(typeof(MissionGeneratorTester))]
    public class MissionGeneratorTesterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            MissionGeneratorTester myScript = (MissionGeneratorTester)target;
            if (GUILayout.Button("Generate Missions"))
            {
                myScript.GenerateMissions();
                Canvas.ForceUpdateCanvases();
            }

            if (GUILayout.Button("Generate single Mission"))
            {
                myScript.GenerateSingleMission();
                Canvas.ForceUpdateCanvases();
            }

            if (GUILayout.Button("Calculate Mission Difficulty"))
            {
                myScript.CalculateMissionDifficulty();
                Canvas.ForceUpdateCanvases();
            }

            if (GUILayout.Button("Adjust Mission Difficulty"))
            {
                myScript.AdjustMissionDifficulty();
                Canvas.ForceUpdateCanvases();
            }

        }
    }
}