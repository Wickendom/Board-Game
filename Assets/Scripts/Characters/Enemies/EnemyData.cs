using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Data's/Characters/Enemies/EnemyData")]
public class EnemyData : CharacterData
{
    public string enemyName;

    public GameObject prefab;

    public readonly EnemyBehaviour.Behaviour Hidden = EnemyBehaviour.Behaviour.Hidden;
    public Vector2[] hiddenBehaviourResults;
    public EnemyActions.Action[] hiddenActions;

    public readonly EnemyBehaviour.Behaviour Engaged = EnemyBehaviour.Behaviour.Engaged;
    public Vector2[] engagedBehaviourResults;
    public EnemyActions.Action[] engagedActions;

    public readonly EnemyBehaviour.Behaviour Close = EnemyBehaviour.Behaviour.Close;
    public Vector2[] closeBehaviourResults;
    public EnemyActions.Action[] closeActions;

    public readonly EnemyBehaviour.Behaviour Other = EnemyBehaviour.Behaviour.Other;
    public Vector2[] otherBehaviourResults;
    public EnemyActions.Action[] otherActions;

    public bool enemyHasInCover;
    public readonly EnemyBehaviour.Behaviour Far = EnemyBehaviour.Behaviour.Far;
    public Vector2[] farBehaviourResults;
    public EnemyActions.Action[] farActions;

    public int expGivenOnDeath = 50;

    public List<SkillData> knownSkills;

    public EnemyActions.Action GetBehaviorAction(EnemyBehaviour.Behaviour behavior, int behaviourResult)
    {
        Vector2[] resultsToUse = null;
        EnemyActions.Action[] actionsToUse = null;
        switch(behavior)
        {
            case EnemyBehaviour.Behaviour.Hidden:
                {
                    resultsToUse = hiddenBehaviourResults;
                    actionsToUse = hiddenActions;
                    break;
                }
            case EnemyBehaviour.Behaviour.Engaged:
                {
                    resultsToUse = engagedBehaviourResults;
                    actionsToUse = engagedActions;
                    break;
                }
            case EnemyBehaviour.Behaviour.Close:
                {
                    resultsToUse = closeBehaviourResults;
                    actionsToUse = closeActions;
                    break;
                }
            case EnemyBehaviour.Behaviour.Other:
                {
                    resultsToUse = otherBehaviourResults;
                    actionsToUse = otherActions;
                    break;
                }
            case EnemyBehaviour.Behaviour.Far:
                {
                    resultsToUse = farBehaviourResults;
                    actionsToUse = farActions;
                    break;
                }
        }

        for (int i = 0; i < resultsToUse.Length; i++)
        {
            if(behaviourResult <= resultsToUse[i].y)
            {
                return actionsToUse[i];
            }
        }

        Debug.LogWarning("An action has not been selected");
        return EnemyActions.Action.Hold;
    }
}
