using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTileSkill : Skill
{
    public void Initialise(SkillData data, List<Character> charactersToHit, List<bool> hitList)
    {
        this.data = data;

        Vector3 spawnPos = charactersToHit[0].GetTileUnitIsOn().tileGO.transform.position;
        //spawnPos.y += yOffset;

        Instantiate(data.skillVFXPrefab, spawnPos, Quaternion.identity, transform);

        for (int i = 0; i < charactersToHit.Count; i++)
        {
            if (hitList[i])
            {
                charactersToHit[i].TakeDamage(new BoardGame.DamageData(data.die.RollDie(), data.damageType));
            }
        }
    }
}
