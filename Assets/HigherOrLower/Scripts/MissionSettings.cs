using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MissionSettings : ScriptableObject
{
    [Range(0, 1)]
    public float WagerChance;

    public Mission[] Scenarios;

    [MenuItem("Assets/More||Less/Create/MissionSettings")]
    public static void CreateAsset()
    {
        var newObj = ScriptableObjectUtility.CreateAsset<MissionSettings>("Assets/HigherOrLower/MissionSettings/");
        newObj.Setup();
    }

    public void Setup()
    {
    }
}
