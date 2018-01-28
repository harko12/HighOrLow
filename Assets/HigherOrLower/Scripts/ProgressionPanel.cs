using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ProgressionPanel : MonoBehaviour {
    public Transform ProgressView;
    public GameObject LevelPrefab, StagePrefab;
    private Animator mAnim;

    private void Awake()
    {
        mAnim = GetComponent<Animator>();
    }

    public void Init(GameStageContainer levelContainer)
    {
        for (int lcv = 0, length = levelContainer.Levels.Length; lcv < length; lcv++)
        {
            var lvl = levelContainer.Levels[lcv];
            var l = GameObject.Instantiate(LevelPrefab, ProgressView.transform).GetComponent<Level>();
            l.Init(lvl.Level, lvl.Description);
            l.name = string.Format("Level {00}", lvl.Level);
            for (int lcv2 = 0, l2 = lvl.Stages.Length; lcv2 < l2; lcv2++)
            {
                var stage = lvl.Stages[lcv2];
                var s = GameObject.Instantiate(StagePrefab, l.StageAnchor.transform).GetComponent<Stage>();
                s.Init(stage);
                s.name = string.Format("Stage {00}", stage.Stage);
            }
        }
    }

    public void ToggleMenu()
    {
        mAnim.SetTrigger("MenuToggle");
    }
}
