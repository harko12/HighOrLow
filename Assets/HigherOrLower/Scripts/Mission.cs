﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

namespace HighOrLow
{

    public enum MissionTypes
    {
        /// <summary>
        /// gameplay: beginning with a set amount of time, the timer runs down as you play.  Getting a right answer adds more time, getting a wrong answer reduces the timer
        /// note: there is a falloff of the amount of time you get back that uses a projected amount of rounds until it gets 'really' hard.  can also use the chances to calculate
        /// cost: 
        /// </summary>
        Survival,
        /// <summary>
        /// gameplay: progresses for a set amount of rounds.  You start with a set amount of time, and have that much time to complete the assignment
        /// cost: 
        /// </summary>
        Sprint,
        /// <summary>
        /// gameplay: progresses for a set amount of rounds.  Each round starts a fresh timer, based on baserecoveryseconds
        /// cost: 
        /// </summary>
        ByRound
    }
    public enum MissionResultType { _Unknown, Complete, Timeout, Failure, Quit }
    [System.Serializable]
    public class MissionResult
    {
        public Wallet EarnedLoot { get; set; }
        public int Failures { get; set; }
        public int MaxStreak { get; set; }
        public int Combo { get; set; }
        public int NearMisses { get; set; }
        public float StartTime { get; set; }
        public float TimeRemaining { get; set; }
        public float RoundTime { get; set; }
        public float MissionTime { get; set; }
        public float Progress { get; set; }
        public bool Succeeded { get; set; }
        public float RecoveryFalloff { get; set; }
        public MissionResultType ResultType { get; set; }

        public MissionResult()
        {
            EarnedLoot = new Wallet();
        }

        public void Reset()
        {
            EarnedLoot.Wipe();
            Combo = 0;
            MaxStreak = 0;
            Failures = 0;
            NearMisses = 0;
            TimeRemaining = 0f;
            RoundTime = 0f;
            MissionTime = 0f;
            Progress = 0f;
            Succeeded = false;
            ResultType = MissionResultType._Unknown;
        }

    }

    [System.Serializable]
    public class Mission
    {
        public const int NO_TIMER_VALUE = -9999;
        public float MIN_TIMER_VALUE = 1;
        public int MAX_CHANCES = 5;
        public int MIN_ROUNDS = 10;
        public float MIN_BYROUND_TIME = .5f;
        public float MAX_RECOVERY_SECONDS = 1f;
        public float MIN_RECOVERY_SECONDS = .1f;
        public float MAX_RECOVERY_FALLOFF = .05f;
        public float MIN_RECOVERY_FALLOFF = .001f;

        public MissionTypes MissionType;
        public GameStage StageInfo { get; set; }
        public float TotalSeconds;
        public float TimeExchangeRate = 1f;
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
        public float BaseRecoverySeconds;
        /// <summary>
        /// Recovery time adjusted per round
        /// </summary>
        private float RecoverySeconds;
        /// <summary>
        /// the amount by which the recoverytime decreases every round.  Higher the number, 
        /// the less time you get back
        /// </summary>
        public float BaseRecoveryFalloff = 0;
        private float RecoveryFalloff;

        /// <summary>
        /// Interval at which to deliver a combo reward
        /// </summary>
        /// <remarks>also used as the a way to get the multiplier for the combo reward</remarks>
        public int RewardComboInterval = 0;
        private int ComboMultiplier = 0;

        public int Rounds;
        public int Chances;
        public bool CanWager;
        public int Wager { get; set; }
        [SerializeField]
        public Wallet Cost, PrizePurse, ComboReward;
        public MissionResult OverallResult { get; set; }

        public Mission()
        {
            TotalSeconds = NO_TIMER_VALUE;
            OverallResult = new MissionResult();
        }

        /// <summary>
        /// Validate mission and fix up any values if possible
        /// </summary>
        public void Validate()
        {
            BaseRecoveryFalloff = Mathf.Clamp(BaseRecoveryFalloff, MIN_RECOVERY_FALLOFF, MAX_RECOVERY_FALLOFF);
            Chances = Mathf.Clamp(Chances, 0, MAX_CHANCES);
            switch (MissionType)
            {
                case MissionTypes.ByRound:
                    if (BaseRecoverySeconds <= MIN_BYROUND_TIME)
                    {
                        BaseRecoverySeconds = MIN_BYROUND_TIME;
                    }
                    if (Rounds <= MIN_ROUNDS)
                    {
                        Rounds = MIN_ROUNDS;
                    }
                    break;
                case MissionTypes.Sprint:
                    if (Rounds <= MIN_ROUNDS)
                    {
                        Rounds = MIN_ROUNDS;
                    }
                    var timerMin = MIN_TIMER_VALUE * Rounds;
                    if (TotalSeconds <= timerMin)
                    {
                        TotalSeconds = timerMin;
                    }
                    break;
                case MissionTypes.Survival:
                    if (TotalSeconds <= MIN_TIMER_VALUE)
                    {
                        TotalSeconds = MIN_TIMER_VALUE;
                    }
                    BaseRecoverySeconds = Mathf.Clamp(BaseRecoverySeconds, MIN_RECOVERY_SECONDS, MAX_RECOVERY_SECONDS);
                    break;
            }
        }

        public float Difficulty()
        {
            Validate();
            var difficulty = 0f;
            float chanceDifficulty = Chances == 0 ? 0f : ((float)MAX_CHANCES - (float)Chances) / (float)MAX_CHANCES;
            var chanceWeight = chanceDifficulty * .5f;
            var timeWeight = 0f;
            float timeDifficulty01, timeDifficulty02;
            switch (MissionType)
            {
                case MissionTypes.ByRound:
                    timeDifficulty01 = StageInfo.Complexity / BaseRecoverySeconds;
                    timeWeight = 1 - chanceWeight;
                    difficulty = (chanceDifficulty * chanceWeight) + (timeDifficulty01 * timeWeight);
                    break;
                case MissionTypes.Sprint:
                    var secondsPerRound = TotalSeconds / Rounds;
                    timeDifficulty01 = StageInfo.Complexity / secondsPerRound;
                    if (timeDifficulty01 < 1) // eventually this difficulty flattens out, since seconds per round can't be < 1.  
                                              // if the stage isn't complex, then the diffuculty will never get above a certain level, and it may not reach the target.
                    {
                        chanceWeight = .95f;
                    }
                    timeWeight = 1 - chanceWeight;
                    difficulty = (chanceDifficulty * chanceWeight) + (timeDifficulty01 * timeWeight);
                    break;
                case MissionTypes.Survival:
                    timeDifficulty01 = StageInfo.Complexity / BaseRecoverySeconds;
                    // because the max recovery falloff is .05,
                    // we see how this one compares to that (1 most difficult, 0 least difficult)
                    timeDifficulty02 = (BaseRecoveryFalloff * 100) / 5;
                    Vector2 weight = new Vector2(.5f, .5f);
                    if (BaseRecoveryFalloff < .1f)
                    {
                        weight.x = .8f;
                        weight.y = .2f;
                    }
                    difficulty = (timeDifficulty01 * weight.x) + (timeDifficulty02 * weight.y);
                    break;
            }
            return Mathf.Round(difficulty * 1000f) * .001f;
        }

        private void AdjustChances(int value)
        {
            if (value < 0)
            {
                if (Chances == 0) Chances = MAX_CHANCES;
                else Chances += value;
                if (Chances == 0) Chances = 1; // can't reduce to 0
            }
            else if (value > 0)
            {
                if (Chances == MAX_CHANCES) Chances = 0; // if you add to the max, you just get 0 (unlimited) chances
                else if (Chances > 0)
                {
                    Chances += value;
                }
            }
        }

        public void AdjustDifficulty(float targetDifficulty)
        {
            var currentDiff = Difficulty();
            var direction = targetDifficulty - currentDiff;
            var diff = Mathf.Abs(direction);
            int multiplier = direction < 0 ? -1 : 1;
            int timeMultiplier = -1 * multiplier; // to get easier, we must add time, not subtract
            int chancesMultiplier = -1 * multiplier; // we need to add chances to make it easier
            int sprintRoundsMultiplier = multiplier; // fewer rounds make sprints easier
            int recoveryFalloffMultiplier = multiplier;
            float variance = .05f;
            int iterations = 0;
            var done = false;
            while (!done)
            {
                switch (MissionType)
                {
                    case MissionTypes.ByRound:
                        variance = .2f;
                        BaseRecoverySeconds += (timeMultiplier * .5f);
                        if (iterations % 10 == 0)
                        {
                            AdjustChances(chancesMultiplier * 1);
                        }
                        break;
                    case MissionTypes.Sprint:
                        variance = .2f;
                        if (diff > .75f)
                        {
                            AdjustChances(chancesMultiplier * 1);
                            // adjust chances first, then start adjusting time
                            if (iterations % 5 == 0)
                            {
                                // once the rounds and seconds hit parity, there is no point making 
                                //them more difficult.  It won't change the number.
                                if (direction < 0 || Rounds != TotalSeconds)
                                {
                                    Rounds += sprintRoundsMultiplier * 1;
                                    if (iterations % 5 == 0)
                                    {
                                        TotalSeconds += timeMultiplier * .5f;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // once the rounds and seconds hit parity, there is no point making 
                            //them more difficult.  It won't change the number.
                            if (Rounds != TotalSeconds)
                            {
                                Rounds += sprintRoundsMultiplier * 1;
                                if (iterations % 5 == 0)
                                {
                                    TotalSeconds += timeMultiplier * .5f;
                                }
                            }
                            // adjust chances first, then start adjusting time
                            if (iterations % 10 == 0)
                            {
                                AdjustChances(chancesMultiplier * 1);
                            }
                        }
                        break;
                    case MissionTypes.Survival:
                        BaseRecoveryFalloff += recoveryFalloffMultiplier * .001f;
                        if (iterations % 5 == 0)
                        {
                            BaseRecoverySeconds += timeMultiplier * .05f;
                        }
                        break;
                }
                var newDifficulty = Difficulty();
                diff = Mathf.Abs(targetDifficulty - newDifficulty);
                if (((-1 * variance) < diff && diff < variance)
                    || iterations > 500)
                {
                    done = true;
                }
                iterations++;
            }
        }

        public override string ToString()
        {
            var result = "";
            if (StageInfo == null) return result;
            switch (MissionType)
            {
                case MissionTypes.ByRound:
                    result = string.Format("{0}({4}), Round Time: {1}, rounds: {2}, chances: {3}", MissionType, BaseRecoverySeconds, Rounds, Chances, StageInfo.Complexity);
                    break;
                case MissionTypes.Sprint:
                    result = string.Format("{0}({4}), total seconds: {1}, rounds: {2}, chances: {3}", MissionType, TotalSeconds, Rounds, Chances, StageInfo.Complexity);
                    break;
                case MissionTypes.Survival:
                    result = string.Format("{0}({2}), starting seconds: {1}", MissionType, TotalSeconds, StageInfo.Complexity);
                    break;
            }
            return result;
        }

        public string Description(bool showPurse = false)
        {
            var sb = new StringBuilder();
            sb.AppendLine(ToString());
            sb.AppendLine();
            sb.AppendFormat("Diff {0}, Stage Complexity {1}", Difficulty(), StageInfo.Complexity);
            if (showPurse && PrizePurse != null)
            {
                sb.AppendLine();
                sb.AppendLine(string.Format("Purse: {0}", PrizePurse.ToString()));
            }
            return sb.ToString();
        }
        /// <summary>
        /// Adjust time values and store results in mission result
        /// </summary>
        /// <param name="roundData"></param>
        public void TimerTick(GameProgression.GameRound roundData)
        {
            // ongoing countdown from the time the mission started (may be unused)
            OverallResult.TimeRemaining = Mathf.Clamp(OverallResult.TimeRemaining - Time.deltaTime, 0, 9999);
            var overallProgress = (!UseOverallTime ? 0 : (TotalSeconds - OverallResult.TimeRemaining) / TotalSeconds);
            // time currently spent in this round
            OverallResult.RoundTime = Time.time - OverallResult.StartTime;
            var elapsedRoundTime = Mathf.Clamp(roundData.WaitSeconds - OverallResult.RoundTime, 0, 9999);
            var roundProgress = (roundData.WaitSeconds <= 0 ? 0 : OverallResult.RoundTime / roundData.WaitSeconds);

            switch (MissionType)
            {
                case MissionTypes.Sprint:
                    OverallResult.Progress = overallProgress;
                    OverallResult.MissionTime = OverallResult.TimeRemaining;
                    break;
                case MissionTypes.Survival:
                    OverallResult.MissionTime = OverallResult.TimeRemaining;
                    OverallResult.Progress = OverallResult.MissionTime > 0 ? 0 : 1; // progress stays at 0 until time runs out
                    break;
                case MissionTypes.ByRound:
                    OverallResult.Progress = roundProgress;
                    OverallResult.MissionTime = elapsedRoundTime;
                    break;
            }
        }

        /// <summary>
        /// Process the round result and determing if we need to continue
        /// </summary>
        /// <param name="roundResult"></param>
        /// <param name="Round"></param>
        /// <returns>boolean whether or not to continue the mission</returns>
        public bool ProcessRoundResult(RoundResultInfo roundResult, ref int Round)
        {
            var continueMission = true;
            HandleCombo(roundResult);
            // adjust some time, based on the result
            AdjustTime(roundResult);
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

            // check for a resulttype (unknown means it's still playing, or the max rounds being exceeded
            if (OverallResult.ResultType != MissionResultType._Unknown
                || RoundsExceeded(Round, Rounds))
            {
                continueMission = false;
            }

            Round++;
            return continueMission;
        }

        /// <summary>
        /// Determine if the currentRound has passed the allowed limit for this mission
        /// </summary>
        /// <param name="currentRound"></param>
        /// <param name="maxRounds"></param>
        /// <returns></returns>
        public bool RoundsExceeded(int currentRound, int maxRounds)
        {
            var exceeded = false;
            switch (MissionType)
            {
                case MissionTypes.Sprint:
                case MissionTypes.ByRound:
                    exceeded = maxRounds != 0 && currentRound > maxRounds;
                    break;
                case MissionTypes.Survival: // survival is not by round, but it does use the rounds to adjust its time calculations
                    exceeded = false;
                    break;
            }
            return exceeded;
        }

        /// <summary>
        /// Calculate the current combo value, update the combomultipier if applicable
        /// </summary>
        /// <param name="roundResult"></param>
        public void HandleCombo(RoundResultInfo roundResult)
        {
            ComboMultiplier = 0;
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
                    OverallResult.Combo = 0;
                    break;
            }

            if (OverallResult.Combo > OverallResult.MaxStreak)
            {
                OverallResult.MaxStreak = OverallResult.Combo;
            }
            if (RewardComboInterval > 0)
            {
                if (OverallResult.Combo % RewardComboInterval == 0)
                {
                    ComboMultiplier = OverallResult.Combo / RewardComboInterval;
                    OverallResult.EarnedLoot += (ComboReward * ComboMultiplier);
                    Debug.LogFormat("ComboMultiplier set to {0}, added ", ComboMultiplier, (ComboReward * ComboMultiplier));
                }
            }
        }

        /// <summary>
        /// Apply any adjustments to the time remaining that may apply for this mission based on the round result
        /// </summary>
        /// <param name="roundResult"></param>
        public void AdjustTime(RoundResultInfo roundResult)
        {
            float adjustment = 0;
            switch (MissionType)
            {
                case MissionTypes.Sprint:
                case MissionTypes.ByRound:
                    break;
                case MissionTypes.Survival:
                    adjustment = GetTimeAdjustmentForSurvival(roundResult.State, roundResult.Round);
                    OverallResult.TimeRemaining += adjustment;
                    break;
            }
            roundResult.TimeAdjustment = adjustment;
        }

        /// <summary>
        /// Get the time adjustment for Survival mode
        /// </summary>
        /// <param name="answered"></param>
        /// <remarks>It is intended to have a slowly decreasing return as you play, to increase difficulty</remarks>
        /// <returns></returns>
        public float GetTimeAdjustmentForSurvival(FinishState answered, int round)
        {
            float adjustment = 0f;

            // based on the max rounds, we want to adjust the falloff such that
            // by the time the target round is reached, no time is being added.  At that point
            // they are on skill and combos alone.
            var falloff = ((float)round / (float)Rounds) * BaseRecoverySeconds;
            adjustment = BaseRecoverySeconds - falloff;

            if (answered == FinishState.Right)
            {
            }
            else if (answered == FinishState.Wrong)
            {
                adjustment *= -.5f; // cut them some slack for messing up ?
            }
//            Debug.LogFormat("round {0} survival adjustment time: {1}", round, adjustment);
            return adjustment;
        }

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

    }
}
