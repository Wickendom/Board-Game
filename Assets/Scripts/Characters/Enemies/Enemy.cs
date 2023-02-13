using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoardGame;
using Photon.Pun;

public class Enemy : Character
{
    public EnemyData data;

    bool isActive;

    EnemyState state;
    EnemyBehaviour behaviour;
    EnemyActions actions;

    public Die behaviourDie = new Die();
    public int behaviourDieResult;

    public void Awake()
    {
        pView = GetComponent<PhotonView>();
    }

    [PunRPC]
    public void Initialise(string enemyData,int characterID)
    {
        Debug.Log("Enemy Initialising");

        pView = GetComponent<PhotonView>();

        data = Lists.GetEnemyData(enemyData);

        Instantiate(data.prefab, visualsObject);
        this.characterID = characterID;
        GameManager.AddCharacterToList(this);

        if (charAnim == null)
            charAnim = gameObject.AddComponent<CharacterAnimator>();

        InitialiseStats();

        curHealth = maxHealth;
        
        state = new EnemyState();
        actions = new EnemyActions(this);
        behaviour = new EnemyBehaviour(data,actions,this);
        behaviourDie.dieType = DieType.D20;
        HideVisuals();
        knownSkills = data.knownSkills;

        charData = data;

        Vitality.SetStatLevel(data.baseVitalityLevel);
        CombatEffectiveness.SetStatLevel(data.baseCombatEffectivenessLevel);
        Strength.SetStatLevel(data.baseStrengthLevel);
        Dexterity.SetStatLevel(data.baseDexterityLevel);
        Intelligence.SetStatLevel(data.baseIntelligenceLevel);
        Speed.SetStatLevel(data.baseSpeedLevel);
        Perception.SetStatLevel(data.basePerceptionLevel);

        for (int i = 0; i < knownSkills.Count; i++)
        {
            knownSkills[i].Initialise();
        }
        //Activate();
    }

    public void Activate()
    {
        if(!isActive && !isDead)
        {
            if(PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
                TurnManager.pView.RPC("AddEnemyToInitiative", RpcTarget.All,pView.ViewID);
            isActive = true;
            ShowVisuals();
            Debug.Log("Enemy Activated");
        }
    }

    public void HideVisuals()
    {
        visualsObject.gameObject.SetActive(false);
    }

    public void ShowVisuals()
    {
        visualsObject.gameObject.SetActive(true);
    }

    public override void StartTurn()
    {
        hasAction = true;
        base.StartTurn();
        behaviourDieResult = behaviourDie.RollDie();
        behaviour.SelectBehaviour(behaviourDieResult);
        pView.RPC("NetworkStartTurn", RpcTarget.Others, currentMovementAvaliable, behaviourDieResult);
    }

    [PunRPC]
    public void NetworkStartTurn(int movement, int behaviourResult)
    {
        Debug.Log("Network Turn Started for enemy");
        /*hasAction = true;
        currentMovementAvaliable = movement;
        behaviour.SelectBehaviour(behaviourResult);*/
    }

    [Photon.Pun.PunRPC]
    public void MoveToTile(Vector3 coords)
    {
        List<Tile> pathfind = new AStarSearch(GetTileUnitIsOn().GetTileAxialLocation(), coords, false, false).FindPath();

        actions.Move(pathfind);
    }

    [PunRPC]
    public override void EndTurn()
    {
        //isItThisUnitsCurrentTurn = false;
        TurnManager.Instance.EndEnemiesTurn();
    }

    public override void UseAction()
    {
        hasAction = false;
        pView.RPC("EndTurn",RpcTarget.All);
    }

    public int RollMovement()
    {
        int result = Speed.RollDie(true);
        //Debug.Log("Enemy Movement Rolled a " + result);
        return result;
    }

    protected override void Die()
    {
        base.Die();

        isActive = false;
        tileUnitIsOn.RemoveEnemyFromTile(this);
        //Destroy(gameObject);
    }

    private void Death()
    {
        QuestTracker.CheckEnemyDeathForQuest(data);
        GameManager.GivePlayersExp(data.expGivenOnDeath);

        onDeath -= Death;
    }

    private void OnEnable()
    {
        onDeath += Death;
    }

    private void OnDisable()
    {
        onDeath -= Death;
    }

    [PunRPC]
    public override void SetUnitToTile(Vector3 coord)
    {
        Tile tile = HexTileMap.GetTileAtCoord(coord);
        base.SetUnitToTile(coord);

        tile.AddEnemyToTile(this);
    }

    public override IEnumerator MoveBetweenTiles(List<Tile> tiles)
    {
        moving = true;
        //ableToMove = false;
        //ClearHiglightedMovementTiles();
        charAnim.Move();

        for (int i = 0; i < tiles.Count; i++)
        {
            transform.LookAt(tiles[i].tileData.GetMovePosition().position);

            while (Vector3.Distance(transform.position, tiles[i].tileData.GetMovePosition().position) > 0.01)
            {
                transform.position = Vector3.MoveTowards(transform.position, tiles[i].tileData.GetMovePosition().position, moveSpeed * Time.deltaTime);
                yield return null;
            }

            tiles[i].tileData.SetMovePosition(this);
            tileUnitIsOn.tileData.RemoveUnitFromMovePosition(this);

            tileUnitIsOn.RemoveEnemyFromTile(this);
            tileUnitIsOn = tiles[i];
            tileUnitIsOn.AddEnemyToTile(this);

            //FogOfWar.UpdateViewableTiles();
            //Debug.Log(name + " has moved to tile " + tileUnitIsOn.q + " " + tileUnitIsOn.r + " " + tileUnitIsOn.s);
        }

        currentMovementAvaliable -= tiles.Count;
        //ShowMovementRange();
        charAnim.StopMoving();
        moving = false;
        if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
        {
            actions.actionStorage();
            UseAction();
        }
        
        //ableToMove = true;
    }

    public SkillData GetRandomSkill()
    {
        return knownSkills[Random.Range(0, knownSkills.Count)];
    }

    [PunRPC]
    public void NetworkedAttack(int skillIndexToUse, int actorIDToAttack, bool attackHit, int attackDamage)
    {
        Debug.Log("Networked attack");
        actions.NetworkAttack(GameManager.GetCharacterByCharacterID(actorIDToAttack), skillIndexToUse, attackHit, attackDamage);
    }

    public SkillData GetRandomRangedSkill()
    {
        List<SkillData> rangedSkills = new List<SkillData>();

        for (int i = 0; i < knownSkills.Count; i++)
        {
            if(knownSkills[i].range > 1)
            {
                rangedSkills.Add(knownSkills[i]);
            }
        }

        return rangedSkills[Random.Range(0, rangedSkills.Count)];
    }
}
