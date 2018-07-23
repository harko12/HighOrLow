using HarkoGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HighOrLow
{
    public class WalletValue : MonoBehaviour
    {
        public Text ValueText;
        public Image ValueImage;
        private float myValue;
        private Color NeutralColor, LossColor, GainColor;

        private Color TargetColor;
        private bool updateColor;

        public float colorFadeTime = .25f;
        private float colorTime = 0f;
        public WalletCurrency myCurrency;

        public bool Updating = false;

        private void Start()
        {
            Init();
        }

        private void Update()
        {
            if (updateColor)
            {
                var t = colorTime / colorFadeTime;
                var newColor = Color.Lerp(ValueText.color, TargetColor, t);
                colorTime += Time.deltaTime;
                if (colorTime > colorFadeTime)
                {
                    newColor = TargetColor;
                    updateColor = false;
                    colorTime = 0f;
                }
                ValueText.color = newColor;
            }
        }

        private void Init()
        {
            ValueImage.sprite = myCurrency.myImage;
            NeutralColor = myCurrency.NeutralColor;
            LossColor = myCurrency.LossColor;
            GainColor = myCurrency.GainColor;
            ValueText.color = NeutralColor;
            //SetValue(0f);
        }

        public void SetTextColorNeutral()
        {
            SetTextColor(NeutralColor);
        }

        public void SetTextColorForTargetValue(float v)
        {
            TargetColor = NeutralColor;
            if (v > myValue)
            {
                TargetColor = GainColor;
            }
            else if (v < myValue)
            {
                TargetColor = LossColor;
            }
            SetTextColor(TargetColor);
        }

        public void SetTextColor(Color c)
        {
            TargetColor = c;
            updateColor = true;
        }

        public void SetValue(float value, bool instant = true)
        {
            if (instant)
            {
                myValue = value;
                UpdateText(myValue);
            }
            else
            {
                StartCoroutine(RunAdjustment(value));
            }
        }

        private void UpdateText(float value)
        {
            switch (myCurrency.myType)
            {
                case WalletCurrencyType.Float:
                    ValueText.text = string.Format("{0:0.00}", value);
                    break;
                case WalletCurrencyType.Number:
                    ValueText.text = string.Format("{0}", (int)value);
                    break;
                case WalletCurrencyType.Time:
                    ValueText.text = TimeUtility.FormattedTime_MSSMM(value);
                    break;
            }
        }

        public IEnumerator RunAdjustment(float targetValue)
        {
            Updating = true;
            SetTextColorForTargetValue(targetValue);
            while(myValue != targetValue)
            {
                AdjustValueTowardsTarget(targetValue, myCurrency.displayTickIncrement);
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(.5f);
            SetTextColorNeutral();
            Updating = false;
        }

        public void AdjustValueTowardsTarget(float target, float adjustment)
        {
            var direction = 1;
            if (target < myValue)
            {
                direction = -1;
            }
            myValue += (adjustment * direction);

            // check overflow
            if (((target - myValue) * direction) < 0)
            {
                myValue = target;
            }

            UpdateText(myValue);
        }

    }
}
