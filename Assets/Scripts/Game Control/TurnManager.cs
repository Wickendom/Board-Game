using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class TurnManager : MonoBehaviourPunCallbacks
{
    public static TurnManager Instance;

    public static PhotonView pView;

    private bool gameHasBegun = false;

    Player currentPlayersTurn;
    bool currentTurnStillHasAction = true;

    public static List<Character> playerCharacters;
    private static List<Character> initiativeTracker; //this is the initiative tracker and holds all objects to cycle through whos turn it is.

    [SerializeField]
    private List<Character> debugInitiativeTracker;

    static int currentIndexInInitiativeTracker = 0;

    private enum BattleState {OOB, PreBattle, InBattle};
    private static BattleState battleState = BattleState.OOB;

    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
        pView = GetComponent<PhotonView>();
        playerCharacters = new List<Character>();
        initiativeTracker = new List<Character>();
    }

    [PunRPC]
    public void BeginGame()
    {
        if(!gameHasBegun)
        {
            Debug.Log("Game Has Begun");
            currentIndexInInitiativeTracker = initiativeTracker.Count;
            CycleToNextUnitsTurn();
            gameHasBegun = true;
        }
        
    }

    public IEnumerator EnterPreBattleState()
    {
        battleState = BattleState.PreBattle;

        yield return 0;

        EnterBattleState();
    }

    public void EnterBattleState()
    {
        Debug.Log("Entering Battle State");
        battleState = BattleState.InBattle;

        List<InitiatitveTrackerData> initiativeData = new List<InitiatitveTrackerData>();

        Debug.Log(initiativeTracker.Count + " units in the initiative tracker");

        for (int i = 0; i < initiativeTracker.Count; i++)//Roll Initiative everyone
        {
            InitiatitveTrackerData data = new InitiatitveTrackerData();

            data.initiative = initiativeTracker[i].Speed.RollDie(BoardGame.DieType.D20,true);
            data.character = initiativeTracker[i];

            initiativeData.Add(data);
        }

        initiativeData.OrderBy(o => o.initiative);

        List<Character> characterDatas = new List<Character>();

        for (int i = 0; i < initiativeData.Count; i++)
        {
            characterDatas.Add(initiativeData[i].character);
        }

        InitiativeTrackerUI.Instance.CreateUIs(characterDatas);
    }

    public void EndTurnTrigger()
    {
        pView.RPC("EndTurn", RpcTarget.All);
    }

    [PunRPC]
    public void EndTurn()
    {
        BoardGame.Player player = GetCurrentTurnCharacter() as BoardGame.Player;
        player.UnHighlightAnyTiles();

        initiativeTracker[currentIndexInInitiativeTracker].EndTurn();
        if(CheckIfItIsLocalPlayersTurn())
        {
            UIController.Instance.UnsetLocalPlayersTurn();
        }
        
        CycleToNextUnitsTurn();
    }

    [PunRPC]
    public void EndEnemiesTurn()
    {
        
        
        CycleToNextUnitsTurn();
    }

    public void StartAttacking(SkillData skillData)
    {
        if(CheckIfItIsLocalPlayersTurn())
            initiativeTracker[currentIndexInInitiativeTracker].StartAttack(skillData);
    }

    public static bool CheckIfItIsLocalPlayersTurn()
    {
        if(initiativeTracker[currentIndexInInitiativeTracker] == GameManager.localPlayer)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static Character GetCurrentTurnCharacter()
    {
        return initiativeTracker[currentIndexInInitiativeTracker];
    }

    [PunRPC]
    private static void CycleToNextUnitsTurn()
    {
        if (battleState == BattleState.InBattle)
            InitiativeTrackerUI.Instance.MoveToEnd(initiativeTracker[currentIndexInInitiativeTracker]);

        if (currentIndexInInitiativeTracker < (initiativeTracker.Count-1))
        {
            currentIndexInInitiativeTracker++;
        }
        else
        {
            currentIndexInInitiativeTracker = 0;
        }
        
        if(initiativeTracker[currentIndexInInitiativeTracker].GetType() == typeof(Enemy))
        {
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected || !PhotonNetwork.IsConnected)
                initiativeTracker[currentIndexInInitiativeTracker].StartTurn();
        }
        else
        {
            initiativeTracker[currentIndexInInitiativeTracker].StartTurn();
        }
            

        if (CheckIfItIsLocalPlayersTurn())
        {
            UIController.Instance.SetLocalPlayersTurn();
        }
    }

    [PunRPC]
    public void AddUnitToInitiative(int viewID)
    {
        Debug.Log("Adding player with view ID " + viewID + " to the initiative tracker");
        initiativeTracker.Add(PhotonView.Find(viewID).GetComponent<Character>());
    }

    [PunRPC]
    public  void AddEnemyToInitiative(int viewID)
    {
        initiativeTracker.Add(PhotonView.Find(viewID).GetComponent<Character>());
        //Debug.Log("Adding enemy with view ID " + viewID + " to initiative tracker");
        if (battleState != BattleState.PreBattle)
            StartCoroutine(EnterPreBattleState());
    }

    public static void RemoveUnitFromInitiative(Character unitToRemove)
    {
        initiativeTracker.Remove(unitToRemove);

        Debug.Log("Removing Unit From Initiative Tracker");

        if (unitToRemove is BoardGame.Player)
        {
            Debug.Log("Player Removed From Initiative Tracker");

            for (int i = 0; i < initiativeTracker.Count; i++)
            {
                if(initiativeTracker[i] is BoardGame.Player)
                {
                    Debug.Log("Another Player was found in the initiative tracker");
                    break;
                }

                if(i == initiativeTracker.Count)
                {
                    Debug.Log("No Players left in initative tracker, loading Guild Scene");
                    NetworkSceneManager.Instance.NetworkLoadSceneAsync(0);
                }
            }
        }
        else if(unitToRemove is Enemy)
        {
            InitiativeTrackerUI.Instance.RemoveFromTracker(unitToRemove);
            Debug.Log("Enemy detected to remove");

            for (int i = 0; i < initiativeTracker.Count; i++)
            {
                Debug.Log("Character Found");
                if (initiativeTracker[i] is Enemy)
                {
                    Debug.Log("An Enemy was found in the initiative tracker, so the battle continues");
                    break;
                }

                if (i == initiativeTracker.Count-1)
                {
                    Debug.Log("No Enemies found, turning off initiative tracker");
                    InitiativeTrackerUI.Instance.TurnOffInitiativeTracker();
                    battleState = BattleState.OOB;
                }
            }
        }
        
    }
}

class InitiatitveTrackerData
{
    public Character character;
    public int initiative;
}