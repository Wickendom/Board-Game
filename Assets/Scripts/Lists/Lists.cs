using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lists : MonoBehaviour
{
    public List<EnemyData> enemyDatas;
    private static List<EnemyData> _enemyDatas;

    public List<ItemData> itemDatas;
    private static List<ItemData> _itemDatas;

    public List<MapTypeData> mapDatas;
    private static List<MapTypeData> _mapDatas;

    public List<StatTypeData> statTypes;
    private static List<StatTypeData> _statTypes;

    public List<PlayerClass> playerClasses;
    private static List<PlayerClass> _playerClasses;

    private static int characterIDToken;

    private void Awake()
    {
        _enemyDatas = enemyDatas;
        _mapDatas = mapDatas;
        _statTypes = statTypes;
        _playerClasses = playerClasses;
        _itemDatas = itemDatas;
    }

    public static List<EnemyData> GetAllEnemyDatas()
    {
        return _enemyDatas;
    }

    public static EnemyData GetEnemyData(string enemyID)
    {
        for (int i = 0; i < _enemyDatas.Count; i++)
        {
            if (_enemyDatas[i].ID == enemyID)
            {
                return _enemyDatas[i];
            }
        }

        Debug.Log("Map Data not found with ID '" + enemyID + "'");
        return null;
    }

    public static List<ItemData> GetAllItemDatas()
    {
        return _itemDatas;
    }

    public static ItemData GetItemData(string itemID)
    {
        for (int i = 0; i < _itemDatas.Count; i++)
        {
            if (_itemDatas[i].ID == itemID)
            {
                return _itemDatas[i];
            }
        }

        Debug.Log("Item Data not found with ID '" + itemID + "'");
        return null;
    }

    public static List<MapTypeData> GetAllMapDatas()
    {
        return _mapDatas;
    }

    public static MapTypeData GetMapData(string mapID)
    {
        for (int i = 0; i < _mapDatas.Count; i++)
        {
            if (_mapDatas[i].ID == mapID)
            {
                return _mapDatas[i];
            }
        }

        Debug.Log("Map Data not found with ID '" + mapID + "'");
        return null;
    }

    public static List<StatTypeData> GetAllStatTypeDatas()
    {
        return _statTypes;
    }

    public static List<PlayerClass> GetAllPlayerClasses()
    {
        return _playerClasses;
    }

    public static PlayerClass GetPlayerClass(string classID)
    {
        for (int  i = 0;  i < _playerClasses.Count;  i++)
        {
            if(_playerClasses[i].ID == classID)
            {
                return _playerClasses[i];
            }
        }

        Debug.Log("Player Class not found with ID '" + classID + "'");
        return null;
    }

    public static int CreateCharacterID()
    {
        int temp = characterIDToken;
        characterIDToken++;
        return temp;
    }
}
