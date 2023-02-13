using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BoardGame;
[CustomEditor(typeof(Status))]
public class StatusEditor : Editor
{
    Status data;
    SerializedObject dataTarget;

    private void OnEnable()
    {
        data = (Status)target;
        dataTarget = new SerializedObject(target);
    }

    public override void OnInspectorGUI()
    {
        dataTarget.Update();

        EditorGUILayout.PropertyField(dataTarget.FindProperty("duration"));
        EditorGUILayout.PropertyField(dataTarget.FindProperty("statusEffector"));

        if(data.statusEffector == Status.StatusEffector.MovementModifier || 
           data.statusEffector == Status.StatusEffector.AttackDamage ||
           data.statusEffector == Status.StatusEffector.AttackHit ||
           data.statusEffector == Status.StatusEffector.Initiative)
        {
            EditorGUILayout.PropertyField(dataTarget.FindProperty("negative"));
            EditorGUILayout.PropertyField(dataTarget.FindProperty("modifier"));
            EditorGUILayout.PropertyField(dataTarget.FindProperty("dieModifier"));
        }

        if (data.statusEffector == Status.StatusEffector.EndOfTurnEffect||
            data.statusEffector == Status.StatusEffector.MovementValue)
        {
            EditorGUILayout.PropertyField(dataTarget.FindProperty("intValue"));
        }


        dataTarget.ApplyModifiedProperties();
    }
}
