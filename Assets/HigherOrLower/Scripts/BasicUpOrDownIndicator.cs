using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HighOrLow
{
    public class BasicUpOrDownIndicator : MonoBehaviour
    {
        public Image myImage;
        public Sprite Idle;
        public Sprite Pointy;
        private Vector2 baseScale;
        private bool? lastHigher;

        private void OnEnable()
        {
            var em = GameReferences.instance.gameEvents;
            em.OnRoundStart.AddListener(OnRoundStart);
            em.OnStageStart.AddListener(onStageSetup);
            em.OnStageEnd.AddListener(OnStageEnd);
        }

        private void OnDisable()
        {
            var em = GameReferences.instance.gameEvents;
            em.OnRoundStart.RemoveListener(OnRoundStart);
            em.OnStageStart.RemoveListener(onStageSetup);
            em.OnStageEnd.RemoveListener(OnStageEnd);
        }
        // Use this for initialization
        void Start()
        {
            lastHigher = null;
            myImage.sprite = Idle;
            baseScale = myImage.rectTransform.sizeDelta;
        }

        public float blinkDuration = .75f;
        public bool blink;
        private float blinkStart = 0f;
        private void Update()
        {
            if (blink)
            {
                blinkStart = Time.time;
                blink = false;
            }

            if (blinkStart != 0f)
            {
                var t = (Time.time - blinkStart) / blinkDuration;
                var yVal = Mathf.Lerp(0, baseScale.y, t);
                myImage.rectTransform.sizeDelta = new Vector2(baseScale.x, yVal);
                if (t > .95f)
                {
                    blinkStart = 0f; // blink done
                }
            }
            
        }

        public void OnRoundStart(string eventId, GameProgression.GameRound round)
        {
            float targetRotation = 0f;
            if (lastHigher != null && lastHigher == round.High)
            {
                return; // skip if it's not different
            }
            if (!round.High)
            {
                targetRotation = 180f;
            }
            lastHigher = round.High;
            blink = true;
            myImage.sprite = Pointy;
            myImage.transform.SetPositionAndRotation(myImage.transform.position, Quaternion.Euler(0f, 0f, targetRotation));
        }
        /*
                public void OnRoundEnd(string eventId, RoundResultInfo roundInfo)
                {
                    if (lastHigher != null && lastHigher == round.High)
                    {
                        return; // skip if it's not different
                    }

                }
                */
        public void onStageSetup(string eventId)
        {
            lastHigher = null;
            blink = true;
            myImage.sprite = Idle;
            myImage.transform.SetPositionAndRotation(myImage.transform.position, Quaternion.Euler(0f, 0f, 0f));
        }

        public void OnStageEnd(string eventId, Mission m)
        {
            onStageSetup(eventId);
        }

    }
}
