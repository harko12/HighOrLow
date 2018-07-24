using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HighOrLow;

[CustomEditor(typeof(WalletPanel))]
public class WalletPanelEditor : Editor {

    public override void OnInspectorGUI()
    {
        WalletPanel myScript = (WalletPanel)target;
        EditorGUILayout.LabelField(myScript.WalletValueDescription());
        DrawDefaultInspector();
    }
}
