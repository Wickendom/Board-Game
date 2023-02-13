using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Map Type", menuName = "Data's/MapType"),System.Serializable]
public class MapTypeData : ScriptableObject
{
    public string mapName;
    public string ID;

    [SerializeField]
    private GameObject defaultTile;

    [Header("Tile Type Spawn Chances")]
    public float normalTileSpawnChance = 25;
    public float hazardTileSpawnChance = 25;
    public float impassibleTileSpawnChance = 25;
    public float lootTileSpawnChance = 25;

    public List<GameObject> normalTilePrefabs = new List<GameObject>();
    public List<GameObject> hazardTilePrefabs = new List<GameObject>();
    public List<GameObject> impassibleTilePrefabs = new List<GameObject>();
    public List<GameObject> lootTilePrefabs = new List<GameObject>();

    public List<float> normalTileIndividualSpawnChances = new List<float>();
    public List<float> hazardTileIndividualSpawnChances = new List<float>();
    public List<float> impassibleTileIndividualSpawnChances = new List<float>();
    public List<float> lootTileIndividualSpawnChances = new List<float>();

    public EnemyData[] enemiesThatCanSpawn;

    public bool CheckSpawnChancesMatchRequirement()
    {
        float value = normalTileSpawnChance + hazardTileSpawnChance + impassibleTileSpawnChance + lootTileSpawnChance;
        if(value != 100)
        {
            Debug.LogError(mapName + " data tile spawn chances do not match 100% this will break the generation");
            return false;
        }

        return true;
    }

    public Vector2 CreateRandomTileGameObjectIDs()
    {
        GameObject[][] tileTypeArray = new GameObject[][] { normalTilePrefabs.ToArray(),hazardTilePrefabs.ToArray(),impassibleTilePrefabs.ToArray(),lootTilePrefabs.ToArray()};

        float[] spawnChances = new float[] {normalTileSpawnChance,hazardTileSpawnChance,impassibleTileSpawnChance,lootTileSpawnChance};
        float[][] individualSpawnChances = new float[][] { normalTileIndividualSpawnChances.ToArray(), hazardTileIndividualSpawnChances.ToArray(), impassibleTileIndividualSpawnChances.ToArray(), lootTileIndividualSpawnChances.ToArray() };

        float randomValue = Random.Range(0, 100);
        float individuleRandomValue = Random.Range(0, 100);

        float currentProb = 0;
        float currentIndividualProb = 0;

        int tileTypeIndex = 0;
        int individualTileIndex = 0;

        foreach(int spawnChance in spawnChances)
        {
            currentProb += spawnChance;
            if (randomValue <= currentProb)
            {
                foreach(int individualSpawnChance in individualSpawnChances[tileTypeIndex])
                {
                    currentIndividualProb += individualSpawnChance;
                    if(individuleRandomValue <= currentIndividualProb)
                    {
                        return new Vector2(tileTypeIndex,individualTileIndex);
                    }

                    individualTileIndex++;
                }
                
            }

            tileTypeIndex++;
        }

        Debug.LogError("Random tile spawn chance value was not met. no tile GameObject given");
        return Vector2.zero;
    }

    public GameObject GetTileGameObjectByID(int tileTypeIndex,int individualTileIndex)
    {
        GameObject[][] tileTypeArray = new GameObject[][] { normalTilePrefabs.ToArray(), hazardTilePrefabs.ToArray(), impassibleTilePrefabs.ToArray(), lootTilePrefabs.ToArray() };

        return tileTypeArray[tileTypeIndex][individualTileIndex];
    }

    public void AddNewNormalTile()
    {
        normalTilePrefabs.Add(defaultTile);
        normalTileIndividualSpawnChances.Add(10);
    }

    public void AddNewHazardTile()
    {
        hazardTilePrefabs.Add(defaultTile);
        hazardTileIndividualSpawnChances.Add(10);
    }

    public void AddNewImpassibleTile()
    {
        impassibleTilePrefabs.Add(defaultTile);
        impassibleTileIndividualSpawnChances.Add(10);
    }

    public void AddNewLootTile()
    {
        lootTilePrefabs.Add(defaultTile);
        lootTileIndividualSpawnChances.Add(10);
    }

    public void RemoveNormalTile(int index)
    {
        if(normalTilePrefabs.Count - 1 >= index)
            normalTilePrefabs.RemoveAt(index);
        
        if(normalTileIndividualSpawnChances.Count - 1 >= index)
            normalTileIndividualSpawnChances.RemoveAt(index);
    }

    public void RemoveHazardTile(int index)
    {
        hazardTilePrefabs.RemoveAt(index);
        hazardTileIndividualSpawnChances.RemoveAt(index);
    }

    public void RemoveImpassibleTile(int index)
    {
        impassibleTilePrefabs.RemoveAt(index);
        impassibleTileIndividualSpawnChances.RemoveAt(index);
    }

    public void RemoveLootTile(int index)
    {
        lootTilePrefabs.RemoveAt(index);
        lootTileIndividualSpawnChances.RemoveAt(index);
    }
}
