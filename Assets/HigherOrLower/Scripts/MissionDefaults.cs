using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MissionDefaults : ScriptableObject
{
    [Range(0,1)]
    public float WagerChance;

    public MissionDefault[] Easy;

    public MissionDefault[] Hard;

    [MenuItem("Assets/More||Less/Create/MissionDefaults")]
    public static void CreateAsset()
    {
        var newStage = ScriptableObjectUtility.CreateAsset<MissionDefaults>("Assets/HigherOrLower/MissionDefaults/");
        newStage.Setup();
    }

    public void Setup()
    {
    }
}

[System.Serializable]
public class MissionDefault
{
    public MissionTypes MissionType;
    public Mission Mission;
}
