using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionButton : MonoBehaviour
{
    public Image MissionTypeImage;
    private Button myButton;
    public Text MissionText;
    public WalletPanel PrizeWallet;
    public Sprite SurvivalSprite, SprintSprite, ByRoundSprite, TimeAttackSprite;
    public int MissionIndex;

    private void Awake()
    {
        myButton = GetComponent<Button>();
    }
    // put any init stuff here, since it's pooled
    private void OnEnable()
    {
        myButton.onClick.AddListener(StartMission);
        transform.localScale = Vector3.one;
    }

    private void OnDisable()
    {
        myButton.onClick.RemoveListener(StartMission);
    }

    void StartMission()
    {
        HighLowGame.GetInstance().StartMission(MissionIndex);
    }

    public void SetMission(Mission m, int missionIndex)
    {
        MissionIndex = missionIndex;
        MissionText.text = m.Description();
        PrizeWallet.UpdateInfo(m.PrizePurse, true);
        switch (m.MissionType)
        {
            case MissionTypes.ByRound:
                MissionTypeImage.sprite = ByRoundSprite;
                break;
            case MissionTypes.Sprint:
                MissionTypeImage.sprite = SprintSprite;
                break;
            case MissionTypes.Survival:
                MissionTypeImage.sprite = SurvivalSprite;
                break;
        }
    }
}
