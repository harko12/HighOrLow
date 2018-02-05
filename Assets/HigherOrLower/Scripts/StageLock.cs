using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageLock : MonoBehaviour {
    public Image BackGround, LockedImg, UnlockedImg;

    public void Lock()
    {
        BackGround.gameObject.SetActive(true);
        BackGround.raycastTarget = true;
        LockedImg.gameObject.SetActive(true);
        UnlockedImg.gameObject.SetActive(false);
    }

    public void Unlock()
    {
        // todo: make some fancy effects here
        BackGround.gameObject.SetActive(false);
        BackGround.raycastTarget = false;
        LockedImg.gameObject.SetActive(false);
        UnlockedImg.gameObject.SetActive(false);
    }
}
