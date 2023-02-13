using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestUITracker : MonoBehaviour
{
    TextMeshProUGUI questTypeUI;
    TextMeshProUGUI EnemyToKillUI;
    TextMeshProUGUI AmountOfEnemiesToKillUI;
    TextMeshProUGUI currentAmountOfEnemiesToKillUI;
    TextMeshProUGUI ItemToReteiveUI;

    private bool ableToUpdate = false;

    // Start is called before the first frame update
    void Start()
    {
        if(QuestTracker.GetQuest() != null)
        {
            TextMeshProUGUI[] UIs = GetComponentsInChildren<TextMeshProUGUI>();

            questTypeUI = UIs[0];
            EnemyToKillUI = UIs[1];
            currentAmountOfEnemiesToKillUI = UIs[2];
            AmountOfEnemiesToKillUI = UIs[3];
            ItemToReteiveUI = UIs[4];

            ableToUpdate = true;

            InitialUIUpdate(QuestTracker.GetQuest());

            QuestTracker.SetQuestUIObject(this);
        }
    }
    
    void InitialUIUpdate(Quest quest)
    {
        if(ableToUpdate)
        {
            questTypeUI.text = quest.questType.ToString();
            EnemyToKillUI.text = (quest.enemiesToKill[0] == null) ? "" : quest.enemiesToKill[0].enemyName;
            AmountOfEnemiesToKillUI.text = quest.enemyAmountToKill.ToString();
            ItemToReteiveUI.text = (quest.itemToFind == null) ? "" : quest.itemToFind.itemName;
        }
    }

    public void IncrementCurrentEnemyKilledAmount(int currentQuestEnemiesKilled)
    {
        currentAmountOfEnemiesToKillUI.text = currentQuestEnemiesKilled.ToString();
    }
}
