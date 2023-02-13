using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnUnitSkill : Skill
{
    public void Initialise(SkillData data, Character unitAffecting, bool doesAbilityHit,int damageAmount)
    {
        Debug.Log("Skill Initialised");

        this.data = data;

        unitToHit = unitAffecting;

        if(doesAbilityHit)
        {
            Vector3 spawnPos = unitAffecting.transform.position;
            spawnPos.y += yOffset;

            if(data.skillVFXPrefab)
                Instantiate(data.skillVFXPrefab, spawnPos, Quaternion.identity, transform);

            unitAffecting.TakeDamage(new BoardGame.DamageData(damageAmount,data.damageType));
        }
        
    }
}
