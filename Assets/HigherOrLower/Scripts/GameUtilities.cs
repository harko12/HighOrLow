using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HighOrLow
{
    public class GameUtilities
    {

    }

    public static class GameExtensions
    {
        public static void SetTransparency(this Image img, float trans)
        {
            if (img != null)
            {
                UnityEngine.Color __alpha = img.color;
                __alpha.a = trans;
                img.color = __alpha;
            }
        }
    }
}
