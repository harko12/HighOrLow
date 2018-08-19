using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HarkoGames
{
    public enum FuzzyDefinitionType
    {
        Grade, ReverseGrade, Triangle, Trapezoid
    }

    [System.Serializable]
    public class FuzzyDefinition
    {
        public string name;
        public FuzzyDefinitionType myType;
        public float[] limits;
        public bool IsValid;

        public void Organize()
        {
            System.Array.Sort(limits);
        }

        public float GetTop()
        {
            Organize();
            if (limits.Count() == 0)
            {
                return 0f;
            }
            return limits[limits.Length - 1];
        }

        public float GetBottom()
        {
            Organize();
            if (limits.Count() == 0)
            {
                return 0f;
            }
            return limits[0];
        }
    }

    public class FuzzyValue : ScriptableObject
    {
        public List<FuzzyDefinition> definitions;
        public bool IsValid;

        [MenuItem("Assets/FuzzyLogic/Create/FuzzyValue")]
        public static void CreateAsset()
        {
            var newObject = ScriptableObjectUtility.CreateAsset<FuzzyValue>("Assets/Harko Games/Fuzzy Logic");
            newObject.name = "Fuzzy Logic ##";
        }
        public Dictionary<string, float> Evaluate(float val)
        {
            var results = new Dictionary<string, float>();
            foreach (var def in definitions)
            {
                float inclusion = 0f;
                switch (def.myType)
                {
                    case FuzzyDefinitionType.Grade:
                        inclusion = FuzzyMath.Grade(val, def.limits[0], def.limits[1]);
                        break;
                    case FuzzyDefinitionType.ReverseGrade:
                        inclusion = FuzzyMath.ReverseGrade(val, def.limits[0], def.limits[1]);
                        break;
                    case FuzzyDefinitionType.Triangle:
                        inclusion = FuzzyMath.Triangle(val, def.limits[0], def.limits[1], def.limits[2]);
                        break;
                    case FuzzyDefinitionType.Trapezoid:
                        inclusion = FuzzyMath.Trapezoid(val, def.limits[0], def.limits[1], def.limits[2], def.limits[3]);
                        break;
                }
                if (inclusion > 0)
                {
                    results.Add(def.name, inclusion);
                }
            }
            return results;
        }

        public void Organize()
        {
            if (definitions.Count == 0)
            {
                return;
            }
            definitions = definitions.OrderBy(d => d.GetBottom()).ToList();
            var defCount = definitions.Count;
            if (defCount < 2)
            {
                return;
            }

            var firstDef = definitions[0];
            var lastBottom = firstDef.GetBottom();
            var lastTop = firstDef.GetTop();
            IsValid = true;
            for(int lcv = 1; lcv < defCount; lcv++)
            {
                var thisDef = definitions[lcv];
                if (thisDef.GetTop() < lastTop || thisDef.GetBottom() < lastBottom)
                {
                    thisDef.IsValid = false;
                    IsValid = false;
                    continue;
                }
                thisDef.IsValid = true;
                lastTop = thisDef.GetTop();
                lastBottom = thisDef.GetBottom();
            }
        }

        public void Add(FuzzyDefinition def)
        {
            if (definitions == null)
            {
                definitions = new List<FuzzyDefinition>();
            }
            definitions.Add(def);
        }

        public void Remove(FuzzyDefinition def)
        {
            definitions = definitions.Where(d => d.name != def.name).ToList();
        }
    }
}
