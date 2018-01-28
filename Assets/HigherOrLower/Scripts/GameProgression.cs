using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameProgression {
    public const int STAGE_COUNT = 10;
    public const int ROUND_COUNT = 10;
    public class GameRound
    {
        public int RightValue { get; set; }
        public int WrongValue { get; set; }
        public bool High { get; set; }
        public bool Led1Right { get; set; }
        public float WaitSeconds { get; set; }
        public List<string> IntroText { get; set; }
        public GameRoundLEDInfo[] LEDInfo = new GameRoundLEDInfo[2];
    }

    public class GameRoundLEDInfo
    {
        public LEDRendering.GraphType graphType { get; set; }
        public bool InvertX { get; set; }
        public bool InvertY { get; set; }
        public int StartX { get; set; }
        public int StartY { get; set; }
        public int Count { get; set; }
    }

    private static bool GetWeightedBool(float weight, int round)
    {
        if (weight == 0f) return false;
        if (weight == 1f) return true;

        var val = Random.value;

        return val < (weight + (round * .1f));
    }

    private static GameRoundLEDInfo GetLEDInfo(GameStage rules, int round, int count, int arraySize, bool isCorrect)
    {
        var li = new GameRoundLEDInfo();
        li.StartX = 0;
        li.StartY = 0;
        if (GetWeightedBool(rules.StartXWeight, round))
        {
            li.StartX = Random.Range(0, arraySize);
        }
        if (GetWeightedBool(rules.StartYWeight, round))
        {
            li.StartY = Random.Range(0, arraySize);
        }

        li.InvertX = false;
        li.InvertY = false;
        if (GetWeightedBool(rules.InvertXWeight, round))
        {
            li.InvertX = true;
        }
        if (GetWeightedBool(rules.InvertYWeight, round))
        {
            li.InvertY = true;
        }

        if (rules.LEDStyleWeights.Length == 1)
        {
            li.graphType = rules.LEDStyleWeights[0].GraphType;
        }
        else
        {
            float val = Random.value;
            var w = 0f;
            for(int lcv = 0, length = rules.LEDStyleWeights.Length; lcv < length; lcv++)
            {
                w += rules.LEDStyleWeights[lcv].Weight;
                if (w > val)
                {
                    li.graphType = rules.LEDStyleWeights[lcv].GraphType;
                    break;
                }
            }
        }
        li.Count = count;
        return li;
    }

    public static GameRound GetRound(int lvl, int stage, int round, int arraySize)
    {
        bool exactMatch;
        var rules = GetRules(lvl, stage, out exactMatch);
        var roundData = new GameRound();
        if (exactMatch) // the intro should only apply when the exact rule was found
        {
            roundData.IntroText = rules.IntroText;
        }

        roundData.High = Random.value < rules.HighLowWeight;
        var multiplier = roundData.High ? -1 : 1;

        int baseValue = 0;
        int wrongValue = 0;
        do
        {
            baseValue = Random.Range(1, rules.MaxValue);
            var adjustment = Random.Range(rules.VarianceMin, rules.VarianceMax) * multiplier;
            wrongValue = baseValue + adjustment;
        } while ((baseValue == 0 && wrongValue == 0));
        roundData.RightValue = baseValue;
        roundData.WrongValue = wrongValue;

        roundData.Led1Right = Random.value > .5f;
        var rightIndex = roundData.Led1Right ? 0 : 1;
        roundData.LEDInfo[rightIndex] = GetLEDInfo(rules, round, roundData.RightValue, arraySize, true);
        roundData.LEDInfo[1 - rightIndex] = GetLEDInfo(rules, round, roundData.WrongValue, arraySize, false);
        if (rules.LockLeftSide)
        {
            roundData.LEDInfo[0].InvertX = false;
            roundData.LEDInfo[0].InvertY = false;
        }
        // falloff the round time as it goes
        roundData.WaitSeconds = rules.WaitSeconds - ((rules.WaitSeconds * .25f) * (round * .1f));
        return roundData;
    }

    private static GameStageContainer mGameLevels;
    public static void InitGameProgression(GameStageContainer stages)
    {
        mGameLevels = stages;
    }

    private static GameStage GetRules(int lvl, int stage, out bool exactMatch)
    {

        GameStage s = null;
        exactMatch = true;

        var lvlIndex = lvl - 1;
        var stageIndex = stage - 1;

        if (mGameLevels.Levels.Length >= lvl)
        {
            var l = mGameLevels.Levels[lvlIndex];
            if (l.Stages.Length >= stage)
            {
                s = l.Stages[stageIndex];
            }
        }
        return s;
    }

    [System.Serializable]
    public class GameStageLEDStyleWeight
    {
        [SerializeField]
        public LEDRendering.GraphType GraphType;
        [SerializeField]
        public float Weight;
    }

}
