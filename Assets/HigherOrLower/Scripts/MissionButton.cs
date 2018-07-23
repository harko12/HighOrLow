using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HighOrLow
{

    public class MissionButton : MonoBehaviour
    {
        public Image MissionTypeImage;
        private Button myButton;
        public Text MissionText;
        public WalletPanel PrizeWallet, CostWallet;
        public Sprite SurvivalSprite, SprintSprite, ByRoundSprite, TimeAttackSprite;
        public int MissionIndex;
        public bool CanPlay { get; set; }

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
            if (CanPlay)
            {
                HighLowGame.GetInstance().StartMission(MissionIndex);
            }
            else
            {
                // put up message
                Debug.Log("can't afford this mission");
            }
        }

        public void SetMission(Mission m, int missionIndex)
        {
            MissionIndex = missionIndex;
            MissionText.text = m.Description();
            PrizeWallet.UpdateWalletAndDisplay(m.PrizePurse);
            CostWallet.UpdateWalletAndDisplay(m.Cost);
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

}
