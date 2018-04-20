using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class MissionEvent : UnityEvent<Mission>
{

}

public class HighLowGame : MonoBehaviour {

    private static HighLowGame mInstance;

    public static HighLowGame GetInstance()
    {
        return mInstance;
    }
    public TimeUpdateEventArgs currentTimeValues { get; set; }

    public enum ButtonState { NONE = 0, LEFT = 1, MID = 2, RIGHT = 3 };
    public LEDPanel LED1, LED2;

    public Text Line1, Line2;

    public Button ButtonLeft, ButtonMid, ButtonRight;
    public int Round;

    public UnityEvent OnRoundStart;
    public UnityEvent OnRoundEnd;
    public UnityEvent OnStageChosen;
    public UnityEvent OnStageStart;
    [SerializeField]
    public MissionEvent OnStageEnd;

    public class TimeUpdateEventArgs
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
    public UnityEvent OnTimerUpdate;

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
        PlayerData.Wipe();// for debugging until the ui is done
        if (PlayerData.Level == 0) PlayerData.Level = 1;
        if (PlayerData.Stage == 0) PlayerData.Stage = 1;
        PlayerData.Push();
        GameProgression.InitGameProgression(this.Levels);
        ProgressPanel.Init(this.Levels, PlayerData);
        currentTimeValues = new TimeUpdateEventArgs();
        ChooseMission();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.LeftArrow)) { SetButtonState(1); }
        else if (Input.GetKeyUp(KeyCode.DownArrow)) { SetButtonState(2); }
        else if (Input.GetKeyUp(KeyCode.RightArrow)) { SetButtonState(3); }
    }

    private void OnEnable()
    {
        //        StartCoroutine(GameLoop());
    }

    private ButtonState _buttonState = ButtonState.NONE;

    public void SetButtonState(int state)
    {
        _buttonState = (ButtonState)state;
        Debug.LogFormat("{0} pushed.", _buttonState.ToString());
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
            Debug.LogFormat("Invalid mission index chosen {0}", index);
            return;
        }
        Debug.LogFormat("Starting {0}", currentMissions[index].ToString());
        currentMissionIndex = index;
        ProgressPanel.ToggleMenu();
        //ProgressPanel.MissionView.ToggleMissions();
//        StartCoroutine(GameLoop(true));
        StartCoroutine(Game());
    }

    public void RoundComplete()
    {
        System.Threading.Thread.Sleep(2000);
    }

    public void MissionComplete(Mission m)
    {
        var earnedPoints = m.Result.EarnedPoints;
        switch (m.Result.ResultType)
        {
            case MissionResultType.Failure:
            case MissionResultType.Quit:
                break;
            case MissionResultType.Timeout:
                break;
            case MissionResultType.Success:
                earnedPoints += 1000;
                PlayerData.myWallet.Points += earnedPoints;
                PlayerData.Push();
                break;
        }
        ProgressPanel.UpdateStages(PlayerData);
        ProgressPanel.ToggleMenu();
        StopCoroutine("Game");
        // clean up global game variables
        Round = 1;
    }

    // start gameloop from mission start ?
    IEnumerator GameLoop()
    {
        //yield return StartCoroutine(Instructions());
        bool exit = false;
        ClearButtonState();
        while (!exit)
        {
            LcdWrite(string.Format("Stage: {0}", PlayerData.Stage), "Hit Enter to go!");
            switch (_buttonState)
            {
                case ButtonState.MID:
                    ClearButtonState();
                    Round = 1;
                    yield return StartCoroutine(Game());
                    break;
            }
            yield return new WaitForSeconds(.5f);
        }
        yield return null;
    }

    public void ResetGameLoop()
    {
        //StopCoroutine(Game());  // should 
    }

    private IEnumerator Countdown(int countdownStart)
    {
        for (int lcv = countdownStart; lcv >=0; lcv--)
        {
            Debug.LogFormat("counting down {0} .. ", lcv);
            yield return new WaitForSeconds(1);
        }
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

    private IEnumerator StageFailed()
    {
        LcdWrite(string.Format("Stage {0}", PlayerData.Stage), "Failed");
        yield return new WaitForSeconds(2);
        LcdWrite("Try", "Again!");
        yield return new WaitForSeconds(2);
    }

    private IEnumerator StageQuit()
    {
        LcdWrite(string.Format("Stage {0}", PlayerData.Stage), "Exiting");
        yield return new WaitForSeconds(2);
        LcdWrite("Returning to", "Menu");
        yield return new WaitForSeconds(2);
    }

    private IEnumerator StageComplete(int earnedPoints)
    {
        LcdWrite("Stage", "Complete!");
        yield return new WaitForSeconds(2);
        LcdWrite("You earned", string.Format("{0} Points", earnedPoints));
        yield return new WaitForSeconds(2);
        LcdWrite("You have", string.Format("{0} Points", PlayerData.myWallet.Points));
        yield return new WaitForSeconds(2);
        LcdWrite("You are", string.Format("Level {0}", PlayerData.Level));
        yield return new WaitForSeconds(2);
        yield return null;
    }

    public enum FinishState { Right, Wrong, Timeout, Quit, None};

    private IEnumerator Game()
    {
        yield return StartCoroutine(Countdown(3));
        var playingRound = true;
        OnStageStart.Invoke();
        float remainingTime = 0f;
        var currentMission = currentMissions[currentMissionIndex];
        currentMission.Result.ResultType = MissionResultType._Unknown;
        if (currentMission.UseOverallTime)
        {
            remainingTime = currentMission.TotalSeconds;
        }

        switch (currentMission.MissionType)
        {
            case MissionTypes.Sprint:
                break;
            case MissionTypes.Survival:
                break;
        }
        while (playingRound)
        {
            var roundData = GameProgression.GetRound(PlayerData.Level, PlayerData.Stage, Round, LED1.LEDArraySize);
            OnRoundStart.Invoke();
            var highMessage = roundData.High ? "     HIGHER!   " : "     LOWER!   "; // change this to an event or setter call 
            ButtonState correctBtn = roundData.Led1Right ? ButtonState.LEFT : ButtonState.RIGHT;
            ButtonState incorrectBtn = correctBtn == ButtonState.LEFT ? ButtonState.RIGHT : ButtonState.LEFT;
            LEDRendering.Graph(roundData.LEDInfo[0].graphType, LED1, 
                roundData.LEDInfo[0].StartX, roundData.LEDInfo[0].StartY, 
                roundData.LEDInfo[0].Count,
                roundData.LEDInfo[0].InvertX, roundData.LEDInfo[0].InvertY);

            LEDRendering.Graph(roundData.LEDInfo[1].graphType, LED2,
                roundData.LEDInfo[1].StartX, roundData.LEDInfo[1].StartY,
                roundData.LEDInfo[1].Count,
                roundData.LEDInfo[1].InvertX, roundData.LEDInfo[1].InvertY);

            float startTime = Time.time;
            var roundTime = roundData.WaitSeconds;
            var loop = true;
            ClearButtonState();
            var _gameState = FinishState.None;
            int progressDashCount = 16;
            while (loop)
            {
                float progress;
                // break out into function?
                if (currentMission.UseOverallTime) // the game is using an overall timer, and will end when it runs out (survival, sprint)
                {
                    remainingTime -= Time.deltaTime;
                    var elapsedTime = currentMission.TotalSeconds - remainingTime;
                    progress = elapsedTime / currentMission.TotalSeconds;
                    currentTimeValues.SetValues(remainingTime, progress, currentMission.UseOverallTime);
                }
                else // the game is using an ever diminishing round timer, that resets ever time, but always gives a little less.
                {
                    roundTime = Time.time - startTime;
                    var elapsedTime = roundData.WaitSeconds - roundTime;
                    progress = roundTime / roundData.WaitSeconds;
                    currentTimeValues.SetValues(elapsedTime, progress, currentMission.UseOverallTime);
                }


                OnTimerUpdate.Invoke();
                progressDashCount = 16 - (Mathf.RoundToInt(16 * progress));
                var progressMessage = progressDashCount > 0 ? new string('-', progressDashCount) : "";

                LcdWrite(highMessage, progressMessage);
                //              Debug.LogFormat("progress: {0}", progress);
                if (progress >= 1)
                {
                    loop = false;
                    _gameState = FinishState.Timeout;
                    continue;
                }
                if (_buttonState == ButtonState.MID)
                {
                    Debug.Log("exit");
                    loop = false;
                    _gameState = FinishState.Quit;
                }
                else if (_buttonState == correctBtn)
                {
                    Debug.Log("right");
                    loop = false;
                    _gameState = FinishState.Right;
                }
                else if (_buttonState == incorrectBtn)
                {
                    Debug.Log("wrong");
                    loop = false;
                    _gameState = FinishState.Wrong;
                }
                yield return null;
            }
            _buttonState = ButtonState.NONE;
            //Debug.Log("round complete");
            switch (_gameState)
            {
                case FinishState.Right:
                    LcdWrite("Great job", "");

                    // add some time
                    roundTime += 2; //TODO: make this a variable of the round, or maybe calculated  .  Perhaps a penalty for wrongness instead of ending the round.
                    remainingTime += currentMission.BaseRecoverySeconds; // add event for this
                    //
                    var earnedPoints = 100;
                    earnedPoints += (progressDashCount * 20);
                    currentMission.Result.EarnedPoints += earnedPoints;
                    Round++;
                    /*
                    if (Round > maxRounds) // TODO: make variable for max rounds
                    {
                        if (PlayerData.Stage > GameProgression.STAGE_COUNT)
                        {
                            PlayerData.Level++;
                            earnedPoints += 5000;
                            PlayerData.Stage = 1;
                        }
                        earnedPoints += 1000;
                        playingRound = false;
                        PlayerData.wallet.Points += earnedPoints;
                        PlayerData.Push();
                        ProgressPanel.UpdateStages(PlayerData);
                        yield return StartCoroutine(StageComplete(earnedPoints));
                    }
                    */
                    break;
                case FinishState.Wrong:
                    LcdWrite("you suck", "");
                    currentMission.Result.Failures++;
                    if (currentMission.Chances > 0 && currentMission.Result.Failures >= currentMission.Chances)
                    {
//                        playingRound = false;
                        currentMission.Result.ResultType = MissionResultType.Failure;
                    }
//                    playingRound = false;
//                    yield return StartCoroutine(StageFailed());
                    break;
                case FinishState.Timeout:
                    LcdWrite("Time's up", "");
                    //playingRound = false;
                    currentMission.Result.ResultType = MissionResultType.Timeout;
                    //yield return StartCoroutine(StageFailed());
                    break;
                case FinishState.Quit:
                    LcdWrite("Exiting", "");
                    //playingRound = false;
                    currentMission.Result.ResultType = MissionResultType.Quit;
                    //yield return StartCoroutine(StageQuit());
                    break;
            }
            // check for a resulttype (unknown means it's still playing, or the max rounds being exceeded
            if (currentMission.Result.ResultType != MissionResultType._Unknown 
                || currentMission.Rounds != 0 && Round > currentMission.Rounds)
            {
                playingRound = false;
            }
            OnRoundEnd.Invoke();
        }
        if (currentMission.Result.ResultType == MissionResultType._Unknown)
        {
            // if at this point it's still unknown, then nothing bad happened.  Success!
            currentMission.Result.ResultType = MissionResultType.Success;
        }
        OnStageEnd.Invoke(arg0: currentMission);
        yield return null;
    }

    private void LcdWrite(string l1, string l2)
    {
        Line1.text = Line2.text = "";
        Line1.text = l1;
        Line2.text = l2;
    }

}
