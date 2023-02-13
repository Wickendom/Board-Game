using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StatTypeData))]
public class StatDataEditor : Editor
{
    StatTypeData data;

    SerializedObject dataTarget;

    SerializedProperty statLevelRequirements;
    SerializedProperty statDice;
    SerializedProperty statModifiers;

    public void OnEnable()
    {
        data = (StatTypeData)target;
        dataTarget = new SerializedObject(target);
    }

    public override void OnInspectorGUI()
    {
        dataTarget.Update();

        statLevelRequirements = dataTarget.FindProperty("levelRequirement");
        statDice = dataTarget.FindProperty("statDice");
        statModifiers = dataTarget.FindProperty("statModifier");

        EditorGUILayout.BeginHorizontal();
        
        EditorGUIUtility.labelWidth = 15;
        EditorGUILayout.LabelField("Stat Level");

        EditorGUIUtility.labelWidth = 20;
        EditorGUILayout.LabelField("Stat Die");
        
        EditorGUILayout.LabelField("Stat Modifier");
        
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < statLevelRequirements.arraySize; i++)
        {
            EditorGUILayout.BeginHorizontal();
            //GUILayout.FlexibleSpace();
            if (statLevelRequirements.arraySize > i)
            {
                //EditorGUIUtility.labelWidth = 50;
                EditorGUILayout.PropertyField(statLevelRequirements.GetArrayElementAtIndex(i), new GUIContent(""));
            }

            if (statDice.arraySize > i)
            {
                EditorGUILayout.PropertyField(statDice.GetArrayElementAtIndex(i), new GUIContent(""));
            }

            if (statModifiers.arraySize > i)
            {
                EditorGUILayout.PropertyField(statModifiers.GetArrayElementAtIndex(i), new GUIContent(""));
            }

            if (GUILayout.Button("X"))
            {
                data.RemoveStatLevel(i);
            }

            EditorGUILayout.EndHorizontal();            
        }

        if (GUILayout.Button("Add New Normal Tile"))
        {
            data.AddStatLevel();
        }

        dataTarget.ApplyModifiedProperties();
    }
}
