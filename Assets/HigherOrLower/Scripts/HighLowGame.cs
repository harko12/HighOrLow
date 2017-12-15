using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighLowGame : MonoBehaviour {
    public enum ButtonState { NONE = 0, LEFT = 1, MID = 2, RIGHT = 3 };
    public LEDPanel LED1, LED2;

    public Text Line1, Line2;

    public Button ButtonLeft, ButtonMid, ButtonRight;

    public int Level, Stage, Round, Points;

    private void Start()
    {
        //GameLoop();
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

    IEnumerator GameLoop()
    {
        //yield return StartCoroutine(Instructions());
        bool exit = false;
        ClearButtonState();
        while (!exit)
        {
            LcdWrite(string.Format("Stage: {0}", Stage), "Hit Enter to go!");
            switch (_buttonState)
            {
                case ButtonState.MID:
                    Round = 1;
                    yield return StartCoroutine(Game());
                    break;
            }
            yield return new WaitForSeconds(.5f);
        }
        yield return null;
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
        LcdWrite(string.Format("Stage {0}", Stage), "Failed");
        yield return new WaitForSeconds(2);
        LcdWrite("Try", "Again!");
        yield return new WaitForSeconds(2);
    }

    private IEnumerator StageQuit()
    {
        LcdWrite(string.Format("Stage {0}", Stage), "Exiting");
        yield return new WaitForSeconds(2);
        LcdWrite("Returning to", "Menu");
        yield return new WaitForSeconds(2);
    }

    private IEnumerator StageComplete(int earedPoints)
    {
        LcdWrite("Stage", "Complete!");
        yield return new WaitForSeconds(2);
        LcdWrite("You earned", string.Format("{0} Points", Points));
        yield return new WaitForSeconds(2);
        LcdWrite("You have", string.Format("{0} Points", Points));
        yield return new WaitForSeconds(2);
        LcdWrite("You are", string.Format("Level {0}", Level));
        yield return new WaitForSeconds(2);
    }

    public enum FinishState { Right, Wrong, Timeout, Quit, None};

    private IEnumerator Game()
    {
        var playingRound = true;
        while (playingRound)
        {

            var high = Random.value > .5f;
            var multiplier = high ? -1 : 1;

            int baseValue = 0;
            int wrongValue = 0;
            do
            {
                baseValue = Random.Range(1, 32);
                var adjustment = Random.Range(5, 25) * multiplier;
                wrongValue = baseValue + adjustment;
            } while ((baseValue == 0 && wrongValue == 0));


            var Led1Right = Random.value > .5f;
            var highMessage = high ? "     HIGH!     " : "     LOW!     ";

            int startX = Random.Range(0, LED1.LEDArraySize);
            int startY = Random.Range(0, LED1.LEDArraySize);

            ButtonState incorrectBtn, correctBtn;
            if (Led1Right)
            {
                LEDRendering.Graph(LEDRendering.GraphType.Horizontal,LED1, startX, startY, baseValue);
                LEDRendering.Graph(LEDRendering.GraphType.Horizontal, LED2, startX, startY, wrongValue, true);
                correctBtn = ButtonState.LEFT;
                incorrectBtn = ButtonState.RIGHT;
            }
            else
            {
                LEDRendering.Graph(LEDRendering.GraphType.Horizontal, LED2, startX, startY, baseValue);
                LEDRendering.Graph(LEDRendering.GraphType.Horizontal, LED1, startX, startY, wrongValue, true);
                correctBtn = ButtonState.RIGHT;
                incorrectBtn = ButtonState.LEFT;
            }
            var waitSeconds = 5f;
            float startTime = Time.time;
            var currentSeconds = waitSeconds;
            var loop = true;
            ClearButtonState();
            var _gameState = FinishState.None;
            int progressDashCount = 16;
            while (loop)
            {
                currentSeconds = Time.time - startTime;
                float progress = currentSeconds / waitSeconds;
                progressDashCount = 16 - (Mathf.RoundToInt(16 * progress));
                var progressMessage = progressDashCount > 0 ? new string('-', progressDashCount) : "";
                LcdWrite(highMessage, progressMessage);
                if (progressDashCount <= 0)
                {
                    loop = false;
                    _gameState = FinishState.Timeout;
                    continue;
                }
                if (_buttonState == correctBtn)
                {
                    //right
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
                else if (_buttonState == ButtonState.MID)
                {
                    Debug.Log("exit");
                    loop = false;
                    _gameState = FinishState.Quit;
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
                    if (Round > 10) // TODO: make variable for max rounds
                    {
                        Stage++;
                        earnedPoints += 1000;
                        playingRound = false;
                        Points += earnedPoints;
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
        }
        yield return null;
    }

    private void LcdWrite(string l1, string l2)
    {
        Line1.text = Line2.text = "";
        Line1.text = l1;
        Line2.text = l2;
    }

}
