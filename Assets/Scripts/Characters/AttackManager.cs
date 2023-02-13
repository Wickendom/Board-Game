using BoardGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    private static List<Tile> currentHighlightedTiles;

    private static bool localUnitIsAttacking;

    [SerializeField]
    private LayerMask tileLayerMask;

    private static SkillData currentAttackData;
    public static Character currentAttackingUnit;

    private void Start()
    {
        currentHighlightedTiles = new List<Tile>();
    }

    private void Update()
    {
        if(localUnitIsAttacking)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 pos = Vector3.zero;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileLayerMask))
                {
                    if(!UIController.Instance.CheckIfHoveringOverUI())
                    {
                        Tile tile = hit.collider.GetComponent<TileData>().tile;
                        Debug.Log(tile.q + " " + tile.r + " " + tile.s);

                        //TO DO: Add check enemy is within attack range system. You can hit anyone on the map at the mo.
                        if (hit.collider.GetComponent<TileData>().tile.GetEnemiesOnTile().Count > 0)
                        {
                            Character characterToAttack = hit.collider.GetComponent<TileData>().tile.GetEnemiesOnTile()[0];
                            Debug.Log(characterToAttack.name + " has been found and is being attacked");
                            if(currentAttackData.skillObjectType == SkillObjectType.OnUnit || currentAttackData.skillObjectType == SkillObjectType.Projectile)
                            {
                                DoAttack(currentAttackData, characterToAttack);
                            }
                            else if(currentAttackData.skillObjectType == SkillObjectType.OnTile || currentAttackData.skillObjectType == SkillObjectType.MultipleTile)
                            {
                                DoAttack(currentAttackData, tile);
                            }
                        }
                        else
                        {
                            Debug.Log("No Character has been found");
                        }
                    }
                }
            }

            if(Input.GetMouseButtonDown(1))
            {
                currentAttackingUnit.StopAttacking();
            }
        }
    }

    public static void StopAttacking()
    {
        localUnitIsAttacking = false;
        UnHighlightCurrentTiles();
    }

    public static void ShowAttackingRange(Tile tileUnitIsOn, SkillData skillData,Character unitAttacking)
    {
        Debug.Log("Showing Attacking Range");
        currentHighlightedTiles = MapControls.HighlightTilesInRange(tileUnitIsOn, skillData.range,Color.red);
        localUnitIsAttacking = true;
        currentAttackData = skillData;
    }

    public static void DoAttack(SkillData data, Character unitToAttack)
    {
        currentAttackingUnit.transform.LookAt(unitToAttack.transform);

        currentAttackingUnit.onAttack();

        bool unitHit = currentAttackingUnit.RollToHit(unitToAttack);

        int damageAmount = data.UseSkill(currentAttackingUnit.transform.position, unitToAttack, currentAttackingUnit.transform, unitHit);

        /**if (unitHit)
        {
            Debug.Log("Player hit enemy " + unitToAttack.name);
        }
        else
        {
            Debug.Log("Player missed attack on " + unitToAttack.name);
        }*/

        currentAttackingUnit.pView.RPC("NetworkAttack", Photon.Pun.RpcTarget.Others,currentAttackingUnit.characterID,unitToAttack.characterID, currentAttackingUnit.knownSkills.IndexOf(data), unitHit ,damageAmount);

        currentAttackingUnit.UseAction();
        currentAttackingUnit.StopAttacking();
    }

    public static void DoAttack(SkillData data, Tile tileToUseSkillOn)
    {
        currentAttackingUnit.transform.LookAt(tileToUseSkillOn.tileGO.transform);

        currentAttackingUnit.onAttack();

        List<bool> unitsHit = new List<bool>();
        List<Character> units = tileToUseSkillOn.GetUnitsOnTile();

        for (int i = 0; i < units.Count; i++)
        {
            unitsHit.Add(currentAttackingUnit.RollToHit(units[i]));
            if (unitsHit[i])
            {
                Debug.Log("Player hit enemy " + units[i].name);
            }
            else
            {
                Debug.Log("Player missed attack on " + units[i].name);
            }
        }

        data.UseSkill(tileToUseSkillOn.tileGO.transform.position, units, unitsHit);

        currentAttackingUnit.UseAction();
        currentAttackingUnit.StopAttacking();
    }

    public static void NetworkDoAttack(int attackingCharacterID,int characterIDToAttack,SkillData skillData,bool attackHit,int damageAmount)
    {
        Character unitToAttack = GameManager.GetCharacterByCharacterID(characterIDToAttack);
        currentAttackingUnit = GameManager.GetCharacterByCharacterID(attackingCharacterID);
        currentAttackingUnit.transform.LookAt(unitToAttack.transform);

        currentAttackingUnit.onAttack();

        skillData.UseSkill(currentAttackingUnit.transform.position, unitToAttack, currentAttackingUnit.transform, attackHit);

        /**if (unitHit)
        {
            Debug.Log("Player hit enemy " + unitToAttack.name);
        }
        else
        {
            Debug.Log("Player missed attack on " + unitToAttack.name);
        }*/

        currentAttackingUnit.UseAction();
        currentAttackingUnit.StopAttacking();
    }

    public static void UnHighlightCurrentTiles()
    {
        MapControls.ClearHighlightedTiles(currentHighlightedTiles);

        currentHighlightedTiles.Clear();
        localUnitIsAttacking = false;
    }
}
