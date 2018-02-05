using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour {
    public Text Title;
    public Transform StageAnchor;
    public GameLevel LevelInfo { get; set; }

    public void Init(int level, string title = null)
    {
        Title.text = string.Format("{00}  {1}", level, title);
    }
}
