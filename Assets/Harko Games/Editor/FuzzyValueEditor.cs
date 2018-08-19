using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HarkoGames
{
    [CustomEditor(typeof(FuzzyValue))]
    public class FuzzyValueEditor : Editor
    {
        SerializedObject m_object; 

        public void OnEnable()
        {
            m_object = new UnityEditor.SerializedObject(target);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            FuzzyDefinitionEditorList.Show(serializedObject.FindProperty("definitions"));
            var myScript = (FuzzyValue)target;
            serializedObject.ApplyModifiedProperties();

            myScript.Organize();
            serializedObject.ApplyModifiedProperties();
        }
    }

}
