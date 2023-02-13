using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace BoardGame
{
    public class Player : Character
    {
        [HideInInspector]
        public int playerID;

        [HideInInspector]
        public PlayerClass playerClass;

        public LayerMask tileLayerMask;

        private bool unitAttaking = false;
        [SerializeField]
        private bool ableToMove = true;

        List<Tile> currentMovementTiles;

        [HideInInspector]
        public int playerLevel;
        [HideInInspector]
        public int expNeededForNextLevel;
        [HideInInspector]
        public int currExp;
        [HideInInspector]
        public int currSkillPoints = 0;
        public SkillManager skillManager;

        private void Start()
        {
            pView = GetComponent<Photon.Pun.PhotonView>();
            if(Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber != pView.Owner.ActorNumber)
            {
                Debug.Log(PlayersManager.GetPlayerNetworkData(pView.Owner.ActorNumber).playerData.playerID);
                InitialisePlayerFromData(PlayersManager.GetPlayerNetworkData(pView.Owner.ActorNumber).playerData);
            }
        }

        [PunRPC]
        public void PreInitialise()
        {
            pView = GetComponent<PhotonView>();
        }

        [PunRPC]

        public void InitialisePlayerFromData(PlayerData playerData)
        {
            pView = GetComponent<Photon.Pun.PhotonView>();

            playerClass = playerData.playerClass;
            Instantiate(playerClass.modelPrefab, visualsObject);

            InitialiseStats();

            playerID = playerData.playerID;
            
            Vitality = playerData.Vitality;
            CombatEffectiveness = playerData.CombatEffectiveness;
            Strength = playerData.Strength;
            Dexterity = playerData.Dexterity;
            Intelligence = playerData.Intelligence;
            Speed = playerData.Speed;
            Perception = playerData.Perception;

            playerLevel = playerData.level;
            currExp = playerData.exp;
            currSkillPoints = playerData.skillPoints;

            curHealth = maxHealth;

            TurnManager.playerCharacters.Add(this);
            if(pView.IsMine)
            TurnManager.pView.RPC("AddUnitToInitiative", RpcTarget.All, pView.ViewID);
            skillManager = new SkillManager(this);
            knownSkills = playerData.knownSkills;

            charData = playerClass;

            if(pView.IsMine)
            pView.RPC("AddPlayerToGameManager", Photon.Pun.RpcTarget.All);

            UIController.Instance.Initialise();
            

            NetworkReadyCheckerContainer.pView.RPC("SetPlayerCreated", Photon.Pun.RpcTarget.MasterClient, Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber);
        }

        [Photon.Pun.PunRPC]
        public void AddPlayerToGameManager()
        {
            GameManager.allPlayers.Add(this);
        }

        [Photon.Pun.PunRPC]
        public void InitialiseAfterAllPlayersCreated()
        {
            Debug.Log("player " + pView.ViewID + " secondary initialised");
            SetTile(HexTileMap.playerTileSpawnPoint);

            Vector3 pos = HexTileMap.GetTileAtCoord(HexTileMap.playerTileSpawnPoint).tileData.GetMovePosition().position;
            pos.y = 0;
            SetPosition(pos);

            FogOfWar.UpdateViewableTiles();
        }

        public void SetSkillManagerUI(SkillManagerUI skillManagerUI)
        {
            skillManager.skillManagerUI = skillManagerUI;
            skillManager.InitialiseUI(GetStatLevels());
        }

        private void Update()
        {
            if(pView.IsMine)
            {
                if (TurnManager.CheckIfItIsLocalPlayersTurn() && !moving)
                {
                    if (Input.GetMouseButtonDown(0) && ableToMove)
                    {
                        RaycastHit hit;
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        Vector3 pos = Vector3.zero;

                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileLayerMask))
                        {
                            if (!UIController.Instance.CheckIfHoveringOverUI())
                            {
                                MakeMove(hit.collider.GetComponent<TileData>().tile);
                            }
                        }
                    }
                }
            }
        }

        public override void StartTurn()
        {
            base.StartTurn();

            hasAction = true;
            ShowMovementRange();
            ableToMove = true;
        }

        public void MakeMove(Tile destination)
        {
            if(TurnManager.CheckIfItIsLocalPlayersTurn())
            {
                if(currentMovementAvaliable > 0)
                {
                    Debug.Log("Moving to Tile - q" + destination.q + " r" + destination.r + " s" + destination.s);
                    pView.RPC("MakeMoveToTileCoord", Photon.Pun.RpcTarget.Others, new Vector3(destination.q, destination.r, destination.s));
                    onMove();

                    List<Tile> tiles = MapControls.Pathfind(tileUnitIsOn, destination);
                    int tileIndex = 0;//(tiles.Count > currentMovementAvaliable) ? currentMovementAvaliable : (tiles.Count);// this limits how far the player can go if he has less movement than how far the tiles are.

                    if(tiles.Count > currentMovementAvaliable)
                    {
                        tileIndex = currentMovementAvaliable;
                        tiles.RemoveRange(tileIndex, tiles.Count - tileIndex);
                    }
                    else
                    {
                        tileIndex = tiles.Count;
                    }

                    ClearHiglightedMovementTiles();
                    StartCoroutine(MoveBetweenTiles(tiles));

                    //transform.localPosition = tileUnitIsOn.tileGO.transform.localPosition;
                }
            }
        }

        [Photon.Pun.PunRPC]
        public void MakeMoveToTileCoord(Vector3 destinationCoord)
        {
            Debug.Log("Network Found Move between tiles for enemy");
            Debug.Log("Moving to Tile - q" + destinationCoord.x + " r" + destinationCoord.y + " s" + destinationCoord.z);
            Tile destination = HexTileMap.GetTileAtCoord(destinationCoord);
            Debug.Log("destination tile coords = q" + destinationCoord.x + " r" + destinationCoord.y + " s" + destinationCoord.z);
            onMove();

            List<Tile> tiles = MapControls.Pathfind(tileUnitIsOn, destination);

            StartCoroutine(MoveBetweenTiles(tiles));

            //transform.localPosition = tileUnitIsOn.tileGO.transform.localPosition;            
        }

        public override IEnumerator MoveBetweenTiles(List<Tile> tiles)
        {
            Debug.Log("Moving Between Tiles");
            moving = true;
            ableToMove = false;

            for (int i = 0; i < tiles.Count; i++)
            {

                //Debug.Log("tile " + i + " " + tiles[i].q + " " + tiles[i].r + " " + tiles[i].s);

                transform.LookAt(tiles[i].tileGO.transform);

                while (Vector3.Distance(transform.position,tiles[i].tileGO.transform.position) > 0.01)
                {
                    transform.position = Vector3.MoveTowards(transform.position, tiles[i].tileGO.transform.position,moveSpeed * Time.deltaTime);
                    yield return null;
                }

                tileUnitIsOn.RemovePlayerFromTile(this);
                tileUnitIsOn = tiles[i];
                tileUnitIsOn.AddPlayerToTile(this);

                FogOfWar.UpdateViewableTiles();
                //Debug.Log(name + " has moved to tile " + tileUnitIsOn.q + " " + tileUnitIsOn.r + " " + tileUnitIsOn.s);
            }

            currentMovementAvaliable -= tiles.Count;
            ShowMovementRange();
            charAnim.StopMoving();
            moving = false;
            ableToMove = true;
        }

        [Photon.Pun.PunRPC]
        public void SetTile(Vector3 tileCoords)
        {
            tileUnitIsOn = HexTileMap.GetTileAtCoord(tileCoords);
        }

        public void AddExp(int amount)
        {
            currExp += amount;
            if (currExp >= expNeededForNextLevel)
                LevelUp();
        }

        private void LevelUp()
        {
            playerLevel++;
            currSkillPoints += 3;
            expNeededForNextLevel = (int)Mathf.Pow(5f,playerLevel);
        }

        public void ShowMovementRange()
        {
            //print("Showing Movement Range");
            if(currentMovementAvaliable > 0)
                currentMovementTiles = MapControls.HighlightTilesInRange(tileUnitIsOn, currentMovementAvaliable,Color.blue);
        }

        private void ClearHiglightedMovementTiles()
        {
            MapControls.ClearHighlightedTiles(currentMovementTiles);
            if(currentMovementTiles.Count > 0)
                currentMovementTiles.Clear();
        }

        public override void StartAttack(SkillData skillData)
        {
            if(CheckCanUseAction())
            {
                unitAttaking = true;
                ableToMove = false;
                ClearHiglightedMovementTiles();
                AttackManager.currentAttackingUnit = this;
                AttackManager.ShowAttackingRange(tileUnitIsOn, skillData, this);
            }
            else
            {
                Debug.Log("Player has no action left or is unable to use their action to attack with");       
            }
            
        }

        [PunRPC]
        public void NetworkAttack(int attackingActorID,int actorIDToAttack, int skillIDToUse, bool attackHit, int damageAmount)
        {
            AttackManager.NetworkDoAttack(attackingActorID, actorIDToAttack, knownSkills[skillIDToUse], attackHit, damageAmount);
        }

        public override void StopAttacking()
        {
            AttackManager.StopAttacking();
            unitAttaking = false;
            ShowMovementRange();
            StartCoroutine(EnableMoveNextFrame());
        }

        IEnumerator EnableMoveNextFrame()
        {
            yield return 0;

            ableToMove = true;
        }

        public override void UnHighlightAnyTiles()
        {
            ClearHiglightedMovementTiles();
            AttackManager.StopAttacking();
        }

        public override void SetUnitToTile(Vector3 coord)
        {
            Tile tile = HexTileMap.GetTileAtCoord(coord);
            base.SetUnitToTile(coord);

            tile.AddPlayerToTile(this);
        }

        public void UpdatePlayerData()
        {
            Debug.Log("PlayerData Saved");
            PlayersManager.UpdatePlayerData(this);
        }

        protected override void Die()
        {
            base.Die();

            ClearHiglightedMovementTiles();
            ableToMove = false;
        }
    }
}

