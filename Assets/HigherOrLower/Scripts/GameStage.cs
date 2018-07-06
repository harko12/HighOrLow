using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameStage : ScriptableObject
{

    public int Level;
    public int Stage;
    public string Title;
    public int PreReqPoints;

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

    /// <summary>
    /// figure out the complexity of this stage
    /// </summary>
    /// <remarks>The lowest it's likely to be is 1.  </remarks>
    public float Complexity
    {
        get
        {
            // figure the distance that this value is away from .5.  .5 would be the most difficult, because there is
            // a 50% chance that the desired outcome will be either high or low.
            // 0 is the easiest, 1 being the hardest, because that means there is an equal chance of it being high or low (.5)
            var highLowDifficulty = (HighLowWeight <= .5f ? HighLowWeight : 1 - HighLowWeight) * 2;

            // similar to highLow, the x y inversions are most difficult when they are right in the middle, meaning at any 
            // time it could be one or the other.  
            var invertXDifficulty = (InvertXWeight <= .5f ? InvertXWeight : 1 - InvertXWeight) * 2;
            var invertYDifficulty = (InvertYWeight <= .5f ? InvertYWeight : 1 - InvertYWeight) * 2;

            // average the inversion difficulties to get one value for inversion
            var inversionDifficulty = (invertXDifficulty + invertYDifficulty) / 2;

            // lockleft provides some difficulty by making it possible the left side is a mirrored version of the right
            var lockLeftDifficulty = (LockLeftSide ? 0 : 1) * inversionDifficulty;

            // the startx and y are percentages of how likely it is to generate a random offset.  0 would be the easiest, and 
            // the difficulty scales up pretty dramatically from there, since the offsets are always random, as soon as you have
            // one things can look different.
            var startxDifficulty = StartXWeight;
            var startyDifficulty = StartYWeight;

            // not sure yet how to figure the led difficulty.  the more there are, the tougher it is
            var ledStyleDifficulty = LEDStyleWeights.Length;

            // so, put it all together.
            return highLowDifficulty + inversionDifficulty + lockLeftDifficulty + startxDifficulty + startyDifficulty + ledStyleDifficulty;
        }
    }

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
