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
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ToggleMissions()
    {
        mAnim.SetTrigger("MissionToggle");
    }

    public void UpdateMissions(Mission[] missions)
    {
        for (var lcv = 0; lcv < 3; lcv++)
        {
            missionDescriptions[lcv].text = missions[lcv].ToString();
        }
    }

}
