using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoardGame;

public class Weapon : Item, IEquipable
{
    public WeaponData data;

    public AttackData attackData;

    public enum AttackType { Melee, Ranged, Magical };
    public AttackType weaponRangeType = AttackType.Melee;

    public Weapon()
    {
        CreateRandomWeapon();
    }

    public void Initialise()
    {
        if(data)
        {
           attackData = new AttackData(data);
        }
        else
        {
            CreateRandomWeapon();
        }
    }

    public void CreateAttackData(WeaponData data)
    {
        attackData = new AttackData(data);
    }

    public void CreateRandomWeapon()
    {
        attackData = new AttackData();
    }

    public int GetDamage()
    {
        return data.damage;
    }

    public AttackData GetAttackData()
    {
        return attackData;
    }

    public void Equip()
    {
        throw new System.NotImplementedException();
    }
}

public class AttackData
{
    public readonly int damage;
    public readonly Weapon.AttackType weaponType;
    public readonly DamageType damageType;
    public readonly int range;

    public AttackData()
    {
        damage = Random.Range(2, 10);
        weaponType = (Weapon.AttackType)Random.Range(0, 2);
        damageType = (DamageType)Random.Range(0, 7);
        range = (weaponType == Weapon.AttackType.Melee) ? 1 : Random.Range(2, 10);
    }

    public AttackData(WeaponData data)
    {
        damage = data.damage;
        weaponType = data.attackType;
        range = data.weaponRange;
    }
}