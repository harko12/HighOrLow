using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WalletPanel : MonoBehaviour {
    public Image pointsImg, tokensImg, coinsImg;
    public Text pointsTxt, tokensTxt, coinsTxt;

    public void UpdateInfo(Wallet w)
    {
        pointsTxt.text = w.Points.ToString();
        coinsTxt.text = w.Coins.ToString();
        tokensTxt.text = w.Tokens.ToString();
    }
}
