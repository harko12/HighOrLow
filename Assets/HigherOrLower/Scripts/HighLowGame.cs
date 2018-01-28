using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HighLowGame : MonoBehaviour {
    public enum ButtonState { NONE = 0, LEFT = 1, MID = 2, RIGHT = 3 };
    public LEDPanel LED1, LED2;

    public Text Line1, Line2;

    public Button ButtonLeft, ButtonMid, ButtonRight;
    public int Round;

    public UnityEvent OnRoundStart;
    public UnityEvent OnRoundEnd;
    public UnityEvent OnStageStart;
    public UnityEvent OnStageEnd;

    public ProgressionPanel ProgressPanel;
    public GameStageContainer Levels;

    private GamePlayer PlayerData = new GamePlayer();

    private void Start()
    {
        PlayerData.Pull();
        PlayerData.Wipe();// for debugging until the ui is done
        if (PlayerData.Level == 0) PlayerData.Level = 1;
        if (PlayerData.Stage == 0) PlayerData.Stage = 1;
        PlayerData.Push();
        GameProgression.InitGameProgression(this.Levels);
        ProgressPanel.Init(this.Levels);
    }

    private void OnEnable()
    {
        StartCoroutine(GameLoop());
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

    IEnumerator GameLoop()
    {
        yield return StartCoroutine(Instructions());
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

    private IEnumerator StageComplete(int earedPoints)
    {
        LcdWrite("Stage", "Complete!");
        yield return new WaitForSeconds(2);
        LcdWrite("You earned", string.Format("{0} Points", earedPoints));
        yield return new WaitForSeconds(2);
        LcdWrite("You have", string.Format("{0} Points", PlayerData.Points));
        yield return new WaitForSeconds(2);
        LcdWrite("You are", string.Format("Level {0}", PlayerData.Level));
        yield return new WaitForSeconds(2);
    }

    public enum FinishState { Right, Wrong, Timeout, Quit, None};

    private IEnumerator Game()
    {
        var playingRound = true;
        OnStageStart.Invoke();
        while (playingRound)
        {
            var roundData = GameProgression.GetRound(PlayerData.Level, PlayerData.Stage, Round, LED1.LEDArraySize);
            OnRoundStart.Invoke();
            if (Round == 1)
            {
                if (roundData.IntroText != null && roundData.IntroText.Any())
                {
                    int lastLcv = -1;
                    for (int lcv = 0, length = roundData.IntroText.Count; lcv < length; lcv++)
                    {
                        string s1, s2;
                        if (lastLcv < 0)
                        {
                            s1 = "";
                        }
                        else
                        {
                            s1 = roundData.IntroText[lastLcv];
                        }
                        s2 = roundData.IntroText[lcv];
                        LcdWrite(s1, s2);
                        lastLcv++;
                        yield return new WaitForSeconds(2f);
                    }
                    yield return new WaitForSeconds(2f);
                }
            }

            var highMessage = roundData.High ? "     HIGHER!   " : "     LOWER!   ";
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
            var currentSeconds = roundData.WaitSeconds;
            var loop = true;
            ClearButtonState();
            var _gameState = FinishState.None;
            int progressDashCount = 16;
            while (loop)
            {
                currentSeconds = Time.time - startTime;
                float progress = currentSeconds / roundData.WaitSeconds;
                progressDashCount = 16 - (Mathf.RoundToInt(16 * progress));
                var progressMessage = progressDashCount > 0 ? new string('-', progressDashCount) : "";
                LcdWrite(highMessage, progressMessage);
                if (progressDashCount <= 0)
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
            Debug.Log("round complete");
            switch (_gameState)
            {
                case FinishState.Right:
                    LcdWrite("Great job", "");
                    var earnedPoints = 100;
                    earnedPoints += (progressDashCount * 20);
                    Round++;
                    if (Round > GameProgression.ROUND_COUNT) // TODO: make variable for max rounds
                    {
                        //PlayerData.Stage++;  // for now, let's not auto progress the stage.
                        if (PlayerData.Stage > GameProgression.STAGE_COUNT)
                        {
                            PlayerData.Level++;
                            earnedPoints += 5000;
                            PlayerData.Stage = 1;
                        }
                        earnedPoints += 1000;
                        playingRound = false;
                        PlayerData.Points += earnedPoints;
                        PlayerData.Push();
                        yield return StartCoroutine(StageComplete(earnedPoints));
                    }
                    break;
                case FinishState.Wrong:
                    LcdWrite("you suck", "");
                    playingRound = false;
                    yield return StartCoroutine(StageFailed());
                    break;
                case FinishState.Timeout:
                    LcdWrite("Time's up", "");
                    playingRound = false;
                    yield return StartCoroutine(StageFailed());
                    break;
                case FinishState.Quit:
                    LcdWrite("Exiting", "");
                    playingRound = false;
                    yield return StartCoroutine(StageQuit());
                    break;
            }
            OnRoundEnd.Invoke();
        }
        OnStageEnd.Invoke();
        yield return null;
    }

    private void LcdWrite(string l1, string l2)
    {
        Line1.text = Line2.text = "";
        Line1.text = l1;
        Line2.text = l2;
    }

}
