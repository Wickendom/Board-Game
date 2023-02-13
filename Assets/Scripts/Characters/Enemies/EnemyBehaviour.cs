using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour
{
    private Enemy enemy;
    private EnemyData data;
    private EnemyActions actions;

    public enum Behaviour {Hidden, Engaged, Far, Close, Other }
    private Behaviour currentBehaviour = Behaviour.Engaged;

    public EnemyBehaviour(EnemyData data, EnemyActions actions, Enemy enemy)
    {
        this.data = data;
        this.actions = actions;
        this.enemy = enemy;
    }

    public void SelectBehaviour(int behaviorRollResult)
    {
        bool playerInSight = false;
        for (int i = 0; i < TurnManager.playerCharacters.Count; i++)
        {
            if(MapControls.CheckLineOfSight(enemy.GetTileUnitIsOn(),TurnManager.playerCharacters[i].GetTileUnitIsOn()))
            {
                playerInSight = true;
            }
        }

        if(!playerInSight)
        {
            currentBehaviour = Behaviour.Hidden;
        }
        else if(actions.CheckIfPlayerIsInMeleeRange())
        {
            currentBehaviour = Behaviour.Engaged;
        }
        else if(enemy.GetTileUnitIsOn().Distance(MapControls.GetClosestVisiblePlayer(enemy.GetTileUnitIsOn()).GetTileUnitIsOn()) <= 3)
        {
            currentBehaviour = Behaviour.Close;
        }
        else if(enemy.GetTileUnitIsOn().Distance(MapControls.GetClosestVisiblePlayer(enemy.GetTileUnitIsOn()).GetTileUnitIsOn()) <= 6)
        {
            currentBehaviour = Behaviour.Far;
        }
        else
        {
            currentBehaviour = Behaviour.Other;
        }

        Debug.Log("Taking " + currentBehaviour +" Action");

        EnemyActions.Action action = data.GetBehaviorAction(currentBehaviour, behaviorRollResult);

        Debug.Log(action);

        actions.TakeCurrentAction(enemy, action);
        //Debug.Log("Current Behaviour for " + data.enemyName + " is currently only being set to Engaged");
    }
}
