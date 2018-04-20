using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicTimer : MonoBehaviour {
    private const string Format = "{0:0}:{1:00}.{2:00}";
    private Text mTimerText;
    private HighLowGame mGame;
	// Use this for initialization
	void Start () {
        mTimerText = this.GetComponent<Text>();
        mGame = HighLowGame.GetInstance();
	}
	
    public void OnTimerUpdate()
    {

        System.TimeSpan t = System.TimeSpan.FromSeconds(mGame.currentTimeValues.RemainingTime);

        int mins = t.Minutes;
        int secs = t.Seconds;
        int milliSecs = t.Milliseconds;

        mTimerText.text = string.Format(Format, mins, secs, milliSecs);
    }
}
