using HighOrLow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Stage : MonoBehaviour {
    public Image H, V, D, FlipX, FlipY, OffsetX, OffsetY;
    public Text StageNumber;
    public StageLock mLock;
    public GameStage StageInfo { get; set; }

    public void Init(GameStage stage, GamePlayer player)
    {
        StageNumber.text = stage.Stage.ToString();
        for (int lcv = 0, length = stage.LEDStyleWeights.Length; lcv < length; lcv++)
        {
            var styleWeight = stage.LEDStyleWeights[lcv];
            var weight = styleWeight.Weight == 0 ? 1 : styleWeight.Weight; // we treat weight zero as 1, for now
            switch (styleWeight.GraphType)
            {
                case LEDRendering.GraphType.Horizontal:
                    H.gameObject.SetActive(true);
                    H.SetTransparency(weight);
                    break;
                case LEDRendering.GraphType.Vertical:
                    V.gameObject.SetActive(true);
                    V.SetTransparency(weight);
                    break;
                case LEDRendering.GraphType.DiagonalX:
                    D.gameObject.SetActive(true);
                    D.SetTransparency(weight);
                    break;
            }
        }
        if (stage.InvertXWeight > 0)
        {
            FlipX.gameObject.SetActive(true);
            FlipX.SetTransparency(stage.InvertXWeight);
        }
        if (stage.InvertYWeight > 0)
        {
            FlipY.gameObject.SetActive(true);
            FlipY.SetTransparency(stage.InvertYWeight);
        }
        if (stage.StartXWeight > 0)
        {
            OffsetX.gameObject.SetActive(true);
            OffsetX.SetTransparency(stage.StartXWeight);
        }
        if (stage.StartYWeight > 0)
        {
            OffsetY.gameObject.SetActive(true);
            OffsetY.SetTransparency(stage.StartYWeight);
        }
        UpdateStage(stage, player);
    }

    public void UpdateStage(GameStage stage, GamePlayer player)
    {
        if (player.Level < stage.Level
             || (stage.PreReqPoints > 0 && player.myWallet.Time < stage.PreReqPoints))
        {
//            mLock.Lock();
            mLock.Unlock();
        }
        else
        {
            mLock.Unlock();
        }
    }

    public void OnStageClick()
    { // TODO: maybe break this dependency on gamemanager and make this an event
        var gameManager = HighLowGame.GetInstance();
        var p = gameManager.Getplayer();
        var missions = MissionGenerator.GetInstance().GenerateMissions(p, StageInfo, 4);
        gameManager.SetCurrentMissions(missions);
        gameManager.ProgressPanel.MissionView.ClearMissions();
        gameManager.ProgressPanel.MissionView.UpdateMissions(p, missions);
        gameManager.ProgressPanel.MissionView.ToggleMissions();
    }
}
