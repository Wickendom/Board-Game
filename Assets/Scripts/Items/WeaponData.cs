using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoardGame;

[CreateAssetMenu(fileName = "Weapon Data", menuName = "Data's/WeaponData")]
public class WeaponData : ItemData
{
    public int damage = 5;

    public Weapon.AttackType attackType = Weapon.AttackType.Melee;

    public DamageType damageType;

    [Tooltip("This only needs changing if this weapon is a ranged weapon")]
    public int weaponRange = 1;

    public Sprite sprite;
    public GameObject weaponPrefab;
}