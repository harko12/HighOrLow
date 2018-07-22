using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HighOrLow;

public class PlayerDebugPanel : MonoBehaviour {
    public InputField lvlInput, stageInput, roundInput;
    public Button updateBtn, clearBtn, rulesInput;
    public HighLowGame gameMaster;
    public LEDPanel led;

    public ProgressionPanel pPanel;

    private GamePlayer mPlayer;

	// Use this for initialization
	void Start () {
        mPlayer = gameMaster.Getplayer();
        UpdateInfo("");
	}
	
    public void InitPanelClicked()
    {
        pPanel.Init(gameMaster.Levels, mPlayer);
    }

    public void UpdatePanelClicked()
    {
        pPanel.UpdateStages(mPlayer);
    }

    public void UpdateInfo(string eventId)
    {
        lvlInput.text = string.Format("{0}", mPlayer.Level);
        stageInput.text = string.Format("{0}", mPlayer.Stage);
        roundInput.text = string.Format("{0}", gameMaster.Round);
    }
    private int lvl = 0, stage = 0, round = 0;

    private void parseValues()
    {
        int val = 0;
        if (int.TryParse(lvlInput.text, out val))
        {
            lvl = val;
        }

        val = 0;
        if (int.TryParse(stageInput.text, out val))
        {
            stage = val;
        }

        val = 0;
        if (int.TryParse(roundInput.text, out val))
        {
            round = val;
        }
    }

    public void UpdateClicked()
    {
        parseValues();
        mPlayer.SetLevelAndStage(lvl, stage);
    }

    public void RulesClicked()
    {
        parseValues();
        var gr = GameProgression.GetRound(lvl, stage, round, led.LEDArraySize);
    }

    public void EnableControls(bool enable)
    {
        lvlInput.enabled = enable;
        stageInput.enabled = enable;
        roundInput.enabled = enable;

        updateBtn.enabled = enable;
        clearBtn.enabled = enable;
        rulesInput.enabled = enable;
    }
}
