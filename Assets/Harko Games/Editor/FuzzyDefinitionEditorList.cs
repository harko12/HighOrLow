using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Flags]
public enum FuzzyDefinitionEditorListOptions
{
    None = 0,
    ListSize = 1,
    ListLabel = 2,
    Buttons = 8,
    Default =  Buttons,
    Default2 = ListSize | ListLabel | Buttons
}

public static class FuzzyDefinitionEditorList
{
    private static GUIContent
            moveButtonContent = new GUIContent("\u21b4", "move down"),
            duplicateButtonContent = new GUIContent("+", "duplicate"),
            deleteButtonContent = new GUIContent("-", "delete"),
            addButtonContent = new GUIContent("+", "add element");

    public static void Show(SerializedProperty list, FuzzyDefinitionEditorListOptions options = FuzzyDefinitionEditorListOptions.Default)
    {
        bool
            showListLabel = (options & FuzzyDefinitionEditorListOptions.ListLabel) != 0,
            showButtons = (options & FuzzyDefinitionEditorListOptions.Buttons) != 0,
            showListSize = (options & FuzzyDefinitionEditorListOptions.ListSize) != 0;

        if (showListLabel)
        {
            EditorGUILayout.PropertyField(list);
            EditorGUI.indentLevel += 1;
        }
        if (list.isExpanded)
        {
            if (showListSize)
            {
                EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
            }
            for (int lcv = 0, length = list.arraySize; lcv < length; lcv++)
            {
                if (showButtons)
                {
                    EditorGUILayout.BeginHorizontal();
                }
                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(lcv));
                if (showButtons)
                {
                    ShowButtons(list, lcv);
                    EditorGUILayout.EndHorizontal();
                }
            }
            if (/*showButtons && list.arraySize == 0 &&*/ GUILayout.Button(addButtonContent, EditorStyles.miniButton))
            {
                list.arraySize += 1;
            }
        }
        if (showListLabel)
        {
            EditorGUI.indentLevel -= 1;
        }
    }

    private static GUILayoutOption miniButtonWidth = GUILayout.Width(20f);

    private static void ShowButtons(SerializedProperty list, int index)
    {
        /*
        if (GUILayout.Button(moveButtonContent, EditorStyles.miniButtonLeft, miniButtonWidth))
        {
            list.MoveArrayElement(index, index + 1);
        }
        if (GUILayout.Button(duplicateButtonContent, EditorStyles.miniButtonMid, miniButtonWidth))
        {
            list.InsertArrayElementAtIndex(index);
        }
        */
        if (GUILayout.Button(deleteButtonContent, EditorStyles.miniButtonRight, miniButtonWidth))
        {
            var oldSize = list.arraySize;
            list.DeleteArrayElementAtIndex(index);
            if (list.arraySize == oldSize) // retry if it didn't delete for some reason
            {
                list.DeleteArrayElementAtIndex(index);
            }
        }
    }
}
