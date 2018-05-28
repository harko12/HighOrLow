using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RoundResultHandler : MonoBehaviour
{
    public Text label;
    private const string msgFormat = @"Round: {0}
Combo: {1}
Points: {2}";
    public void OnRoundEnd(RoundResultInfo args)
    {
        label.text = string.Format(msgFormat, args.Round, args.MyMission.OverallResult.MaxStreak, 0);
    }
}
