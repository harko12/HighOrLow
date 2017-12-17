using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDebugPanel : MonoBehaviour {
    public InputField lvlInput, stageInput, roundInput;
    public Button updateBtn, clearBtn;
    public HighLowGame gameMaster;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void EnableControls(bool enable)
    {
        lvlInput.enabled = enable;
        stageInput.enabled = enable;
        roundInput.enabled = enable;

        updateBtn.enabled = enable;
        clearBtn.enabled = enable;
    }
}
