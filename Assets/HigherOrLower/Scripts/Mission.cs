using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MissionTypes { Survival, Sprint }
public enum MissionResultType { _Unknown, Success, Timeout, Failure, Quit}
[System.Serializable]
public class MissionResult
{
    public int EarnedPoints { get; set; }
    public int Failures { get; set; }
    public int MaxStreak { get; set; }
    public int NearMisses { get; set; }
    public float TimeRemaining { get; set; }
    public bool Succeeded { get; set; }
    public MissionResultType ResultType { get; set; }
    
}

[System.Serializable]
public class Mission {
    public const int NO_TIMER_VALUE = -9999;
    public MissionTypes MissionType { get; set; }
    public GameStage StageInfo { get; set; }
    public float TotalSeconds { get; set; }
    public bool UseOverallTime
    {
        get
        {
            return TotalSeconds != NO_TIMER_VALUE;
        }
    }
    public float BaseRecoverySeconds { get; set; }
    public int Rounds { get; set; }
    public int Chances { get; set; }
    public bool CanWager { get; set; }
    public int Wager { get; set; }
    public Wallet PrizePurse { get; set; }
    public MissionResult Result { get; set; }

    public Mission()
    {
        TotalSeconds = NO_TIMER_VALUE;
        Result = new MissionResult();
    }

    public override string ToString()
    {
        return string.Format("Mission {0}, {1}-{2}, total seconds: {3}, rounds: {4}, chances: {5}", MissionType, StageInfo.Level, StageInfo.Stage, TotalSeconds, Rounds, Chances);
    }

    public static Mission[] GenerateMissions(GamePlayer p, GameStage stage)
    {
        /* need to come up with some rules, based on player's level and 
         * current score, and such as to what kind of missions to generate. 
         * For now, I'll do one of each
         */
        var m1 = new Mission()
        {
            StageInfo = stage,
            MissionType = MissionTypes.Sprint,
            TotalSeconds = 30f,
            Rounds = 15,
            BaseRecoverySeconds = 0f,
            CanWager = false,
            PrizePurse = new Wallet() { Points = 1000, Coins = 1, Tokens = 10 }
        };

        var m2 = new Mission()
        {
            StageInfo = stage,
            MissionType = MissionTypes.Survival,
            Rounds = 0,
            BaseRecoverySeconds = 2f, // milliseconds, I think
            CanWager = false,
            PrizePurse = new Wallet() { Points = 1000, Coins = 1, Tokens = 10 }
        };

        var m3 = new Mission()
        {
            StageInfo = stage,
            MissionType = MissionTypes.Sprint,
            TotalSeconds = 60f,
            Rounds = 20,
            Chances = 1,
            BaseRecoverySeconds = 2f, // milliseconds, I think
            CanWager = false,
            PrizePurse = new Wallet() { Points = 1000, Coins = 1, Tokens = 10 }
        };

        return new Mission[] { m1, m2, m3 };
    }
}
