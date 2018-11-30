using HarkoGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HighOrLow
{

    [RequireComponent(typeof(Animator))]
    public class ProgressionPanel : MonoBehaviour
    {
        public Transform ProgressView;
        public GameObject LevelPrefab, StagePrefab;
        private Animator mAnim;
        public Text PlayerInfo;
        public WalletPanel WalletInfo;
        public MissionPanel MissionView;

        public System.Action<int, int> OnHideStages;
        public System.Action OnShowStages;


        private void Awake()
        {
            mAnim = GetComponent<Animator>();
        }

        public void Init(GameStageContainer levelContainer, GamePlayer p)
        {
            // clear handlers
            OnHideStages = null;
            OnShowStages = null;

            for (int lcv = 0, length = levelContainer.Levels.Length; lcv < length; lcv++)
            {
                var lvl = levelContainer.Levels[lcv];
                var l = GameObject.Instantiate(LevelPrefab, ProgressView.transform).GetComponent<Level>();
                l.Init(lvl.Level, lvl.Description);
                l.name = string.Format("Level {00}", lvl.Level);
                l.LevelInfo = lvl;
                for (int lcv2 = 0, l2 = lvl.Stages.Length; lcv2 < l2; lcv2++)
                {
                    var stage = lvl.Stages[lcv2];
                    var s = GameObject.Instantiate(StagePrefab, l.StageAnchor.transform).GetComponent<Stage>();
                    s.Init(stage, p);
                    s.name = string.Format("Stage {00}", stage.Stage);
                    s.StageInfo = stage;
                    OnHideStages += s.HideStage;
                    OnShowStages += s.ShowStage;
                }
            }
            UpdateStages(p);
            WalletInfo.UpdateWalletAndDisplay(p.myWallet);
        }

        public void UpdateStages(GamePlayer p)
        {
            PlayerInfo.text = string.Format("{0} Level: {1}", p.Name, p.Level);
            //        WalletInfo.UpdateInfo(p.myWallet);
            for (int lcv = 0, length = ProgressView.transform.childCount; lcv < length; lcv++)
            {
                var level = ProgressView.transform.GetChild(lcv).gameObject.GetComponent<Level>();

                for (int lcv2 = 0, length2 = level.StageAnchor.transform.childCount; lcv2 < length2; lcv2++)
                {
                    var stage = level.StageAnchor.transform.GetChild(lcv2).gameObject.GetComponent<Stage>();
                    stage.UpdateStage(stage.StageInfo, p);
                }
            }
        }

        public void onStageClicked(string eventId, GameStage stage)
        {
            StartCoroutine(RunStageClicked(eventId, stage));
        }

        public IEnumerator RunStageClicked(string eventId, GameStage stage)
        {
            EventMonitor.StartEvent(eventId);
            OnHideStages.Invoke(stage.Level, stage.Stage);
            yield return new WaitForSeconds(1f);
            EventMonitor.EndEvent(eventId);
            yield return null;
        }

        public void ToggleMenu()
        {
            mAnim.SetTrigger("MenuToggle");
        }
    }
}