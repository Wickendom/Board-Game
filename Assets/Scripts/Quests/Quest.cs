using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public MapTypeData mapTypeData;

    public int questID;

    public enum QuestType {Assassinate, Eliminate, Retrieve};
    public QuestType questType = QuestType.Assassinate;

    public List<EnemyData> enemiesToKill;
    public int enemyAmountToKill;
    public ItemData itemToFind;

    public Quest (string mapTypeDataID, int questID, int questTypeID, string[] enemiesToKillIDs, int enemyAmountToKill, string itemToFindID)
    {
        mapTypeData = Lists.GetMapData(mapTypeDataID);
        this.questID = questID;
        questType = (Quest.QuestType)questTypeID;
        enemiesToKill = new List<EnemyData>();

        for (int i = 0; i < enemiesToKillIDs.Length; i++)
        {
            enemiesToKill.Add(Lists.GetEnemyData(enemiesToKillIDs[i]));
        }

        this.enemyAmountToKill = enemyAmountToKill;
        if(questType == QuestType.Retrieve)
        {
            itemToFind = Lists.GetItemData(itemToFindID);
        }
    }

    public Quest(int id)
    {
        enemiesToKill = new List<EnemyData>();
        questID = id;
        CreateRandomQuest();
    }

    public string[] GetEnemyIDs()
    {
        string[] enemyIDs = new string[enemiesToKill.Count];

        for (int i = 0; i < enemiesToKill.Count; i++)
        {
            enemyIDs[i] = enemiesToKill[i].ID;
        }

        return enemyIDs;
    }

    public string GetItemID()
    {
        if(itemToFind != null)
        {
            return itemToFind.ID;
        }

        return null;
    }

    private void CreateRandomQuest()
    {
        mapTypeData = Lists.GetAllMapDatas()[Random.Range(0, Lists.GetAllMapDatas().Count - 1)];

        questType = (QuestType)Random.Range(0, 2);

        switch(questType)
        {
            case QuestType.Assassinate:
                {
                    enemiesToKill.Add(mapTypeData.enemiesThatCanSpawn[Random.Range(0, mapTypeData.enemiesThatCanSpawn.Length - 1)]);
                    enemyAmountToKill = 1;
                    break;
                }
            case QuestType.Eliminate:
                {
                    enemiesToKill.Add(mapTypeData.enemiesThatCanSpawn[Random.Range(0, mapTypeData.enemiesThatCanSpawn.Length - 1)]);
                    enemyAmountToKill = Random.Range(1, 10);
                    break;
                }
            case QuestType.Retrieve:
                {
                    itemToFind = Lists.GetAllItemDatas()[Random.Range(0, Lists.GetAllItemDatas().Count - 1)];
                    break;
                }
        }
    }

    public bool CheckQuestComplete()
    {
        return false;
    }
}