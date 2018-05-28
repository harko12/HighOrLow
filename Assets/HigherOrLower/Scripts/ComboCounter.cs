using HarkoGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboCounter : MonoBehaviour {
    public Text ComboNumberText;
    public Text ComboLabelText;
    private CanvasGroup mComboGroup;

    private int mCurrentCombo;

    private void Awake()
    {
        mComboGroup = GetComponent<CanvasGroup>();
    }

    // Use this for initialization
    void Start () {
        mComboGroup.alpha = 0f;
	}

    public void OnRoundEnd(string eventId, RoundResultInfo roundInfo)
    {
        SetComboText(roundInfo.MyMission.OverallResult.Combo);
    }

    public bool testShow;
    public bool testHide;
    public float fadeTime = .5f;
    public int testcombovalue;
    public bool testCombo;

    public void Update()
    {

        if (testShow)
        {
            testShow = false;
            testHide = false;
            StartCoroutine(Fade(true, fadeTime));
        }

        if (testHide)
        {
            testShow = false;
            testHide = false;
            StartCoroutine(Fade(false, fadeTime));
        }

        if (testCombo)
        {
            testCombo = false;
            SetComboText(testcombovalue);
        }

    }

    public void Toggle(bool show)
    {
        if (show && mComboGroup.alpha <= .1f)
        {
            StartCoroutine(Fade(true, fadeTime));
        }

        if (!show && mComboGroup.alpha > .1f)
        {
            StartCoroutine(Fade(false, fadeTime));
        }
    }

    public IEnumerator Fade(bool show, float length)
    {
        float start = show ? 0f : 1f;
        float end = show ? 1f : 0f;
        float t = 0;
        while (t <= length)
        {
            mComboGroup.alpha = Mathf.Lerp(start, end, t / length);
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
        }
        yield return null;
    }

    private void SetComboText(int newCombo)
    {
        if (newCombo == 0) // && mCurrentCombo != 0)
        {
            // sad, trigger a sad effect.  Maybe a red text, or rundown of the numbers
            ComboNumberText.text = "";
            ComboLabelText.text = "";
            Toggle(false);
        }
        else
        {
            ComboLabelText.text = "combo";
            ComboNumberText.text = string.Format("{0}", newCombo);
            Toggle(true);
        }
        mCurrentCombo = newCombo;
    }
}
