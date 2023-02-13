using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoardGame;

[CreateAssetMenu(fileName = "End Of Turn Effect", menuName = "Data's/Statuses/End Of Turn Status")]
public class EndOfTurnEffect : Status
{
    private enum EndTurnEffect {Heal,Damage}
    [SerializeField]
    private EndTurnEffect endTurnEffect;
    
    public DamageType damageType = DamageType.Fire;
    public override void OnEndTurn()
    {
        switch(endTurnEffect)
        {
            case EndTurnEffect.Heal:
                {
                    Heal();
                    break;
                }

            case EndTurnEffect.Damage:
                {
                    Damage();
                    break;
                }
        }
    }

    private void Heal()
    {
        unit.Heal(intValue);
    }

    private void Damage()
    {
        unit.TakeDamage(new DamageData(intValue,damageType));
    }
}
