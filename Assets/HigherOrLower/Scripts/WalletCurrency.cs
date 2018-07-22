using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace HighOrLow
{
    public enum WalletCurrencyType { Number, Float, Time };

    public class WalletCurrency : ScriptableObject
    {
        public WalletCurrencyType myType;
        public Sprite myImage;
        public string Name;
        public Color NeutralColor, LossColor, GainColor;
        public float displayTickIncrement;

        [MenuItem("Assets/More||Less/Create/WalletCurrency")]
        public static void CreateAsset()
        {
            var newCurrency = ScriptableObjectUtility.CreateAsset<WalletCurrency>("Assets/HigherOrLower/Wallet/Currencies/");
            newCurrency.Setup();
        }

        public void Setup()
        {

        }
    }
}
