using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HarkoGames
{
    [CustomEditor(typeof(FuzzyValueTester))]
    public class FuzzyValueTesterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var myScript = (FuzzyValueTester)target;
            if (GUILayout.Button("Test"))
            {
                myScript.Test();
                Canvas.ForceUpdateCanvases();
            }
            DrawResults();
        }

        public void DrawResults()
        {
            var myScript = (FuzzyValueTester)target;
            if (myScript == null || myScript.TestResults == null)
            {
                return;
            }
            var rect = EditorGUILayout.BeginVertical();
            foreach (var key in myScript.TestResults.Keys)
            {
                var result = myScript.TestResults[key];
                EditorGUILayout.LabelField(string.Format("{0}: {1}", key.ToString(), result));
            }
            EditorGUILayout.EndVertical();
        }

    }
}
