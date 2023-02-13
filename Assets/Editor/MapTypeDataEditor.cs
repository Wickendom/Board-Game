using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapTypeData))]
public class MapTypeDataEditor : Editor
{
    MapTypeData data;

    SerializedObject dataTarget;

    SerializedProperty ID;

    SerializedProperty normalTilePrefabsProp;
    SerializedProperty hazardTilePrefabsProp;
    SerializedProperty impassibleTilePrefabsProp;
    SerializedProperty lootTilePrefabsProp;

    SerializedProperty normalTileIndividualSpawnChanceProp;
    SerializedProperty hazardTileIndividualSpawnChanceProp;
    SerializedProperty impassibleTileIndividualSpawnChanceProp;
    SerializedProperty lootTileIndividualSpawnChanceProp;

    bool normalFoldout = true;
    bool hazardFoldout = true;
    bool impassibleFoldout = true;
    bool lootFoldout = true;

    SerializedProperty enemiesToSpawn;

    public void OnEnable()
    {
        data = (MapTypeData)target;
        dataTarget = new SerializedObject(target);
    }

    public override void OnInspectorGUI()
    {
        dataTarget.Update();

        data.mapName = EditorGUILayout.TextField("Map Name", data.mapName);
        ID = dataTarget.FindProperty("ID");
        EditorGUILayout.PropertyField(ID, new GUIContent("ID"));

        GUILayout.Space(10);

        data.normalTileSpawnChance = EditorGUILayout.FloatField("Normal Tile Spawn Chance", data.normalTileSpawnChance);
        data.hazardTileSpawnChance = EditorGUILayout.FloatField("Hazard Tile Spawn Chance", data.hazardTileSpawnChance);
        data.impassibleTileSpawnChance = EditorGUILayout.FloatField("Impassible Tile Spawn Chance", data.impassibleTileSpawnChance);
        data.lootTileSpawnChance = EditorGUILayout.FloatField("Loot Tile Spawn Chance", data.lootTileSpawnChance);

        float tileTypeSpawnChance = 0;

        tileTypeSpawnChance = data.normalTileSpawnChance + data.hazardTileSpawnChance + data.impassibleTileSpawnChance + data.lootTileSpawnChance;
        

        EditorGUILayout.LabelField("Total tile type spawn chance is " + tileTypeSpawnChance.ToString() + "%");

        if (tileTypeSpawnChance != 100)
        {
            EditorGUILayout.LabelField("Tile type spawn chances do not equal 100%. Please adjust spawn chances to make them add up to 100%");
        }

        GUILayout.Space(10);

        normalTilePrefabsProp = dataTarget.FindProperty("normalTilePrefabs");
        hazardTilePrefabsProp = dataTarget.FindProperty("hazardTilePrefabs");
        impassibleTilePrefabsProp = dataTarget.FindProperty("impassibleTilePrefabs");
        lootTilePrefabsProp = dataTarget.FindProperty("lootTilePrefabs");

        normalTileIndividualSpawnChanceProp = dataTarget.FindProperty("normalTileIndividualSpawnChances");
        hazardTileIndividualSpawnChanceProp = dataTarget.FindProperty("hazardTileIndividualSpawnChances");
        impassibleTileIndividualSpawnChanceProp = dataTarget.FindProperty("impassibleTileIndividualSpawnChances");
        lootTileIndividualSpawnChanceProp = dataTarget.FindProperty("lootTileIndividualSpawnChances");

        GUILayout.Space(10);

        normalFoldout = EditorGUILayout.Foldout(normalFoldout, new GUIContent("Normal Tiles"));
        EditorGUI.indentLevel++;
        if(normalFoldout)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Tile");
            EditorGUILayout.LabelField("Spawn Chance");
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < normalTilePrefabsProp.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                //GUILayout.FlexibleSpace();
                if (normalTilePrefabsProp.arraySize > i)
                {
                    //EditorGUIUtility.labelWidth = 50;
                    EditorGUILayout.PropertyField(normalTilePrefabsProp.GetArrayElementAtIndex(i), new GUIContent(""));
                }

                if (normalTileIndividualSpawnChanceProp.arraySize > i)
                {
                    EditorGUILayout.PropertyField(normalTileIndividualSpawnChanceProp.GetArrayElementAtIndex(i), new GUIContent(""));
                }

                if (GUILayout.Button("X"))
                {
                    data.RemoveNormalTile(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add New Normal Tile"))
            {
                data.AddNewNormalTile();
            }

            float normalTotalSpawnChance = 0;

            for (int i = 0; i < normalTilePrefabsProp.arraySize; i++)
            {
                normalTotalSpawnChance += normalTileIndividualSpawnChanceProp.GetArrayElementAtIndex(i).floatValue;
            }

            EditorGUILayout.LabelField("Total normal tile spawn chance is " + normalTotalSpawnChance.ToString() + "%");

            if(normalTotalSpawnChance != 100)
            {
                EditorGUILayout.LabelField("Normal tiles spawn chances do not equal 100%. Please adjust spawn chances to make them add up to 100%");
            }
        }
        EditorGUI.indentLevel--;

        GUILayout.Space(10);
        
        hazardFoldout = EditorGUILayout.Foldout(hazardFoldout, new GUIContent("Hazard Tiles"));
        EditorGUI.indentLevel++;
        if (hazardFoldout)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Tile");
            EditorGUILayout.LabelField("Spawn Chance");
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < hazardTilePrefabsProp.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                //GUILayout.FlexibleSpace();
                if (hazardTilePrefabsProp.arraySize > i)
                {
                    //EditorGUIUtility.labelWidth = 50;
                    EditorGUILayout.PropertyField(hazardTilePrefabsProp.GetArrayElementAtIndex(i), new GUIContent(""));
                }

                if (hazardTileIndividualSpawnChanceProp.arraySize > i)
                {
                    EditorGUILayout.PropertyField(hazardTileIndividualSpawnChanceProp.GetArrayElementAtIndex(i), new GUIContent(""));
                }

                if (GUILayout.Button("X"))
                {
                    data.RemoveHazardTile(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add New Hazard Tile"))
            {
                data.AddNewHazardTile();
            }

            float hazardTotalSpawnChance = 0;

            for (int i = 0; i < hazardTilePrefabsProp.arraySize; i++)
            {
                hazardTotalSpawnChance += hazardTileIndividualSpawnChanceProp.GetArrayElementAtIndex(i).floatValue;
            }

            EditorGUILayout.LabelField("Total hazard tile spawn chance is " + hazardTotalSpawnChance.ToString() + "%");

            if (hazardTotalSpawnChance != 100)
            {
                EditorGUILayout.LabelField("Hazard tiles spawn chances do not equal 100%. Please adjust spawn chances to make them add up to 100%");
            }
        }
        EditorGUI.indentLevel--;
        GUILayout.Space(10);

        impassibleFoldout = EditorGUILayout.Foldout(impassibleFoldout, new GUIContent("Impassible Tiles"));
        EditorGUI.indentLevel++;
        if (impassibleFoldout)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Tile");
            EditorGUILayout.LabelField("Spawn Chance");
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < impassibleTilePrefabsProp.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                //GUILayout.FlexibleSpace();
                if (impassibleTilePrefabsProp.arraySize > i)
                {
                    //EditorGUIUtility.labelWidth = 50;
                    EditorGUILayout.PropertyField(impassibleTilePrefabsProp.GetArrayElementAtIndex(i), new GUIContent(""));
                }

                if (impassibleTileIndividualSpawnChanceProp.arraySize > i)
                {
                    EditorGUILayout.PropertyField(impassibleTileIndividualSpawnChanceProp.GetArrayElementAtIndex(i), new GUIContent(""));
                }

                if (GUILayout.Button("X"))
                {
                    data.RemoveImpassibleTile(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add New Impassible Tile"))
            {
                data.AddNewImpassibleTile();
            }

            float impassibleTotalSpawnChance = 0;

            for (int i = 0; i < impassibleTilePrefabsProp.arraySize; i++)
            {
                impassibleTotalSpawnChance += impassibleTileIndividualSpawnChanceProp.GetArrayElementAtIndex(i).floatValue;
            }

            EditorGUILayout.LabelField("Total impassible tile spawn chance is " + impassibleTotalSpawnChance.ToString() + "%");

            if (impassibleTotalSpawnChance != 100)
            {
                EditorGUILayout.LabelField("Impassible tiles spawn chances do not equal 100%. Please adjust spawn chances to make them add up to 100%");
            }
        }
        EditorGUI.indentLevel--;
        GUILayout.Space(10);

        lootFoldout = EditorGUILayout.Foldout(lootFoldout, new GUIContent("Loot Tiles"));
        EditorGUI.indentLevel++;
        if (lootFoldout)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Tile");
            EditorGUILayout.LabelField("Spawn Chance");
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < lootTilePrefabsProp.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                //GUILayout.FlexibleSpace();
                if (lootTilePrefabsProp.arraySize > i)
                {
                    //EditorGUIUtility.labelWidth = 50;
                    EditorGUILayout.PropertyField(lootTilePrefabsProp.GetArrayElementAtIndex(i), new GUIContent(""));
                }

                if (lootTileIndividualSpawnChanceProp.arraySize > i)
                {
                    EditorGUILayout.PropertyField(lootTileIndividualSpawnChanceProp.GetArrayElementAtIndex(i), new GUIContent(""));
                }

                if (GUILayout.Button("X"))
                {
                    data.RemoveLootTile(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add New Loot Tile"))
            {
                data.AddNewLootTile();
            }

            float lootTotalSpawnChance = 0;

            for (int i = 0; i < lootTilePrefabsProp.arraySize; i++)
            {
                lootTotalSpawnChance += lootTileIndividualSpawnChanceProp.GetArrayElementAtIndex(i).floatValue;
            }

            EditorGUILayout.LabelField("Total loot tile spawn chance is " + lootTotalSpawnChance.ToString() + "%");

            if (lootTotalSpawnChance != 100)
            {
                EditorGUILayout.LabelField("Loot tiles spawn chances do not equal 100%. Please adjust spawn chances to make them add up to 100%");
            }
        }
        EditorGUI.indentLevel--;

        enemiesToSpawn = dataTarget.FindProperty("enemiesThatCanSpawn");

        EditorGUILayout.PropertyField(enemiesToSpawn, new GUIContent("Enemies That Can Spawn"));

        dataTarget.ApplyModifiedProperties();
    }
}
