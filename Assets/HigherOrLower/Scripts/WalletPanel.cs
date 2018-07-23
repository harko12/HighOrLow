using HarkoGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HighOrLow
{

    public class WalletPanel : MonoBehaviour {
        public WalletValue coinValue, tokenValue, timeValue;
        public Wallet myWallet;

        public void Awake()
        {
            myWallet = new Wallet();
        }

        /// <summary>
        /// A way to compare the wallet of a panel to an outside wallet to see if the outside wallet can afford the one in the panel
        /// </summary>
        /// <param name="w"></param>
        /// <remarks>Kind of backwards, but I didn't want to expose the panel's wallet publicly</remarks>
        /// <returns></returns>
        public bool IsAffordableBy(Wallet w)
        {
            return w.CanAfford(myWallet);
        }

        public void UpdateInfo(Wallet w, bool instant = true)
        {
            myWallet.Load(w.ToJson());
            coinValue.SetValue(w.Coins, instant);
            tokenValue.SetValue(w.Tokens, instant);
            timeValue.SetValue(w.Time, instant);
        }

        public void OnRoundEnd(string eventId, RoundResultInfo roundInfo)
        {
            StartCoroutine(HandleWalletAdjustment(eventId, roundInfo.MyMission.OverallResult.EarnedLoot));
        }

        public void onStageStart(string eventId)
        {
            myWallet.Wipe();
            UpdateInfo(myWallet);
        }

        /// <summary>
        /// visualization for adjusting a wallet slow enough for the player to see
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="newWallet">new wallet value to adjust to. </param>
        /// <remarks>This is NOT intended to do the math on the player's wallet, only on a display wallet</remarks>
        /// <returns></returns>
        public IEnumerator HandleWalletAdjustment(string eventId, Wallet newWallet)
        {
            //EventMonitor.StartEvent(eventId);

            timeValue.SetValue(newWallet.Time, false);
            while (timeValue.Updating)
            {
                yield return null;
            }

            coinValue.SetValue(newWallet.Coins, false);
            while (coinValue.Updating)
            {
                yield return null;
            }

            tokenValue.SetValue(newWallet.Tokens, false);
            while (tokenValue.Updating)
            {
                yield return null;
            }

            //EventMonitor.EndEvent(eventId);

            yield return null;
        }
    }
}