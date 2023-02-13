using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class QuestTracker : MonoBehaviour
{
    public static QuestTracker Instance;

    private PhotonView photonView;

    private static Quest currentQuest;
    private static QuestUITracker questUITracker;

    private static int currentEnemiesKilled = 0;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != null)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        photonView = GetComponent<PhotonView>();
    }

    public static void SetQuest(Quest quest)
    {
        currentQuest = quest;
    }

    public static Quest GetQuest()
    {
        return currentQuest;
    }

    public static void SetQuestUIObject(QuestUITracker UITracker)
    {
        questUITracker = UITracker;
    }

    public static void CheckEnemyDeathForQuest(EnemyData enemyData)
    {
        bool matchingEnemy = false;

        if(currentQuest != null)
        {
            for (int i = 0; i < currentQuest.enemiesToKill.Count; i++)
            {
                if (enemyData == currentQuest.enemiesToKill[i])
                {
                    matchingEnemy = true;
                    break;
                }
            }

            if (matchingEnemy == true)
            {
                IncrementCurrentEnemyKilledAmount();
            }
        }
    }

    private static void IncrementCurrentEnemyKilledAmount()
    {
        currentEnemiesKilled++;
        questUITracker.IncrementCurrentEnemyKilledAmount(currentEnemiesKilled);

        if (currentEnemiesKilled >= currentQuest.enemyAmountToKill)
        {
            SavingManager.savePlayer();
            NetworkSceneManager.Instance.NetworkLoadSceneAsync(0);
        }
    }

    public static void Reset()
    {
        currentQuest = null;
    }
}
