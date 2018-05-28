using HarkoGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WalletPanel : MonoBehaviour {
    public Image pointsImg, tokensImg, coinsImg;
    public Text timeTxt, tokensTxt, coinsTxt;
    private Wallet myWallet;
    public Color NeutralColor, LossColor, GainColor;

    public void UpdateInfo(Wallet w, bool instant = true)
    {
        if (myWallet == null)
        {
            myWallet = new Wallet();
        }

        myWallet = w;
        timeTxt.text = TimeUtility.FormattedTime_MSSMM(myWallet.Time);
        coinsTxt.text = myWallet.Coins.ToString();
        tokensTxt.text = myWallet.Tokens.ToString();
    }

    public void OnRoundEnd(string eventId, RoundResultInfo roundInfo)
    {
        StartCoroutine(HandleWalletAdjustment(eventId, roundInfo.MyMission.OverallResult.EarnedLoot));
    }

    public void onStageStart(string eventId)
    {
        myWallet = new Wallet();
    }

    private IEnumerator HandleWalletAdjustment(string eventId, Wallet newWallet)
    {
        // EventMonitor.StartEvent(eventId);
        SetColor(coinsTxt, newWallet.Coins.CompareTo(myWallet.Coins));
        SetColor(timeTxt, newWallet.Time.CompareTo(myWallet.Time));
        SetColor(tokensTxt, newWallet.Tokens.CompareTo(myWallet.Tokens));
        bool updating;
        do
        {
            updating = false;
            if (newWallet.Coins != myWallet.Coins)
            {
                updating = true;
                myWallet.Coins++;
            }

            if (newWallet.Tokens != myWallet.Tokens)
            {
                updating = true;
                myWallet.Tokens++;
            }

            if (newWallet.Time != myWallet.Time)
            {
                updating = true;
                myWallet.Time += .2f;
                if (myWallet.Time > newWallet.Time)
                {
                    myWallet.Time = newWallet.Time;
                }
            }
            timeTxt.text = TimeUtility.FormattedTime_MSSMM(myWallet.Time);
            coinsTxt.text = myWallet.Coins.ToString();
            tokensTxt.text = myWallet.Tokens.ToString();
            yield return new WaitForEndOfFrame();
        } while (updating);
        //EventMonitor.EndEvent(eventId);

        yield return null;
    }

    private void SetColor(Text t, int comparison)
    {
        var newColor = NeutralColor;
        if (comparison > 0)
        {
            newColor = GainColor;
        }
        else if (comparison < 0)
        {
            newColor = LossColor;
        }
        t.color = newColor;
    }

}
