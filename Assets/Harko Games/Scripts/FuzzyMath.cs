using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HarkoGames
{
    public static class FuzzyMath
    {
        public static float Grade(float value, float x0, float x1)
        {
            var result = 0f;
            var x = value;

            if (x <= x0)
            {
                result = 0f;
            }
            else if (x >= x1)
            {
                result = 1f;
            }
            else
            {
                result = (x / (x1 - x0)) - (x0 / (x1 - x0));
            }
            return result;
        }

        public static float ReverseGrade(float value, float x0, float x1)
        {
            var result = 0f;
            var x = value;

            if (x <= x0)
            {
                result = 1f;
            }
            else if (x >= x1)
            {
                result = 0f;
            }
            else
            {
                result = (-x / (x1 - x0)) + (x1 / (x1 - x0));
            }
            return result;
        }

        public static float Triangle(float value, float x0, float x1, float x2)
        {
            var result = 0f;
            var x = value;

            if (x <= x0)
            {
                result = 0f;
            }
            else if (x == x1)
            {
                result = 1f;
            }
            else if ((x > x0) && (x < x1))
            {
                result = (x / (x1 - x0)) - (x0 / (x1 - x0));
            }
            else
            {
                result = (-x / (x2 - x1)) + (x2 / (x2 - x1));
            }
            return result;
        }

        public static float Trapezoid(float value, float x0, float x1, float x2, float x3)
        {
            var result = 0f;
            var x = value;

            if (x <= x0)
            {
                result = 0f;
            }
            else if ((x >= x1) && (x <= x2))
            {
                result = 1f;
            }
            else if ((x > x0) && (x < x1))
            {
                result = (x / (x1 - x0)) - (x0 / (x1 - x0));
            }
            else
            {
                result = (-x / (x3 - x2)) + (x3 / (x3 - x2));
            }
            return result;
        }

    }
}
