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

    private static GameRoundLEDInfo GetLEDInfo(GameStageSetup rules, int round, int count, int arraySize, bool isCorrect)
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

        if (rules.LEDStyleWeights.Count == 1)
        {
            li.graphType = rules.LEDStyleWeights.First().Key;
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

    private static GameStageSetup GetRules(int lvl, int stage, out bool exactMatch)
    {
        GameStageSetup gs = null;
        Dictionary<int, GameStageSetup> levelDict = null;
        var done = false;
        exactMatch = true;
        var targetLvl = lvl;
        var targetStage = stage;
        while (!done)
        {
            if(StageDirections.TryGetValue(targetLvl, out levelDict))
            {
                while (targetStage > 0)
                {
                    if (!levelDict.TryGetValue(targetStage, out gs))
                    {
                        targetStage--;
                        exactMatch = false;
                        continue;
                    }
                    return gs;
                }
                targetLvl--;
                targetStage = STAGE_COUNT;
                exactMatch = false;
                continue;
            }
            else
            {
                targetLvl--;
                targetStage = STAGE_COUNT;
                exactMatch = false;
            }

            if (targetLvl <= 0)
            {
                done = true;
            }
        }
        return null;
    }

    #region GameStageSetup
    public static Dictionary<int, Dictionary<int, GameStageSetup>> StageDirections =
        new Dictionary<int, Dictionary<int, GameStageSetup>>();

    public class GameStageSetup
    {
        public int Level { get; set; }
        public int Stage { get; set; }
        public string Title { get; set; }
        public int MaxValue { get; set; }
        public float WaitSeconds { get; set; }
        public int VarianceMax { get; set; }
        public int VarianceMin { get; set; }
        /// <summary>
        /// Weight to determine the mix of what is asked for, 
        /// higher or lower.  0f for all lower, 100f for all higher
        /// </summary>
        public float HighLowWeight { get; set; }
        public List<string> IntroText;
        public Dictionary<LEDRendering.GraphType, float> LEDStyleWeights;
        public float InvertXWeight { get; set; }
        public float InvertYWeight { get; set; }
        public bool LockLeftSide { get; set; }
        public float StartXWeight { get; set; }
        public float StartYWeight { get; set; }

        public GameStageSetup()
        {
            IntroText = new List<string>();
            LEDStyleWeights = new Dictionary<LEDRendering.GraphType, float>();
            // some defaults
            MaxValue = 32;
            VarianceMax = 25;
            VarianceMin = 5;
            LockLeftSide = true;
        }
    }

    private static void AddStageSetup(GameStageSetup gs)
    {
        if (!StageDirections.ContainsKey(gs.Level))
        {
            StageDirections.Add(gs.Level, new Dictionary<int, GameStageSetup>());
        }
        StageDirections[gs.Level].Add(gs.Stage, gs);
    }

    public static void InitStageSetups()
    {
        //1-1
        var gs = new GameStageSetup()
        {
            Level = 1,
            Stage = 1,
            Title = "Learning Horizontal",
            WaitSeconds = 10f,
            HighLowWeight = 1f,
            InvertXWeight = 0f,
            InvertYWeight = 0f,
            StartXWeight = 0f,
            StartYWeight = 0f,
            IntroText = new List<string>()
            {
              //"               "
                "Time to learn",
                "a pattern..",
                "Horizontal!",
            },
            LEDStyleWeights = new Dictionary<LEDRendering.GraphType, float>()
            {
                {LEDRendering.GraphType.Horizontal, 100f }
            }
        };
        AddStageSetup(gs);

        //1-2
        gs = new GameStageSetup()
        {
            Level = 1,
            Stage = 2,
            Title = "Horizontal flip",
            WaitSeconds = 10f,
            HighLowWeight = 1f,
            InvertXWeight = 1f,
            InvertYWeight = 0f,
            StartXWeight = 0f,
            StartYWeight = 0f,
            IntroText = new List<string>()
            {
              //"               "
                "Mirror..",
                ".. Mirror",
                "On the Wall",
            },
            LEDStyleWeights = new Dictionary<LEDRendering.GraphType, float>()
            {
                {LEDRendering.GraphType.Horizontal, 100f }
            }
        };
        AddStageSetup(gs);

        //1-3
        gs = new GameStageSetup()
        {
            Level = 1,
            Stage = 3,
            Title = "Vertical flip",
            WaitSeconds = 10f,
            HighLowWeight = 1f,
            InvertXWeight = 0f,
            InvertYWeight = 1f,
            StartXWeight = 0f,
            StartYWeight = 0f,
            IntroText = new List<string>()
            {
              //"               "
                "Sometimes",
                "Up",
                "Is Down",
            },
            LEDStyleWeights = new Dictionary<LEDRendering.GraphType, float>()
            {
                {LEDRendering.GraphType.Horizontal, 100f }
            }
        };
        AddStageSetup(gs);

        //1-4
        gs = new GameStageSetup()
        {
            Level = 1,
            Stage = 4,
            Title = "learning vertical",
            WaitSeconds = 10f,
            HighLowWeight = 1f,
            InvertXWeight = 0f,
            InvertYWeight = 0f,
            StartXWeight = 0f,
            StartYWeight = 0f,
            IntroText = new List<string>()
            {
              //"               "
                "a new ",
                "Pattern has its",
                "ups and downs",
            },
            LEDStyleWeights = new Dictionary<LEDRendering.GraphType, float>()
            {
                {LEDRendering.GraphType.Vertical, 100f }
            }
        };
        AddStageSetup(gs);

        //1-5
        gs = new GameStageSetup()
        {
            Level = 1,
            Stage = 5,
            Title = "vertical",
            WaitSeconds = 10f,
            HighLowWeight = 1f,
            InvertXWeight = .5f,
            InvertYWeight = .5f,
            StartXWeight = 0f,
            StartYWeight = 0f,
            IntroText = new List<string>()
            {
              //"               "
                " the flips ",
                "are back",
            },
            LEDStyleWeights = new Dictionary<LEDRendering.GraphType, float>()
            {
                {LEDRendering.GraphType.Vertical, 100f }
            }
        };
        AddStageSetup(gs);


    }
    #endregion
}
