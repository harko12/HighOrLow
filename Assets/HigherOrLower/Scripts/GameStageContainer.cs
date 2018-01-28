using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameStageContainer : ScriptableObject
{
    public GameLevel[] Levels;

    [MenuItem("Assets/More||Less/Create/GameStageContainer")]
    public static void CreateAsset()
    {
        var newContainer = ScriptableObjectUtility.CreateAsset<GameStageContainer>("Assets/HigherOrLower/Stages/Containers");
        newContainer.name = "Container ##";
    }
}
