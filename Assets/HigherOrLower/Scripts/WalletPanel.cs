using HarkoGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HighOrLow
{

    public class WalletPanel : MonoBehaviour {
        public WalletValue coinValue, tokenValue, timeValue;
        private Wallet myWallet;

        public string WalletValueDescription()
        {
            return myWallet.ToString();
        }

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

        public void onStageStart(string eventId)
        {
            myWallet.Wipe();
            UpdateWalletAndDisplay(myWallet);
        }

        /// <summary>
        /// Update the wallet of the panel and the display
        /// </summary>
        /// <param name="w"></param>
        /// <remarks>will be instant</remarks>
        public void UpdateWalletAndDisplay(Wallet w)
        {
            myWallet.Load(w.ToJson());
            coinValue.SetValue(w.Coins);
            tokenValue.SetValue(w.Tokens);
            timeValue.SetValue(w.Time);
        }

        public void OnRoundEnd(string eventId, RoundResultInfo roundInfo)
        {
            StartCoroutine(UpdateWalletAndDisplayAsync(eventId, roundInfo.MyMission.OverallResult.EarnedLoot));
        }

        /// <summary>
        /// visualization for adjusting a wallet slow enough for the player to see
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="newWallet">new wallet value to adjust to. </param>
        /// <remarks>This is NOT intended to do the math on the player's wallet, only on a display wallet</remarks>
        /// <returns></returns>
        public IEnumerator UpdateWalletAndDisplayAsync(string eventId, Wallet newWallet)
        {
            //EventMonitor.StartEvent(eventId);
            myWallet.Load(newWallet.ToJson());

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