using HarkoGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour {
    public Image ProgressImage;
    public int CountdownIterations = 3;
    public Gradient ColorProgression;
    private CanvasGroup myGroup;
    private float progress;

    public void Awake()
    {
        myGroup = GetComponent<CanvasGroup>();
        myGroup.alpha = 0f;
        myGroup.blocksRaycasts = false;
    }

    public void onCountDown(string eventId)
    {
        StartCoroutine(RunCountdown(eventId, CountdownIterations));
    }

    public IEnumerator RunCountdown(string eventId, int countdownStart)
    {
        EventMonitor.StartEvent(eventId);
        myGroup.alpha = 1f;
        myGroup.blocksRaycasts = true;
        float step = .1f;
        progress = 0f;
        for (float lcv = countdownStart; lcv >= 0; lcv -= step)
        {
            progress += step;
            var normalProgress = progress / countdownStart;
            ProgressImage.fillAmount = normalProgress;
            ProgressImage.color = ColorProgression.Evaluate(normalProgress);
            yield return new WaitForSeconds(step);
        }
        myGroup.alpha = 0f;
        myGroup.blocksRaycasts = false;
        EventMonitor.EndEvent(eventId);
    }


}
