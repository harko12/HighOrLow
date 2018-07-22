using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WalletPanelTester))]
public class WalletPanelTesterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        WalletPanelTester myScript = (WalletPanelTester)target;
        if (GUILayout.Button("Update Wallets"))
        {
            myScript.updateWallets();
            Canvas.ForceUpdateCanvases();
        }

        if (GUILayout.Button("Update coins"))
        {
            myScript.updateCoins();
            Canvas.ForceUpdateCanvases();
        }
        if (GUILayout.Button("Update tokens"))
        {
            myScript.updateTokens();
            Canvas.ForceUpdateCanvases();
        }
        if (GUILayout.Button("Update time"))
        {
            myScript.updateTime();
            Canvas.ForceUpdateCanvases();
        }
    }
}
