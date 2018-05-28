using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HarkoGames;

public class BasicTimer : MonoBehaviour {
    private Text mTimerText;
    private Animator textAnim;
    public Text AdjustmentText;

    private void Awake()
    {
        textAnim = AdjustmentText.GetComponent<Animator>();
    }
    // Use this for initialization
    void Start () {
        mTimerText = GetComponent<Text>();
	}
	
    public void OnTimerUpdate(string eventId, TimeUpdateInfo args)
    {
        mTimerText.text = TimeUtility.FormattedTime_MSSMM(args.RemainingTime);
    }

    public void OnRoundEnd(string eventId, RoundResultInfo roundInfo)
    {
        StartCoroutine(HandleTimeAdjustment(eventId, roundInfo));
    }

    public void onStageStart(string eventId)
    {
        mTimerText.text = TimeUtility.FormattedTime_MSSMM(0);
    }

    private IEnumerator HandleTimeAdjustment(string eventId, RoundResultInfo roundInfo)
    {
        EventMonitor.StartEvent(eventId);
        if (roundInfo.TimeAdjustment != 0)
        {
            AdjustmentText.text = string.Format("{0}{1}", roundInfo.TimeAdjustment > 0 ? "+" : "-", TimeUtility.FormattedTime_MSSMM(roundInfo.TimeAdjustment));
            textAnim.SetTrigger(roundInfo.TimeAdjustment > 0 ? "AddTime" : "RemoveTime");
            yield return new WaitForSeconds(.25f);
        }
        EventMonitor.EndEvent(eventId);
        yield return null;
    }
}
