using BoardGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingSkill : SkillData
{
    public int damage;

    public Status statusToInflict;

    public virtual void DealDamage()
    {

    }
}
