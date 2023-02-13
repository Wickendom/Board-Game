using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Skill
{
    Transform projectilesTransform;
    Vector3 endPos;
    bool positionsSet;
    bool hits;//this is whether the projectile hits its target or not
    int damageAmount;

    public void Initialise(SkillData data, Character unitAffecting, Transform spawningUnit, bool doesAbilityHit,int damageAmount)
    {
        this.data = data;

        unitToHit = unitAffecting;

        Instantiate(data.skillVFXPrefab,new Vector3(transform.position.x,transform.position.y + yOffset, transform.position.z),Quaternion.identity,this.transform);

        endPos = unitAffecting.transform.position;
        endPos.y += yOffset;
        transform.LookAt(unitAffecting.transform);

        hits = doesAbilityHit;
        this.damageAmount = damageAmount;
        positionsSet = true;

        if(data.projectileMoveSpeed <= 0)
        {
            data.projectileMoveSpeed = 3;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(positionsSet)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPos, data.projectileMoveSpeed * Time.deltaTime);

            if(Vector3.Distance(transform.position, endPos) < 0.01f)
            {
                if(hits)
                {
                    unitToHit.TakeDamage(new BoardGame.DamageData(damageAmount, data.damageType));
                    Instantiate(data.projectileOnHitVfxPrefab, transform.position, transform.rotation);
                }

                Destroy(gameObject);
            }
        }
    }
}
