using HighOrLow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalletPanelTester : MonoBehaviour {
    public WalletValue coinValue;
    public WalletValue tokenValue;
    public WalletValue timeValue;

    public bool instantUpdates;

    public WalletPanel[] targetWalletPanels;

    public Wallet testWallet;

    public void updateTokens()
    {
        tokenValue.SetValue(testWallet.Tokens, instantUpdates);
        var w = new Wallet();
        w.Tokens = testWallet.Tokens;
        updateWallets();
    }

    public void updateCoins()
    {
        coinValue.SetValue(testWallet.Coins, instantUpdates);
        var w = new Wallet();
        w.Coins = testWallet.Coins;
        updateWallets();
    }

    public void updateTime()
    {
        timeValue.SetValue(testWallet.Time, instantUpdates);
        var w = new Wallet();
        w.Time = testWallet.Time;
        updateWallets();
    }

    public void updateWallets()
    {
        foreach (WalletPanel wp in targetWalletPanels)
        {
            if (instantUpdates)
            {
                wp.UpdateWalletAndDisplay(testWallet);
            }
            else
            {
                StartCoroutine(wp.UpdateWalletAndDisplayAsync("", testWallet));
            }
        }
    }
}
