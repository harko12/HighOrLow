using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace HighOrLow
{
    public enum FinishState { Right, Wrong, Timeout, Quit, None };

    /// <summary>
    /// mission event class
    /// </summary>
    [System.Serializable]
    public class MissionEvent : UnityEvent<string, Mission>
    {
    }

    /// <summary>
    /// mission event class
    /// </summary>
    [System.Serializable]
    public class GameStageEvent : UnityEvent<string, GameStage>
    {
    }

    /// <summary>
    /// Timer update information class
    /// </summary>
    public class TimeUpdateInfo
    {
        public float RemainingTime { get; set; }
        public float Progress { get; set; }
        public bool UseTotalTime { get; set; }

        public void SetValues(float r, float p, bool u)
        {
            RemainingTime = r;
            Progress = p;
            UseTotalTime = u;
        }
    }

    /// <summary>
    /// Timer update event class
    /// </summary>
    [System.Serializable]
    public class TimerEvent : UnityEvent<string, TimeUpdateInfo>
    {
    }

    [System.Serializable]
    public class TimeAdjustedEvent : UnityEvent<string, float>
    {

    }

    [System.Serializable]
    public class RoundResultInfo
    {
        public Mission MyMission { get; set; }
        public FinishState State { get; set; }
        public float TimeAdjustment { get; set; }
        public int Round { get; set; }
        public Wallet RoundLoot { get; set; }

        public RoundResultInfo()
        {

        }

        public RoundResultInfo(Mission m)
        {
            MyMission = m;
            Reset(0);
        }

        public void Reset(int round)
        {
            Round = round;
            RoundLoot = new Wallet();
            State = FinishState.None;
        }
    }

    [System.Serializable]
    public class RoundEndEvent : UnityEvent<string, RoundResultInfo>
    {

    }

    [System.Serializable]
    public class RoundStartEvent : UnityEvent<string, GameProgression.GameRound>
    {

    }

    [System.Serializable]
    public class MonitoredEvent : UnityEvent<string>
    {

    }


    public class GameEventManager : ScriptableObject
    {
        [MenuItem("Assets/More||Less/Create/GameEventManager")]
        public static void CreateAsset()
        {
            var newObject = ScriptableObjectUtility.CreateAsset<GameEventManager>("Assets/HigherOrLower");
            newObject.name = "Game Event Manager ##";
        }

        // events
        [SerializeField]
        public GameStageEvent OnStageClicked;
        public const string OnStageClickedKey = "HighLowGame.OnStageClicked";
        public string OnStageClickedInvoke(GameStage stage)
        {
            OnStageClicked.Invoke(OnStageClickedKey, stage);
            return OnStageClickedKey;
        }

        [SerializeField]
        public RoundStartEvent OnRoundStart;
        private const string OnRoundStartKey = "HighLowGame.OnRoundStart";
        public string OnRoundStartInvoke(GameProgression.GameRound round)
        {
            OnRoundStart.Invoke(OnRoundStartKey, round);
            return OnRoundStartKey;
        }
        [SerializeField]
        public RoundEndEvent OnRoundEnd;
        private const string OnRoundEndKey = "HighLowGame.OnRoundEnd";
        public string OnRoundEndInvoke(RoundResultInfo result)
        {
            OnRoundEnd.Invoke(OnRoundEndKey, result);
            return OnRoundEndKey;
        }

        [SerializeField]
        public MissionEvent OnMissionChosen;
        private const string OnMissionChosenKey = "HighLowGame.OnMissionChosen";
        public string OnMissionChosenInvoke(Mission m)
        {
            OnMissionChosen.Invoke(OnMissionChosenKey, m);
            return OnMissionChosenKey;
        }

        [SerializeField]
        public MonitoredEvent OnStageStart;
        private const string OnStageStartKey = "HighLowGame.OnStageStart";
        public string OnStageStartInvoke()
        {
            OnStageStart.Invoke(OnStageStartKey);
            return OnStageStartKey;
        }

        [SerializeField]
        public MissionEvent OnStageEnd;
        private const string OnStageEndKey = "HighLowGame.OnStageEnd";
        public string OnStageEndInvoke(Mission m)
        {
            OnStageEnd.Invoke(OnStageEndKey, m);
            return OnStageEndKey;
        }

        [SerializeField]
        public TimerEvent OnTimerUpdate;
        private const string OnTimerUpdateKey = "HighLowGame.OnTimerUpdate";
        public string OnTimerUpdateInvoke(TimeUpdateInfo t)
        {
            OnTimerUpdate.Invoke(OnTimerUpdateKey, t);
            return OnTimerUpdateKey;
        }
        /*
        [SerializeField]
        public TimeAdjustedEvent OnTimeAdjusted;
        private const string OnTimeAdjustedKey = "HighLowGame.OnTimeAdjusted";
        public string OnTimerAdjustedInvoke(TimeUpdateInfo t)
        {
            OnTimerAdjusted.Invoke(OnTimerUpdateKey, t);
            return OnTimerUpdateKey;
        }
        */
    }
}

