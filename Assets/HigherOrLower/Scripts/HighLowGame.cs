using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using HarkoGames;

/// <summary>
/// mission event class
/// </summary>
[System.Serializable]
public class MissionEvent : UnityEvent<string, Mission>
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
public class MonitoredEvent : UnityEvent<string>
{

}

public enum FinishState { Right, Wrong, Timeout, Quit, None };

public class HighLowGame : MonoBehaviour {

    private static HighLowGame mInstance;
    private bool updateTimer;

    public static HighLowGame GetInstance()
    {
        return mInstance;
    }

    public TimeUpdateInfo currentTimeValues { get; set; }

    public enum ButtonState { NONE = 0, LEFT = 1, MID = 2, RIGHT = 3 };
    public LEDPanel LED1, LED2;

    public Text Line1, Line2;

    public Button ButtonLeft, ButtonMid, ButtonRight;
    public int Round;

    [SerializeField]
    public MonitoredEvent OnRoundStart;
    private const string OnRoundStartKey = "HighLowGame.OnRoundStart";
    [SerializeField]
    public RoundEndEvent OnRoundEnd;
    private const string OnRoundEndKey = "HighLowGame.OnRoundEnd";
    [SerializeField]
    public MonitoredEvent OnStageChosen;
    private const string OnStageChosenKey = "HighLowGame.OnStageChosen";
    [SerializeField]
    public MonitoredEvent OnStageStart;
    private const string OnStageStartKey = "HighLowGame.OnStageStart";
    [SerializeField]
    public MissionEvent OnStageEnd;
    private const string OnStageEndKey = "HighLowGame.OnStageEnd";

    [SerializeField]
    public TimerEvent OnTimerUpdate;
    private const string OnTimerUpdateKey = "HighLowGame.OnTimerUpdate";
    [SerializeField]
    public TimeAdjustedEvent OnTimeAdjusted;
    private const string OnTimeAdjustedKey = "HighLowGame.OnTimeAdjusted";

    public ProgressionPanel ProgressPanel;
    public GameStageContainer Levels;

    private GamePlayer PlayerData = new GamePlayer();

    private Mission[] currentMissions;

    private int currentMissionIndex;

    public void SetCurrentMissions(Mission[] missions)
    {
        currentMissions = missions;
    }

    public Mission GetCurrentMission()
    {
        return currentMissions[currentMissionIndex];
    }

    private void Awake()
    {
        mInstance = this;
    }

    private void Start()
    {
        PlayerData.Pull();
        //PlayerData.Wipe();// for debugging until the ui is done
        if (PlayerData.myWallet.IsEmpty())
        {
            PlayerData.myWallet += new Wallet() { Coins = 0, Time = 60, Tokens = 10 }; // for now, some starting capitol
        }
        if (PlayerData.Level == 0) PlayerData.Level = 1;
        if (PlayerData.Stage == 0) PlayerData.Stage = 1;
        PlayerData.Push();
        GameProgression.InitGameProgression(this.Levels);
        ProgressPanel.Init(this.Levels, PlayerData);
        currentTimeValues = new TimeUpdateInfo();
        ChooseMission();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.LeftArrow)) { SetButtonState(1); }
        else if (Input.GetKeyUp(KeyCode.DownArrow)) { SetButtonState(2); }
        else if (Input.GetKeyUp(KeyCode.RightArrow)) { SetButtonState(3); }
    }

    private void FixedUpdate()
    {
        if (updateTimer)
        {
            GetCurrentMission().TimerTick(mCurrentRoundData);
        }
        
    }

    private ButtonState _buttonState = ButtonState.NONE;

    public void SetButtonState(int state)
    {
        _buttonState = (ButtonState)state;
       // Debug.LogFormat("{0} pushed.", _buttonState.ToString());
    }

    public void ClearButtonState() // really just clears the buttons
    {
        _buttonState = ButtonState.NONE;
    }

    public GamePlayer Getplayer()
    {
        return PlayerData;
    }

    private void ChooseMission()
    {
        ProgressPanel.ToggleMenu(); // bring in progress panel
    }

    public void StartMission(int index)
    {
        if (index >= currentMissions.Count())
        {
           // Debug.LogFormat("Invalid mission index chosen {0}", index);
            return;
        }
        //Debug.LogFormat("Starting {0}", currentMissions[index].ToString());
        currentMissionIndex = index;
        StartCoroutine(Game());
    }

    private Mission SetCurrentMission(int index)
    {
        var m = currentMissions[index];
        PlayerData.SetLevelAndStage(m.StageInfo.Level, m.StageInfo.Stage);
        return m;
    }

    public IEnumerator StageStart()
    {
        OnStageStart.Invoke(OnStageStartKey);
        while (EventMonitor.IsRunning(OnStageStartKey))
        {
            yield return null;
        }
        yield return null;
    }

    public IEnumerator StageComplete(Mission m, RoundResultInfo result)
    {
        OnStageEnd.Invoke(OnStageEndKey, m);
        while (EventMonitor.IsRunning(OnStageEndKey))
        {
            yield return null;
        }
        yield return null;
    }

    public IEnumerator RoundStart()
    {
        OnRoundStart.Invoke(OnRoundStartKey);
        while (EventMonitor.IsRunning(OnRoundStartKey))
        {
            yield return null;
        }
        yield return null;
    }

    public IEnumerator RoundComplete(Mission m, RoundResultInfo result)
    {
        OnRoundEnd.Invoke(OnRoundEndKey, result);
        while (EventMonitor.IsRunning(OnRoundEndKey))
        {
            yield return null;
        }
        yield return null;

    }

    public IEnumerator TimerUpdate(TimeUpdateInfo t)
    {
        OnTimerUpdate.Invoke(OnTimerUpdateKey, t);
        while (EventMonitor.IsRunning(OnTimerUpdateKey))
        {
            yield return null;
        }
        yield return null;
    }


    public void MissionComplete(string eventId, Mission m)
    {
        m.Complete();
        var earned = m.OverallResult.EarnedLoot;
        switch (m.OverallResult.ResultType)
        {
            case MissionResultType.Failure:
            case MissionResultType.Quit:
                break;
            case MissionResultType.Timeout:
                break;
            case MissionResultType.Success:
                PlayerData.myWallet += m.PrizePurse;
                PlayerData.myWallet += earned;
                PlayerData.Push();
                break;
        }
        ProgressPanel.UpdateStages(PlayerData);
        ProgressPanel.ToggleMenu();
        StopCoroutine("Game");
        // clean up global game variables
        Round = 1;
    }

    public void onStageSetup(string eventId)
    {
        LED1.ClearLED();
        LED2.ClearLED();
        LcdWrite("", "");
    }

    private IEnumerator Instructions()
    {
        LcdWrite("Welcome!", "Read Closely");
        yield return new WaitForSeconds(2);
        LcdWrite("Look Closely", "At the LEDs");
        yield return new WaitForSeconds(2);
        LcdWrite("If it says high-", "er, pick higher.");
        yield return new WaitForSeconds(2);
        LcdWrite("Lower, pick the ", "Lower number.");
        yield return new WaitForSeconds(2);
        LcdWrite("Easy", "      Peasy.");
        yield return new WaitForSeconds(2);
    }
    private GameProgression.GameRound mCurrentRoundData;
    private IEnumerator Game()
    {
        var continueMission = true;
        var currentMission = SetCurrentMission(currentMissionIndex);
        currentMission.OverallResult.ResultType = MissionResultType._Unknown;
        currentMission.OverallResult.TimeRemaining = 0f;
        if (currentMission.UseOverallTime)
        {
            currentMission.OverallResult.TimeRemaining = currentMission.TotalSeconds;
        }

        // coroutine to show deducting the cost
        PlayerData.myWallet -= currentMission.Cost;
        ProgressPanel.ToggleMenu();

        yield return StartCoroutine(StageStart());

        var roundResult = new RoundResultInfo(currentMission);

        switch (currentMission.MissionType)
        {
            case MissionTypes.Sprint:
                break;
            case MissionTypes.Survival:
                break;
        }
        Round = 1;
        while (continueMission)
        {
            mCurrentRoundData = GameProgression.GetRound(PlayerData.Level, PlayerData.Stage, Round, LED1.LEDArraySize);
            yield return StartCoroutine(RoundStart());
            var highMessage = mCurrentRoundData.High ? "     HIGHER!   " : "     LOWER!   "; // change this to an event or setter call 
            ButtonState correctBtn = mCurrentRoundData.Led1Right ? ButtonState.LEFT : ButtonState.RIGHT;
            ButtonState incorrectBtn = correctBtn == ButtonState.LEFT ? ButtonState.RIGHT : ButtonState.LEFT;
            LEDRendering.Graph(mCurrentRoundData.LEDInfo[0].graphType, LED1, 
                mCurrentRoundData.LEDInfo[0].StartX, mCurrentRoundData.LEDInfo[0].StartY, 
                mCurrentRoundData.LEDInfo[0].Count,
                mCurrentRoundData.LEDInfo[0].InvertX, mCurrentRoundData.LEDInfo[0].InvertY);

            LEDRendering.Graph(mCurrentRoundData.LEDInfo[1].graphType, LED2,
                mCurrentRoundData.LEDInfo[1].StartX, mCurrentRoundData.LEDInfo[1].StartY,
                mCurrentRoundData.LEDInfo[1].Count,
                mCurrentRoundData.LEDInfo[1].InvertX, mCurrentRoundData.LEDInfo[1].InvertY);

            float timeAdjustment = roundResult.TimeAdjustment;
            roundResult.Reset(Round);

            currentMission.OverallResult.StartTime = Time.time;
            var loop = true;
            ClearButtonState();
            var _gameState = FinishState.None;
            int progressDashCount = 16;
            updateTimer = true;
            while (loop)
            {
                //currentMission.TimerTick(mCurrentRoundData);

                currentTimeValues.SetValues(currentMission.OverallResult.MissionTime, currentMission.OverallResult.Progress, currentMission.UseOverallTime);
                yield return StartCoroutine(TimerUpdate(currentTimeValues));
                progressDashCount = 16 - (Mathf.RoundToInt(16 * currentMission.OverallResult.Progress));
                var progressMessage = progressDashCount > 0 ? new string('-', progressDashCount) : "";
                LcdWrite(highMessage, progressMessage);
                //              Debug.LogFormat("progress: {0}", progress);
                if (currentMission.OverallResult.Progress >= 1)
                {
                    loop = false;
                    _gameState = FinishState.Timeout;
                    continue;
                }
                if (_buttonState == ButtonState.MID)
                {
                    //Debug.Log("exit");
                    loop = false;
                    _gameState = FinishState.Quit;
                }
                else if (_buttonState == correctBtn)
                {
                    //Debug.Log("right");
                    loop = false;
                    _gameState = FinishState.Right;
                }
                else if (_buttonState == incorrectBtn)
                {
                    //Debug.Log("wrong");
                    loop = false;
                    _gameState = FinishState.Wrong;
                }
                yield return null;
            }
            updateTimer = false;
            _buttonState = ButtonState.NONE;
            roundResult.State = _gameState;

            continueMission = currentMission.ProcessRoundResult(roundResult, ref Round);
            yield return StartCoroutine(RoundComplete(currentMission, roundResult));
        }
        if (currentMission.OverallResult.ResultType == MissionResultType._Unknown)
        {
            // if at this point it's still unknown, then nothing bad happened.  Success!
            currentMission.OverallResult.ResultType = MissionResultType.Success;
        }
        yield return StartCoroutine(StageComplete(currentMission, roundResult));
        yield return null;
    }

    private void LcdWrite(string l1, string l2)
    {
        Line1.text = Line2.text = "";
        Line1.text = l1;
        Line2.text = l2;
    }

}
