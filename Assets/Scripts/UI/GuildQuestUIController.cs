using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuildQuestUIController : MonoBehaviour
{
    [SerializeField]
    private List<QuestContainer> questContainers;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetQuestContainers(List<Quest> quests)
    {
        for (int i = 0; i < questContainers.Count; i++)
        {
            questContainers[i].Initialise(quests[i]);
        }
    }

}
