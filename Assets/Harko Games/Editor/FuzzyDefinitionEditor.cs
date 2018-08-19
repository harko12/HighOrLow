using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HarkoGames
{
    [CustomPropertyDrawer(typeof(FuzzyDefinition))]
    public class FuzzyDefinitionEditor : PropertyDrawer
    {
        private float defaultLineHeight = 16f;
        private float linePadding = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var nameProp = property.FindPropertyRelative("name");
            var typeProp = property.FindPropertyRelative("myType");
            var limitsProp = property.FindPropertyRelative("limits");
            label = EditorGUI.BeginProperty(position, label, property);
            var nameRect = new Rect(position.x, position.y, position.width, defaultLineHeight);
            var typeRect = new Rect(position.x, nameRect.y + defaultLineHeight + linePadding, position.width, defaultLineHeight);
            var limitRect = new Rect(position.x, typeRect.y + defaultLineHeight + linePadding, position.width, (defaultLineHeight * 2) + linePadding);
            EditorGUI.PropertyField(nameRect, nameProp, GUIContent.none);
            EditorGUI.PropertyField(typeRect, typeProp, GUIContent.none);
            ShowLimits(limitRect, typeProp, limitsProp);
            EditorGUI.EndProperty();
        }

        private void ShowLimits(Rect limitRect, SerializedProperty typeProp, SerializedProperty limitsProp)
        {
            var myType = (FuzzyDefinitionType)typeProp.intValue;

            var width = limitRect.width / 4f;
            var rects = new Rect[4, 2];
            for (int y = 0; y < 2; y++)
            {
                var yPos = limitRect.y + (y * (defaultLineHeight + linePadding));
                for (int x = 0; x < 4; x++)
                {
                    var r = new Rect(limitRect.x + (x * width), yPos, width, defaultLineHeight);
                    rects[x, y] = r;
                }
            }

            switch (myType)
            {
                case FuzzyDefinitionType.Grade:
                    ShowLimits_grade(rects, limitsProp);
                    break;
                case FuzzyDefinitionType.ReverseGrade:
                    ShowLimits_reverseGrade(rects, limitsProp);
                    break;
                case FuzzyDefinitionType.Triangle:
                    ShowLimits_triangle(rects, limitsProp);
                    break;
                case FuzzyDefinitionType.Trapezoid:
                    ShowLimits_trapezoid(rects, limitsProp);
                    break;
            }
        }

        private void ShowLimits_grade(Rect[,] rects,  SerializedProperty limitsProp)
        {
            limitsProp.arraySize = 2;
            EditorGUI.LabelField(rects[0, 0], "");
            EditorGUI.LabelField(rects[1, 0], "");
            EditorGUI.PropertyField(rects[2, 0], limitsProp.GetArrayElementAtIndex(1), GUIContent.none);
            EditorGUI.LabelField(rects[3, 0], "");
            EditorGUI.LabelField(rects[0, 1], "");
            EditorGUI.PropertyField(rects[1, 1], limitsProp.GetArrayElementAtIndex(0), GUIContent.none);
            EditorGUI.LabelField(rects[2, 1], "");
            EditorGUI.LabelField(rects[3, 1], "");
        }

        private void ShowLimits_reverseGrade(Rect[,] rects, SerializedProperty limitsProp)
        {
            limitsProp.arraySize = 2;
            EditorGUI.LabelField(rects[0, 0], "");
            EditorGUI.PropertyField(rects[1, 0], limitsProp.GetArrayElementAtIndex(0), GUIContent.none);
            EditorGUI.LabelField(rects[2, 0], "");
            EditorGUI.LabelField(rects[3, 0], "");
            EditorGUI.LabelField(rects[0, 1], "");
            EditorGUI.LabelField(rects[1, 1], "");
            EditorGUI.PropertyField(rects[2, 1], limitsProp.GetArrayElementAtIndex(1), GUIContent.none);
            EditorGUI.LabelField(rects[3, 1], "");
        }

        private void ShowLimits_triangle(Rect[,] rects, SerializedProperty limitsProp)
        {
            limitsProp.arraySize = 3;
            EditorGUI.LabelField(rects[0, 0], "");
            EditorGUI.PropertyField(rects[1, 0], limitsProp.GetArrayElementAtIndex(1), GUIContent.none);
            EditorGUI.LabelField(rects[2, 0], "");
            EditorGUI.LabelField(rects[3, 0], "");
            EditorGUI.PropertyField(rects[0, 1], limitsProp.GetArrayElementAtIndex(0), GUIContent.none);
            EditorGUI.LabelField(rects[1, 1], "");
            EditorGUI.LabelField(rects[2, 1], "");
            EditorGUI.PropertyField(rects[3, 1], limitsProp.GetArrayElementAtIndex(2), GUIContent.none);
        }

        private void ShowLimits_trapezoid(Rect[,] rects, SerializedProperty limitsProp)
        {
            limitsProp.arraySize = 4;
            EditorGUI.LabelField(rects[0, 0], "");
            EditorGUI.PropertyField(rects[1, 0], limitsProp.GetArrayElementAtIndex(1), GUIContent.none);
            EditorGUI.PropertyField(rects[2, 0], limitsProp.GetArrayElementAtIndex(2), GUIContent.none);
            EditorGUI.LabelField(rects[3, 0], "");
            EditorGUI.PropertyField(rects[0, 1], limitsProp.GetArrayElementAtIndex(0), GUIContent.none);
            EditorGUI.LabelField(rects[1, 1], "");
            EditorGUI.LabelField(rects[2, 1], "");
            EditorGUI.PropertyField(rects[3, 1], limitsProp.GetArrayElementAtIndex(3), GUIContent.none);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (defaultLineHeight + linePadding) * 4;
        }
    }
}
