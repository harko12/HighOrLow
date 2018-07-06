﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class MissionPanel : MonoBehaviour {
    private Animator mAnim;

    public GameObject missionButtonPrefab;
    public Transform contentRoot;
    public ObjectPooler missionButtonPool;

    private void Awake()
    {
        mAnim = GetComponent<Animator>();
    }

    public void ToggleMissions()
    {
        mAnim.SetTrigger("MissionToggle");
    }

    public void ClearMissions()
    {
        foreach (Transform t in contentRoot)
        {
            t.gameObject.SetActive(false);
        }
    }

    public void UpdateMissions(Mission[] missions)
    {
        for (var lcv = 0; lcv < missions.Length; lcv++)
        {
            var mb = missionButtonPool.GetPooledObject();
            var mbScript = mb.GetComponent<MissionButton>();
            mbScript.SetMission(missions[lcv], lcv);
            mb.SetActive(true);
        }
    }

}
