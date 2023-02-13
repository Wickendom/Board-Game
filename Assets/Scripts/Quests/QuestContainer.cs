using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestContainer : MonoBehaviour
{
    Quest quest;

    public MapTypeData mapType;

    public Quest.QuestType questType;

    public List<EnemyData> enemiesToKill;
    public int enemyAmountToKill;
    public ItemData itemToFind;

    [SerializeField]
    TextMeshProUGUI mapTypeUI;
    [SerializeField]
    TextMeshProUGUI questTypeUI;
    [SerializeField]
    TextMeshProUGUI EnemyToKillUI;
    [SerializeField]
    TextMeshProUGUI AmountOfEnemiesToKillUI;
    [SerializeField]
    TextMeshProUGUI ItemToReteiveUI;

    public void Initialise(Quest quest)
    {
        mapType = quest.mapTypeData;

        this.quest = quest;

        questType = quest.questType;

        enemiesToKill = quest.enemiesToKill;
        enemyAmountToKill = quest.enemyAmountToKill;
        itemToFind = quest.itemToFind;

        UpdateUI();
    }

    private void UpdateUI()
    {
        mapTypeUI.text = mapType.mapName;
        questTypeUI.text = questType.ToString();
        EnemyToKillUI.text = (enemiesToKill[0] == null)?"":enemiesToKill[0].enemyName;
        AmountOfEnemiesToKillUI.text = enemyAmountToKill.ToString();
        ItemToReteiveUI.text = (itemToFind == null)?"":itemToFind.itemName;
    }

    public void VoteQuest()
    {
        Debug.Log("Quest Voted");
        QuestController.VoteOnQuest(quest);
    }
}
