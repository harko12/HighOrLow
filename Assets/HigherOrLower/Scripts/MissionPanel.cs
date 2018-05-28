using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionPanel : MonoBehaviour {
    private Animator mAnim;

    public Text[] missionDescriptions;

    private void Awake()
    {
        mAnim = GetComponent<Animator>();
    }

    public void ToggleMissions()
    {
        mAnim.SetTrigger("MissionToggle");
    }

    public void UpdateMissions(Mission[] missions)
    {
        for (var lcv = 0; lcv < missions.Length; lcv++)
        {
            missionDescriptions[lcv].text = missions[lcv].ToString();
        }
    }

}
