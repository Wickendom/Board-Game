using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoardGame;

public class Stat
{
    public StatTypeData statTypeData;

    public int curStatLevel = 1;
    private int maxStatLevel = 20;

    int currentStatMilestone = 0;

    public Die statDie;

    public int modifier;

    public Stat()
    {
        statDie = new Die();

        curStatLevel = 1;
        //modifier = statTypeData.statModifier[0];
        //statDie.dieType = statTypeData.statDice[0];
    }

    public Stat(DieType die)
    {
        statDie = new Die(die);

        curStatLevel = 1;
        //modifier = statTypeData.statModifier[0];
        //statDie.dieType = statTypeData.statDice[0];
    }

    public void Initialise()
    {
        statDie = new Die();

        curStatLevel = 1;
        modifier = statTypeData.statModifier[0];
        statDie.dieType = statTypeData.statDice[0];
    }

    public int RollDie(bool addModifier)
    {
        int result = statDie.RollDie();

        if (addModifier)
            result += modifier;

        return result;
    }

    public int RollDie(DieType die,bool addModifier)
    {
        Die dice = new Die(die);

        int result = dice.RollDie();

        if (addModifier)
            result += modifier;

        return result;
    }

    public void IncreaseStatLevel()
    {
        curStatLevel++;

        int tempMilestoneIndex = 0;
        for (int i = currentStatMilestone; i < statTypeData.levelRequirement.Count; i++)
        {
            if(curStatLevel >= statTypeData.levelRequirement[i])
            {
                tempMilestoneIndex = i;
            }
            else
            {
                break;
            }
        }
        currentStatMilestone = tempMilestoneIndex;

        statDie.dieType = statTypeData.statDice[currentStatMilestone];
    }

    public void SetStatLevel(int level)
    {
        curStatLevel = level;

        int tempMilestoneIndex = 0;
        for (int i = currentStatMilestone; i < statTypeData.levelRequirement.Count; i++)
        {
            if (curStatLevel >= statTypeData.levelRequirement[i])
            {
                tempMilestoneIndex = i;
            }
            else
            {
                break;
            }
        }
        currentStatMilestone = tempMilestoneIndex;

        statDie.dieType = statTypeData.statDice[currentStatMilestone];
        modifier = statTypeData.statModifier[currentStatMilestone];
    }
}

public struct StatData
{
    public int statLevel;
}