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
    private Vector3 initialRotation, targetRotation;
    private Vector3 hideVert = new Vector3(0f, 90f, 0f);
    private Vector3 hideHoriz = new Vector3(90f, 0f, 0f);
    private float rotDuration = .5f;
    private float rotTime;

    private void Awake()
    {
        initialRotation = transform.rotation.eulerAngles;
    }

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
    {
        var gameManager = HighLowGame.GetInstance();
        StartCoroutine(gameManager.StageClicked(StageInfo));
    }

    private void Update()
    {
        if (testHide)
        {
            testHide = false;
            HideStage(testLevel, testStage);
        }

        if (targetRotation != Vector3.zero)
        {
            var t = (Time.time - rotTime) / rotDuration;
            var rot = Quaternion.Euler(Vector3.Lerp(initialRotation, targetRotation, t));
            if (t >= .95)
            {
                rot = Quaternion.Euler(targetRotation);
                targetRotation = Vector3.zero;
            }
            transform.rotation = rot;
        }
    }

    public void ShowStage()
    {
        transform.rotation = Quaternion.Euler(initialRotation);
    }

    public int testStage, testLevel;
    public bool testHide = false;
    public void HideStage(int clickedLevel, int clickedStage)
    {
        if (clickedLevel > StageInfo.Level)
        {
            targetRotation = hideHoriz;
        }
        else if (clickedLevel < StageInfo.Level)
        {
            targetRotation = -1 * hideHoriz;
        }
        else
        {
            if (clickedStage < StageInfo.Stage)
            {
                targetRotation = -1 * hideVert;
            }
            else if (clickedStage > StageInfo.Stage)
            {
                targetRotation = hideVert;
            }
        }
        rotTime = Time.time;
    }
}
