using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameLevel : ScriptableObject {
    public int Level;
    public string Description;
    public GameStage[] Stages;

    [MenuItem("Assets/More||Less/Create/GameLevel")]
    public static void CreateAsset()
    {
        var newLevel = ScriptableObjectUtility.CreateAsset<GameLevel>("Assets/HigherOrLower/Stages/Levels");
        newLevel.name = "Level ##";
    }

}
