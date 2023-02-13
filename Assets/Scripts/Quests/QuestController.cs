using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class QuestController : MonoBehaviour, IInteractable
{
    private PhotonView photonView;

    List<Quest> quests;

    [SerializeField]
    private int questAmount = 5;

    [SerializeField]
    GuildQuestUIController questUIController;

    private static Quest selectedQuest;

    private static List<int> questIDVotes;//this is the votes for the quest 

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        quests = new List<Quest>();
        questIDVotes = new List<int>();

        if (PhotonNetwork.IsMasterClient)
        {
            QuestGenerator qGenerator = new QuestGenerator(this,questAmount);
            questUIController.SetQuestContainers(quests);
            
            for (int i = 0; i < quests.Count; i++)
            {
                photonView.RPC("ReceiveQuestFromMaster", RpcTarget.OthersBuffered, quests[i].mapTypeData.ID, quests[i].questID, (int)quests[i].questType, quests[i].GetEnemyIDs(), quests[i].enemyAmountToKill, quests[i].GetItemID());
            }
        } 
    }

    public void AddQuestToList(Quest quest)
    {
        quests.Add(quest);
    }

    [PunRPC]
    public void ReceiveQuestFromMaster(string mapTypeDataID, int questID, int questTypeID, string[] enemiesToKillIDs, int enemyAmountToKill, string itemToFindID)
    {
        quests.Add(new Quest(mapTypeDataID, questID, questTypeID, enemiesToKillIDs, enemyAmountToKill, itemToFindID));

        if(quests.Count == questAmount)
        {
            questUIController.SetQuestContainers(quests);
        }
    }

    [PunRPC]
    public static void VoteOnQuest(Quest quest)
    {
        selectedQuest = quest;

        Debug.Log("Voted for quest " + quest.questID);

        questIDVotes.Add(quest.questID);
    }

    public void BeginQuest()
    {
        if(PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if(questIDVotes.Count > 0)
        {
            int[] votes = new int[questAmount];

            for (int i = 0; i < questIDVotes.Count; i++)
            {
                votes[questIDVotes[i]]++;
            }

            int voteResult = votes[0];
            for (int i = 1; i < votes.Length; i++)
            {
                if (votes[i] > voteResult)
                {
                    voteResult = votes[i];
                }
            }

            QuestTracker.SetQuest(quests[voteResult]);
            if(PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("SetSelectedQuestOverNetwork", RpcTarget.Others, voteResult);
            }
            Debug.Log("Quest Selected");
            NetworkSceneManager.Instance.NetworkLoadSceneAsync(2);
        }
        else 
        {
            Debug.Log("No Votes Were cast");
        }
    }

    [PunRPC]
    public void SetSelectedQuestOverNetwork(int questID)
    {
        QuestTracker.SetQuest(quests[questID]);
    }

    public void Interact()
    {
        GuildUIController.ToggleQuestUI();
    }
}