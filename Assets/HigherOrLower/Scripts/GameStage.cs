using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameStage : ScriptableObject
{

    public int Level;
    public int Stage;
    public string Title;

    public int MaxValue;
    public float WaitSeconds;
    public int VarianceMax;
    public int VarianceMin;
    /// <summary>
    /// Weight to determine the mix of what is asked for, 
    /// higher or lower.  0f for all lower, 100f for all higher
    /// </summary>
    public float HighLowWeight;
    public List<string> IntroText;
    public GameProgression.GameStageLEDStyleWeight[] LEDStyleWeights;
    public float InvertXWeight;
    public float InvertYWeight;
    public bool LockLeftSide;
    public float StartXWeight;
    public float StartYWeight;

    [MenuItem("Assets/More||Less/Create/GameStage")]
    public static void CreateAsset()
    {
        var newStage = ScriptableObjectUtility.CreateAsset<GameStage>("Assets/HigherOrLower/Stages");
        newStage.SetupStage();
    }

    public void SetupStage()
    {
        IntroText = new List<string>();
        // some defaults
        MaxValue = 32;
        VarianceMax = 25;
        VarianceMin = 5;
        LockLeftSide = true;
    }
}
