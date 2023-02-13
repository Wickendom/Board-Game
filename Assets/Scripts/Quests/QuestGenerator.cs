using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGenerator
{
    public QuestGenerator(QuestController questController,int questAmount)
    {
        for (int i = 0; i < questAmount; i++)
        {
            questController.AddQuestToList(new Quest(i));
        }
    }
}
