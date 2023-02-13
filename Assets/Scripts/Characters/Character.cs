using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoardGame;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(PhotonView))]
public class Character : MonoBehaviour
{
    public CharacterData charData;

    public PhotonView pView;

    [SerializeField]
    protected Transform visualsObject;

    [SerializeField]
    protected GameObject skillPrefab;

    [SerializeField]
    protected Transform skillObjectSpawnPos;

    [HideInInspector]
    public int maxHealth = 10;
    [HideInInspector]
    public int curHealth;

    [HideInInspector]
    public int AC = 10;

    public Stat Vitality;
    public Stat CombatEffectiveness;
    public Stat Strength;
    public Stat Dexterity;
    public Stat Intelligence;
    public Stat Speed;
    public Stat Perception;

    protected Tile tileUnitIsOn;

    //[HideInInspector]
    //public bool isItThisUnitsCurrentTurn = false;
    [HideInInspector]
    public int currentMovementAvaliable;

    public Weapon weapon;
    [HideInInspector]
    public bool hasAction;

    public List<DamageType> vulnerabilties;
    public List<DamageType> resistances;

    public List<Status> currentStatuses;

    public delegate void OnDeath();
    public delegate void OnAttack();
    public delegate void OnMove();
    public delegate void OnTakeDamage();

    public OnDeath onDeath;
    public OnAttack onAttack;
    public OnMove onMove;
    public OnTakeDamage onTakeDamage;

    public bool isDead;

    protected CharacterAnimator charAnim;

    [SerializeField]
    private bool useAnimator = false;

    [SerializeField]
    protected bool moving = false;

    [SerializeField]
    protected float moveSpeed = 1;

    public List<SkillData> knownSkills;

    public int characterID;

    // Start is called before the first frame update
    void Awake()
    {
        if(useAnimator && charAnim == null)
            charAnim = gameObject.AddComponent<CharacterAnimator>();
        

        if (weapon != null)
        {
            weapon.Initialise();
        }
        else
        {
            weapon = new Weapon();
        }

        knownSkills = new List<SkillData>();
    }

    [Photon.Pun.PunRPC]
    public void SetPosition(Vector3 position)
    {
        Debug.Log(pView.ViewID + " " + name +" postition set to " + position);
        transform.position = position;
    }

    public void InitialiseStats()
    {
        if(useAnimator)
            charAnim.Initialise(this);

        Vitality = new Stat();
        CombatEffectiveness = new Stat();
        Strength = new Stat();
        Dexterity = new Stat();
        Intelligence = new Stat();
        Speed = new Stat();
        Perception = new Stat();

        Stat[] statsTemp = new Stat[] { Vitality, CombatEffectiveness, Strength, Dexterity, Intelligence, Speed, Perception };
        List<StatTypeData> statTypes = Lists.GetAllStatTypeDatas();

        for (int i = 0; i < statsTemp.Length; i++)
        {
            statsTemp[i].statTypeData = statTypes[i];
            //statsTemp[i].Initialise();
        }

        currentStatuses = new List<Status>();

        CombatEffectiveness.statDie = new Die(DieType.D20);
    }

    public int[] GetStatLevels()
    {
        int[] temp = new int[]{
        Vitality.curStatLevel,
        Strength.curStatLevel,
        Dexterity.curStatLevel,
        Intelligence.curStatLevel,
        CombatEffectiveness.curStatLevel,
        Perception.curStatLevel,
        Speed.curStatLevel
        };

        return temp;
    }

    public void SetAC(int ac)
    {
        AC = ac;
    }

    public virtual void StartTurn()
    {
        currentMovementAvaliable = GetMovementRange();
        //isItThisUnitsCurrentTurn = true;
    }

    public int GetInitiative()
    {
        int value = Speed.statDie.RollDie();
        
        return value;
    }

    public void CheckEndOfTurnEffect()
    {
        
    }

    public void Heal(int value)
    {
        curHealth += value;

        if(curHealth > maxHealth)
        {
            curHealth = maxHealth;
        }
    }

    protected int GetMovementRange()
    {
        int moveRange = Speed.statDie.RollDie();

        for (int i = 0; i < currentStatuses.Count; i++)
        {
            if(currentStatuses[i].statusEffector == Status.StatusEffector.MovementModifier)
            {
                Die tempDie = new Die(currentStatuses[i].dieModifier);
                moveRange += currentStatuses[i].modifier + tempDie.RollDie();
            }
        }

        return moveRange;
    }

    public virtual void EndTurn()
    {
        for (int i = 0; i < currentStatuses.Count; i++)
        {
            if(currentStatuses[i].statusEffector == Status.StatusEffector.EndOfTurnEffect)
            {
                currentStatuses[i].OnEndTurn();
            }
        }
        UnHighlightAnyTiles();
        //isItThisUnitsCurrentTurn = false;
    }

    public virtual void StartAttack(SkillData skillData)
    {
        
    }

    protected bool CheckCanUseAction()
    {
        bool canUseAction = false;
        
        if(hasAction)
        {
            for (int i = 0; i < currentStatuses.Count; i++)
            {
                if(currentStatuses[i].statusEffector == Status.StatusEffector.Action)
                {
                    break;
                }

                if(i == currentStatuses.Count)
                {
                    canUseAction = true;
                }
            }

            if(currentStatuses.Count == 0)
            {
                canUseAction = true;
            }
        }

        return canUseAction;
    }

    public bool RollToHit(Character unitToHit)
    {
        int attackValue = CombatEffectiveness.statDie.RollDie() + CombatEffectiveness.modifier;

        for (int i = 0; i < currentStatuses.Count; i++)
        {
            if(currentStatuses[i].statusEffector == Status.StatusEffector.AttackHit)
            {
                attackValue += currentStatuses[i].modifier + new Die(currentStatuses[i].dieModifier).RollDie();
            }
        }

        Debug.Log("Attack roll result: " + attackValue);

        if (attackValue >= unitToHit.AC)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int RollDamage(SkillData skillData)
    {
        
        if(skillData.useModifer)
        {
            Stat statToRollWith = null;

            switch (skillData.modifierType)
            {
                case ModifierType.Strength:
                    {
                        statToRollWith = Strength;
                        break;
                    }
                case ModifierType.Dexterity:
                    {
                        statToRollWith = Dexterity;
                        break;
                    }
                case ModifierType.Intelligence:
                    {
                        statToRollWith = Intelligence;
                        break;
                    }
            }

            return statToRollWith.statDie.RollDie() + statToRollWith.modifier;
        }

        return skillData.die.RollDie();
    }

    public virtual void StopAttacking()
    {
        
    }

    public virtual void UseAction()
    {
        hasAction = false;
    }

    private void ShowMovementRange()
    {

    }

    public void TakeDamage(DamageData damage)
    {
        if(useAnimator)
            onTakeDamage();

        Debug.Log(name + " Has Taken " + damage.amount.ToString() + " Damage");

        if(CheckDamageVulnerbilities(damage.type))
        {
            damage.amount *= 2;
        }

        //TO DO: Create floating damage numbers here using damage.amount
        UIController.Instance.CreateDamageNumbers(this, damage.amount);

        curHealth -= damage.amount;
        if(curHealth <= 0)
        {
            Die();
        }
    }

    private bool CheckDamageResistances(DamageType damageType)
    {
        bool resistant = false;

        for (int i = 0; i < resistances.Count; i++)
        {
            if(resistances[i] == damageType)
            {
                resistant = true;
                break;
            }
        }

        return resistant;
    }

    private bool CheckDamageVulnerbilities(DamageType damageType)
    {
        bool vulnerable = false;

        for (int i = 0; i < vulnerabilties.Count; i++)
        {
            if (resistances[i] == damageType)
            {
                vulnerable = true;
                break;
            }
        }

        return vulnerable;
    }

    protected virtual void Die()
    {
        isDead = true;
        onDeath();

        Debug.Log(name + " has died");

        TurnManager.RemoveUnitFromInitiative(this);
    }

    public virtual void UnHighlightAnyTiles()
    {

    }

    [PunRPC]
    public virtual void SetUnitToTile(Vector3 coord)
    {
        tileUnitIsOn = HexTileMap.GetTileAtCoord(coord);
        Debug.Log("Character Tile coords set to " + coord);
    }

    public Tile GetTileUnitIsOn()
    {
        return tileUnitIsOn;
    }

    public virtual IEnumerator MoveBetweenTiles(List<Tile> tiles)
    {
        yield return null;
    }

    public void SetWeapon(Weapon weapon)
    {
        this.weapon = weapon;
    }
}