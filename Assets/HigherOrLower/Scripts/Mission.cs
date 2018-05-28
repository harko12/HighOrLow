using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MissionTypes { Survival, Sprint, ByRound }
public enum MissionResultType { _Unknown, Success, Timeout, Failure, Quit}
[System.Serializable]
public class MissionResult
{
    public Wallet EarnedLoot { get; set; }
    public int Failures { get; set; }
    public int MaxStreak { get; set; }
    public int Combo { get; set; }
    public int NearMisses { get; set; }
    public float TimeRemaining { get; set; }
    public float RoundTime { get; set; }
    public float MissionTime { get; set; }
    public float Progress { get; set; }
    public bool Succeeded { get; set; }
    public MissionResultType ResultType { get; set; }

    public MissionResult()
    {
        EarnedLoot = new Wallet();
    }
    
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
            return TotalSeconds != NO_TIMER_VALUE && TotalSeconds > 0;
        }
    }
    /// <summary>
    /// Base amount to adjust time by in survival mode
    /// </summary>
    public float BaseRecoverySeconds { get; set; }
    public int Rounds { get; set; }
    public int Chances { get; set; }
    public bool CanWager { get; set; }
    public int Wager { get; set; }
    public Wallet PrizePurse { get; set; }
    public MissionResult OverallResult { get; set; }

    public Mission()
    {
        TotalSeconds = NO_TIMER_VALUE;
        OverallResult = new MissionResult();
    }

    public override string ToString()
    {
        return string.Format("Mission {0}, {1}-{2}, total seconds: {3}, rounds: {4}, chances: {5}", MissionType, StageInfo.Level, StageInfo.Stage, TotalSeconds, Rounds, Chances);
    }

    /// <summary>
    /// Adjust time values and store results in mission result
    /// </summary>
    /// <param name="roundData"></param>
    /// <param name="startTime"></param>
    public void TimerTick(GameProgression.GameRound roundData, float startTime)
    {
        // ongoing countdown from the time the mission started (may be unused)
        OverallResult.TimeRemaining = Mathf.Clamp(OverallResult.TimeRemaining - Time.deltaTime, 0, 9999);
        var overallProgress = (!UseOverallTime ? 0
            : (TotalSeconds - OverallResult.TimeRemaining) / TotalSeconds);

        // time currently spent in this round
        OverallResult.RoundTime = Time.time - startTime;
        var elapsedRoundTime = Mathf.Clamp(roundData.WaitSeconds - OverallResult.RoundTime, 0, 9999);
        var roundProgress = (roundData.WaitSeconds <= 0 ? 0
            : OverallResult.RoundTime / roundData.WaitSeconds);

        switch (MissionType)
        {
            case MissionTypes.Sprint:
            case MissionTypes.Survival:
                OverallResult.Progress = overallProgress;
                OverallResult.MissionTime = OverallResult.TimeRemaining;
                break;
            case MissionTypes.ByRound:
                OverallResult.Progress = roundProgress;
                OverallResult.MissionTime = elapsedRoundTime;
                break;
        }
    }

    public void ProcessRoundResult(RoundResultInfo roundResult, ref int Round)
    {
        // adjust some time, based on the result
        AdjustTime(roundResult);
        HandleCombo(roundResult);
        HandlePoints(roundResult);

        OverallResult.EarnedLoot += roundResult.RoundLoot;

        switch (roundResult.State)
        {
            case FinishState.Right:
                break;
            case FinishState.Wrong:
                OverallResult.Failures++;
                if (Chances > 0 && OverallResult.Failures >= Chances)
                {
                    OverallResult.ResultType = MissionResultType.Failure;
                }
                break;
            case FinishState.Timeout:
                OverallResult.ResultType = MissionResultType.Timeout;
                break;
            case FinishState.Quit:
                OverallResult.ResultType = MissionResultType.Quit;
                break;
        }

        Round++;
    }

    public void AdjustTime(RoundResultInfo roundResult)
    {
        float adjustment = 0;
        switch (MissionType)
        {
            case MissionTypes.Sprint:
            case MissionTypes.ByRound:
                break;
            case MissionTypes.Survival:
                if (roundResult.State == FinishState.Right)
                {
                    adjustment = BaseRecoverySeconds;
                }
                else if (roundResult.State == FinishState.Wrong)
                {
                    adjustment = BaseRecoverySeconds * -.5f;
                }
                OverallResult.TimeRemaining += adjustment;
                break;
        }
        roundResult.TimeAdjustment = adjustment;
    }
    public float TimeExchangeRate = 1f;
    public void HandlePoints(RoundResultInfo roundResult)
    {
        var m = roundResult.MyMission;
        switch (m.MissionType)
        {
            case MissionTypes.Sprint:
                // time limit counts down to a specified amount of rounds
                switch (roundResult.State)
                {
                    case FinishState.Right:
                        var timeSlice = m.TotalSeconds / m.Rounds;
                        var roundProgress = roundResult.Round / m.Rounds;
                        var baseTimeSlice = (timeSlice * m.TimeExchangeRate);
                        roundResult.RoundLoot.Time += (baseTimeSlice + (baseTimeSlice * roundProgress));
                        break;
                    case FinishState.Wrong:
                        break;
                }
                break;
            case MissionTypes.ByRound:
                // you start with a given amount each round, for a specified number of rounds
                var remainingTime = m.OverallResult.MissionTime;
                var remainingPercentage = 1 - m.OverallResult.Progress;
                switch (roundResult.State)
                {
                    case FinishState.Right:
                        roundResult.RoundLoot.Time += (remainingTime * m.TimeExchangeRate);
                        break;
                    case FinishState.Wrong:
                        break;
                }
                break;
            case MissionTypes.Survival:
                // rename this one.  maybe time attack or something
                // real survival could be pay a certain amount of time, then use that time to get as high a combo as possible, or a time target for 
                // bonuses (tokens, coins)

                // you pay an initial fee of time to play.  then you get the chance to earn back
                // that time and more, if you play well enough.  But this scoring only happens at the end of the mission
                switch (roundResult.State)
                {
                    case FinishState.Right:
                        break;
                    case FinishState.Wrong:
                        break;
                }
                break;
        }
    }

    public void HandleCombo(RoundResultInfo roundResult)
    {
        switch (roundResult.State)
        {
            case FinishState.Right:
                OverallResult.Combo++;
                break;
            case FinishState.Wrong:
                OverallResult.Combo = 0;
                break;
            case FinishState.Timeout:
            case FinishState.Quit:
                break;
        }
    }

    public void Complete()
    {
        switch (MissionType)
        {
            case MissionTypes.Sprint:
            case MissionTypes.ByRound:
                break;
            case MissionTypes.Survival:
                // take whatever time is left, and let that be their reward.
                OverallResult.EarnedLoot.Time += (OverallResult.MissionTime * TimeExchangeRate);
                break;
        }
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
            CanWager = false,
            PrizePurse = new Wallet() { Time = 30, Coins = 1, Tokens = 10 }
        };

        var m2 = new Mission()
        {
            StageInfo = stage,
            MissionType = MissionTypes.Sprint,
            TotalSeconds = 60f,
            Rounds = 20,
            Chances = 1,
            CanWager = false,
            PrizePurse = new Wallet() { Time = 30, Coins = 1, Tokens = 10 }
        };

        var m3 = new Mission()
        {
            StageInfo = stage,
            MissionType = MissionTypes.Survival,
            Rounds = 0,
            TotalSeconds = 5f,
            BaseRecoverySeconds = 1f, // milliseconds, I think
            CanWager = false,
            PrizePurse = new Wallet() { Time = 0, Coins = 0, Tokens = 10 }
        };

        var m4 = new Mission()
        {
            StageInfo = stage,
            MissionType = MissionTypes.ByRound,
            Rounds = 10,
            CanWager = false,
            PrizePurse = new Wallet() { Time = 30, Coins = 1, Tokens = 10 }
        };

        return new Mission[] { m1, m2, m3, m4 };
    }
}
