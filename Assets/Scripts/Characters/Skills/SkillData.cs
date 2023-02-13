using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoardGame;

namespace BoardGame
{
    public enum SkillTargetType { SingleTarget, SingleTile, MultipleTiles}
    public enum SkillTargetUnitType { Friendly,Enemy}
    public enum SkillObjectType { Projectile,OnUnit,OnTile,MultipleTile}
    public enum ModifierType { Strength, Dexterity, Intelligence}
}

[CreateAssetMenu(fileName = "Skill", menuName = "Data's/Skill")]
public class SkillData : ScriptableObject
{
    public string skillName = "Skill Name";

    public Sprite skillIcon;

    public GameObject skillVFXPrefab;
    
    public Die die;
    [Tooltip("This is the die that will be used to calculate the result of the spell. for example damage or healing")]
    public DieType dieToUse;

    public SkillTargetType skillTargetType = SkillTargetType.SingleTarget;
    public SkillTargetUnitType skillTargetUnitType = SkillTargetUnitType.Enemy;
    public SkillObjectType skillObjectType = SkillObjectType.Projectile;

    public int range = 3;

    //Modifier Related Stuff
    public bool useModifer = false;
    public ModifierType modifierType;

    /// Damage Related Stuff
    public DamageType damageType;

    /// PROJECTILE RELATED STUFF
    public GameObject projectileOnHitVfxPrefab;
    public float projectileMoveSpeed;


    private bool initialised;

    public void Initialise()
    {
        die = new Die(dieToUse);

        initialised = true;
    }

    public virtual void Targeting()
    {

    }

    public virtual void ShowAttackRange()
    {
        if (!initialised)
            Initialise();

        TurnManager.Instance.StartAttacking(this);
    }

    public int UseSkill(Vector3 skillSpawnPos, Character characterBeingAttacked, Transform spawningUnit, bool doesAbilityHit)
    {
        GameObject skillGameObject = new GameObject(skillName + " VFX");
        Vector3 spawnPos = new Vector3();

        if (!initialised)
            Initialise();

        int damage = die.RollDie();

        switch (skillObjectType)
        {
            case SkillObjectType.Projectile:
                {
                    spawnPos = spawningUnit.transform.position;
                    skillGameObject.transform.position = spawnPos;

                    Projectile proj = skillGameObject.AddComponent<Projectile>();
                    proj.Initialise(this, characterBeingAttacked, spawningUnit, doesAbilityHit, damage);
                    break;
                }
            case SkillObjectType.OnUnit:
                {
                    spawnPos = characterBeingAttacked.transform.position;
                    skillGameObject.transform.position = spawnPos;
                    skillGameObject.transform.SetParent(characterBeingAttacked.transform);

                    OnUnitSkill onUnit = skillGameObject.AddComponent<OnUnitSkill>();
                    onUnit.Initialise(this, characterBeingAttacked, doesAbilityHit,damage);
                    break;
                }
        }

        return damage;

    }

    public void UseSkill(Vector3 skillSpawnPos, Character characterBeingAttacked, Transform spawningUnit, bool doesAbilityHit, int damage)
    {
        GameObject skillGameObject = new GameObject(skillName + " VFX");
        Vector3 spawnPos = new Vector3();

        if (!initialised)
            Initialise();

        switch (skillObjectType)
        {
            case SkillObjectType.Projectile:
                {
                    spawnPos = spawningUnit.transform.position;
                    skillGameObject.transform.position = spawnPos;

                    Projectile proj = skillGameObject.AddComponent<Projectile>();
                    proj.Initialise(this, characterBeingAttacked, spawningUnit, doesAbilityHit, damage);
                    break;
                }
            case SkillObjectType.OnUnit:
                {
                    spawnPos = characterBeingAttacked.transform.position;
                    skillGameObject.transform.position = spawnPos;
                    skillGameObject.transform.SetParent(characterBeingAttacked.transform);

                    OnUnitSkill onUnit = skillGameObject.AddComponent<OnUnitSkill>();
                    onUnit.Initialise(this, characterBeingAttacked, doesAbilityHit, damage);
                    break;
                }
        }
    }

    public void UseSkill(Vector3 skillSpawnPos, List<Character> unitsHit, List<bool> doesAbilityHit)
    {
        GameObject skillGameObject = new GameObject();

        switch (skillObjectType)
        {
            case SkillObjectType.OnTile:
                {
                    OnTileSkill skill = skillGameObject.AddComponent<OnTileSkill>();
                    skill.Initialise(this,unitsHit,doesAbilityHit);
                    break;
                }
            case SkillObjectType.MultipleTile:
                {
                    break;
                }
        }
    }
}
