using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerClass))]
public class PlayerClassEditor : Editor
{
    PlayerClass data;
    SerializedObject dataTarget;

    SerializedProperty skillsList;

    bool foldoutSkillsMenu = false;

    private void OnEnable()
    {
        data = (PlayerClass)target;
        dataTarget = new SerializedObject(target);
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        dataTarget.Update();

        EditorGUILayout.PropertyField(dataTarget.FindProperty("className"));
        EditorGUILayout.PropertyField(dataTarget.FindProperty("ID"));
        EditorGUILayout.PropertyField(dataTarget.FindProperty("modelPrefab"));
        EditorGUILayout.PropertyField(dataTarget.FindProperty("initiativeTrackerSprite"));

        skillsList = dataTarget.FindProperty("skillsAbleToLearn");

        EditorGUILayout.PropertyField(dataTarget.FindProperty("baseVitalityLevel"));
        EditorGUILayout.PropertyField(dataTarget.FindProperty("baseCombatEffectivenessLevel"));
        EditorGUILayout.PropertyField(dataTarget.FindProperty("baseStrengthLevel"));
        EditorGUILayout.PropertyField(dataTarget.FindProperty("baseDexterityLevel"));
        EditorGUILayout.PropertyField(dataTarget.FindProperty("baseIntelligenceLevel"));
        EditorGUILayout.PropertyField(dataTarget.FindProperty("baseSpeedLevel"));
        EditorGUILayout.PropertyField(dataTarget.FindProperty("basePerceptionLevel"));

        EditorGUILayout.PropertyField(dataTarget.FindProperty("startingHeadArmour"));
        EditorGUILayout.PropertyField(dataTarget.FindProperty("startingTorsoArmour"));
        EditorGUILayout.PropertyField(dataTarget.FindProperty("startingBeltArmour"));
        EditorGUILayout.PropertyField(dataTarget.FindProperty("startingLegsArmour"));
        EditorGUILayout.PropertyField(dataTarget.FindProperty("startingFeetArmour"));

        EditorGUILayout.PropertyField(dataTarget.FindProperty("startingWeapon"));

        for (int i = 0; i < skillsList.arraySize; i++)
        {
            GUILayout.BeginHorizontal();
            if (skillsList.arraySize > i)
            {
                EditorGUILayout.PropertyField(skillsList.GetArrayElementAtIndex(i), new GUIContent("Skill Milestone " + (i+1).ToString()));
            }
            if (GUILayout.Button("X"))
            {
                data.RemoveSkillMilestone(i);
            }
            GUILayout.EndHorizontal();
        }

        if(GUILayout.Button("Add New Skill Level Milestone"))
        {
            data.AddNewSkillTier();
        }

        dataTarget.ApplyModifiedProperties();
    }
}
