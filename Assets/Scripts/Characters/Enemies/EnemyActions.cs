using BoardGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActions
{
    public Enemy enemy;
    public enum Action {Advance,Aim,Charge,FallBack,Hold,Onslaught,Sneak};

    public SkillData skillToUse;
    public delegate void actionToUse();
    public actionToUse actionStorage;
    public EnemyActions(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void TakeCurrentAction(Enemy unit, Action action)
    {
        switch(action)
        {
            case Action.Advance:
                {
                    actionStorage = Attack;
                    Advance();
                    break;
                }
            case Action.Aim:
                {
                    Aim();
                    break;
                }
            case Action.Charge:
                {
                    Charge();
                    break;
                }
            case Action.FallBack:
                {
                    FallBack();
                    break;
                }
            case Action.Hold:
                {
                    Hold();
                    break;
                }
            case Action.Onslaught:
                {
                    Onslaught();
                    break;
                }
            case Action.Sneak:
                {
                    Sneak();
                    break;
                }
        }
    }

    public void Hold()
    {
        Debug.Log("Taking Hold Action");
        enemy.UseAction();
    }

    public void FallBack()
    {
        Debug.Log("Taking Fallback Action");
    }

    public void Aim()
    {
        Debug.Log("Taking Aim Action");
        if(!AttackFurthestPlayer())
        {
            Hold();
        }
        enemy.UseAction();
    }

    public void Advance()
    {
        Debug.Log("Taking Advance Action");
        actionStorage = Attack;
        MoveToClosestPlayer();
        //Attack();
    }

    public void Charge()
    {
        Debug.Log("Taking Charge Action");
        MoveToClosestPlayer();
        
        if(CheckIfPlayerIsInMeleeRange())
        {
            actionStorage = Attack;
        }
        else
        {
            actionStorage = MoveToClosestPlayer;
        }
        actionStorage();
    }

    public void Onslaught()
    {
        Debug.Log("Taking Onslaught Action");
        SkillData skillData = enemy.GetRandomSkill();

        if(skillData.range == 1)
        {
            MoveToClosestPlayer();
        }

        if (CheckIfPlayerIsInMeleeRange())
        {
            actionStorage = Attack;
        }
        else
        {
            actionStorage = MoveToClosestPlayer;
        }

        //AttackClosestPlayerInRange(skillData);
        //
        //skillData = enemy.GetRandomSkill();
        //AttackClosestPlayerInRange(skillData);
        enemy.UseAction();
    }

    public void Sneak()
    {
        Debug.Log("Taking Sneak Action");
    }

    public void Move(Tile tileEnemyIsOn,Tile destination, int movementRange)
    {
        //enemy.onMove();

        List<Tile> tiles = MapControls.Pathfind(tileEnemyIsOn, destination);
        
        if(tiles.Count != 0)
        {
            int tileIndex = (tiles.Count > movementRange) ? movementRange : (tiles.Count);
            if (tiles[tileIndex - 1].GetPlayersOnTile().Count > 0 && tiles.Count != 1)
            {
                tileIndex--;
                tiles.RemoveAt(tiles.Count - 1);
            }

            enemy.StartCoroutine(enemy.MoveBetweenTiles(tiles));
        }
        else
        {
            actionStorage();
            enemy.UseAction();
            Debug.LogWarning("Enemy was not able to reach its goal");
        }
        
    }

    public void Move(List<Tile> path, int movementRange)
    {
        //enemy.onMove();

        if (path.Count != 0)
        {
            int tileIndex = (path.Count > movementRange) ? movementRange : (path.Count);
            if (path[tileIndex - 1].GetPlayersOnTile().Count > 0 && path.Count != 1)
            {
                tileIndex--;
               // path.RemoveAt(path.Count - 1);
            }
            //enemy.pView.RPC("MoveToTile",Photon.Pun.RpcTarget.Others,path[tileIndex].coords);
            enemy.StartCoroutine(enemy.MoveBetweenTiles(path));
        }
        else
        {
            actionStorage();
            enemy.UseAction();
            Debug.LogWarning("Enemy was not able to reach its goal");
        }

    }

    public void Move(List<Tile> path)
    {
        if (path.Count != 0)
        {
            int tileIndex = path.Count;
            if (path[tileIndex - 1].GetPlayersOnTile().Count > 0 && path.Count != 1)
            {
                tileIndex--;
                // path.RemoveAt(path.Count - 1);
            }

            enemy.StartCoroutine(enemy.MoveBetweenTiles(path));
        }
    }

    public void MoveToClosestPlayer()
    {
        int currentClosestDistance = 0;
        int characterIndex = 0;
        bool characterIndexChanged = false;

        int moveDistance = enemy.RollMovement();

        //Debug.Log("Enemy moving to closest player");

        List<Tile> chosenPath = new List<Tile>();

        for (int i = 0; i < TurnManager.playerCharacters.Count; i++)
        {
            List<Tile> pathfind = new AStarSearch(enemy.GetTileUnitIsOn().GetTileAxialLocation(), TurnManager.playerCharacters[i].GetTileUnitIsOn().GetTileAxialLocation(), false, false).FindPath();
            if(pathfind.Count != 0)
                pathfind.RemoveAt(pathfind.Count - 1);

            if(pathfind.Count > moveDistance)
            {
                pathfind.RemoveRange(moveDistance, pathfind.Count - moveDistance);
            }

            if(pathfind.Count != 0)
            {
                if (i == 0)
                {
                    currentClosestDistance = pathfind.Count;
                    characterIndexChanged = true;
                    chosenPath = pathfind;
                }
                else
                {
                    if (currentClosestDistance > pathfind.Count)
                    {
                        currentClosestDistance = pathfind.Count;
                        characterIndex = i;
                        characterIndexChanged = true;
                        chosenPath = pathfind;
                    }
                }
            }
        }

        if(characterIndexChanged)
        {
            Move(chosenPath, enemy.currentMovementAvaliable);
        }
        else
        {
            Debug.LogWarning("Enemy was not able to reach its goal");
            actionStorage();
            enemy.UseAction();
        }
    }

    public void MoveToHiddenTile()
    {

    }

    private void Attack()
    {
        //enemy.onAttack();

        skillToUse = enemy.GetRandomSkill();

        if (skillToUse.range == 1)
        {
            MeleeAttackPlayer();
        }
        else
        {
            Character character = MapControls.GetRandomPlayerInRange(enemy.GetTileUnitIsOn(), skillToUse.range);
            if(character != null)
                RangedAttackPlayer(character);
        }
    }

    public void Attack(int skillIndex, int characterID)// this is used for non master clients to sync skills used
    {
        skillToUse = enemy.knownSkills[skillIndex];

        if (skillToUse.range == 1)
        {
            MeleeAttackPlayer();
        }
        else
        {
            Character character = MapControls.GetRandomPlayerInRange(enemy.GetTileUnitIsOn(), skillToUse.range);
            if (character != null)
                RangedAttackPlayer(character);
        }
    }

    private void Attack(SkillData skillData, Character chara)
    {
        //enemy.onAttack();
        skillToUse = skillData;

        if (skillToUse.range == 1)
        {
            MeleeAttackPlayer();
        }
        else
        {
            Character character = MapControls.GetRandomPlayerInRange(enemy.GetTileUnitIsOn(), skillToUse.range);
            if (character != null)
                RangedAttackPlayer(chara);
        }
    }

    private int DealDamage(Character characterToAttack)
    {
        int damageAmount = skillToUse.UseSkill(characterToAttack.transform.position,characterToAttack,enemy.transform,true);
        //characterToAttack.TakeDamage(new DamageData(enemy.RollDamage(skillToUse), skillToUse.damageType));
        return damageAmount;
    }

    public void NetworkAttack(Character characterToAttack, int skillID, bool attackHit, int damageAmount)
    {
        skillToUse = enemy.knownSkills[skillID];

        skillToUse.UseSkill(characterToAttack.transform.position, characterToAttack, enemy.transform, attackHit,damageAmount);
    }

    public void AttackClosestPlayerInRange()
    {
        SkillData skillData = enemy.GetRandomSkill();

        Attack(skillData, MapControls.GetClosestPlayerInRange(enemy.GetTileUnitIsOn(),skillData.range));
    }

    public void AttackClosestPlayerInRange(SkillData skillData)
    {
        Attack(skillData, MapControls.GetClosestPlayerInRange(enemy.GetTileUnitIsOn(), skillData.range));
    }

    public bool AttackFurthestPlayer()
    {
        SkillData skillData = enemy.GetRandomRangedSkill();
        
        if(skillData == null)
        {
            Debug.Log(enemy.name + " Has no ranged abilites to use");
            return false;
        }

        Character chara = MapControls.GetFurthestPlayerInRange(enemy.GetTileUnitIsOn(), skillData.range);
        if (chara == null)
            return false;
        
        RangedAttackPlayer(chara);
        return true;
    }

    public bool CheckIfPlayerIsInMeleeRange()
    {
        List<Tile> neighboringTiles = enemy.GetTileUnitIsOn().Neighbors(false);

        for (int i = 0; i < neighboringTiles.Count; i++)
        {
            if (neighboringTiles[i].GetPlayersOnTile().Count > 0)
            {
                Debug.Log("Player Found in Melee Range");
                return true;
            }
        }

        return false;
    }

    public void MeleeAttackPlayer()
    {
        Character characterToAttack = SelectPlayerWithinMeleeRange();
        Debug.Log(characterToAttack.name);
        enemy.transform.LookAt(characterToAttack.transform);

        bool attackHits = CheckAttackHits(characterToAttack);
        int damageAmount = 0;

        if (attackHits)
        {
            damageAmount = DealDamage(characterToAttack);
        }

        enemy.pView.RPC("NetworkedAttack", Photon.Pun.RpcTarget.Others, enemy.knownSkills.IndexOf(skillToUse), characterToAttack.characterID, attackHits, damageAmount);
    }

    public Character SelectPlayerWithinMeleeRange()
    {
        List<Tile> neighboringTiles = enemy.GetTileUnitIsOn().Neighbors(false);
        List<Character> charactersInNeighboringTiles = new List<Character>();

        for (int i = 0; i < neighboringTiles.Count; i++)
        {
            if (neighboringTiles[i].GetPlayersOnTile().Count > 0)
            {
                Debug.Log("Player Found in Melee Range");
                charactersInNeighboringTiles.Add(neighboringTiles[i].GetPlayersOnTile()[0]);
            }
        }

        Character chara = null;

        if (charactersInNeighboringTiles.Count > 0)
        {
            int randomCharacterIndexInRange = Random.Range(0, charactersInNeighboringTiles.Count);
            chara = charactersInNeighboringTiles[randomCharacterIndexInRange];
        }
        else
        {
            Debug.Log("No Players Found");
        }

        return chara;
    }

    public void RangedAttackPlayer(Character characterToAttack)
    {
        enemy.transform.LookAt(characterToAttack.transform);
        bool attackHits = CheckAttackHits(characterToAttack);
        int damageAmount = 0;

        if (attackHits)
        {
            damageAmount = DealDamage(characterToAttack);
        }

        enemy.pView.RPC("NetworkedAttack", Photon.Pun.RpcTarget.Others, enemy.knownSkills.IndexOf(skillToUse), characterToAttack.characterID, attackHits, damageAmount);
    }

    private bool CheckAttackHits(Character characterToAttack)
    {
        if (enemy.RollToHit(characterToAttack))
        {
            Debug.Log(characterToAttack.name + " was hit");
            return true;
        }
        else
        {
            Debug.Log("Enemy missed " + characterToAttack.name);
            return false;
        }
    }
}
