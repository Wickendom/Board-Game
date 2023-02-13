using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    private static PhotonView photonView;

    private static GameObject _playerPrefab;
    public GameObject playerPrefab;

    public static BoardGame.Player localPlayer;
    public static List<BoardGame.Player> allPlayers;

    public List<BoardGame.Player> playerScripts;

    public static List<bool> playersLoadedIntoGame;

    public static List<Character> characters;//this is all characters in the current scene.

    // Start is called before the first frame update
    void Awake()
    {
        _playerPrefab = playerPrefab;

        allPlayers = new List<BoardGame.Player>();
        playerScripts = new List<BoardGame.Player>();
        playersLoadedIntoGame = new List<bool>();
        characters = new List<Character>();
    }

    private void Start()
    {
        for (int i = 0; i < PlayersManager.allNetworkedPlayerData.Count; i++)
        {
            playersLoadedIntoGame.Add(false);
        }
    }

    public void OnEnable()
    {
        SavingManager.savePlayer += SaveLocalPlayerData;
    }

    public void OnDisable()
    {
        SavingManager.savePlayer -= SaveLocalPlayerData;
    }

    public static void CreateLocalPlayer()
    {
        GameObject playerGO = null;

        //playerGO = Instantiate(_playerPrefab);
        playerGO = PhotonNetwork.Instantiate("Player", Vector3.zero,Quaternion.identity);
        BoardGame.Player player = playerGO.GetComponent<BoardGame.Player>();
        localPlayer = player;
        //player.SetUnitToTile(HexTileMap.GetRandomPathableTile());
        player.characterID = Lists.CreateCharacterID();
        AddCharacterToList(player);

        player.pView = player.GetComponent<PhotonView>();
        player.pView.RPC("PreInitialise", RpcTarget.Others);
        
        Tile spawnTile = HexTileMap.GetPlayerSpawnTile();
        player.SetUnitToTile(spawnTile.coords);
            
        player.SetPosition(player.GetTileUnitIsOn().tileData.GetMovePosition().position);
        player.GetTileUnitIsOn().tileData.SetMovePosition(player);
        player.InitialisePlayerFromData(PlayersManager.GetLocalPlayerNetworkData().playerData);
        
        player.SetSkillManagerUI(SkillManagerUI.Instance);
        CameraController.Instance.CenterOnStart(localPlayer.transform);
        Debug.Log("Local player created");
        //allPlayers.Add(player);
    }

    public void CreatePlayer()
    {
        GameObject playerGO = null;

        //playerGO = Instantiate(_playerPrefab);
        playerGO = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        BoardGame.Player player = playerGO.GetComponent<BoardGame.Player>();
        
        if(localPlayer == null)
        {
            localPlayer = player;
        }
        
        player.characterID = Lists.CreateCharacterID();
        AddCharacterToList(player);

        player.pView = player.GetComponent<PhotonView>();
        player.pView.RPC("PreInitialise", RpcTarget.Others);

        Tile spawnTile = HexTileMap.GetPlayerSpawnTile();
        player.SetUnitToTile(spawnTile.coords);

        player.SetPosition(player.GetTileUnitIsOn().tileData.GetMovePosition().position);
        player.GetTileUnitIsOn().tileData.SetMovePosition(player);
        player.InitialisePlayerFromData(PlayersManager.GetLocalPlayerNetworkData().playerData);

        player.SetSkillManagerUI(SkillManagerUI.Instance);
        CameraController.Instance.CenterOnStart(localPlayer.transform);
        Debug.Log("Local player created");
    }

    public static void AddCharacterToList(Character chara)
    {
        Debug.Log(string.Format("Adding {0} to character list, character ID is {1}", chara.name, chara.characterID));
        for (int i = 0; i < characters.Count; i++)
        {
            Debug.Log(string.Format("{0} with character ID {1} is in list at time of search", characters[i].name, characters[i].characterID));
        }
        characters.Add(chara);
    }

    public static Character GetCharacterByCharacterID(int characterID)
    {
        Character chara = null;
        Debug.Log("Searching for character ID " + characterID);
        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].characterID == characterID)
            {
                chara = characters[i];
                break;
            }       
        }

        Debug.Log("No Character Found");
        return chara;
    }

    private void SaveLocalPlayerData()
    {
        localPlayer.UpdatePlayerData();
    }

    public static void SecondaryInitialiseAllPlayers()
    {
        for (int i = 0; i < allPlayers.Count; i++)
        {
            allPlayers[i].pView.RPC("InitialiseAfterAllPlayersCreated", RpcTarget.All);
        }
    }

    public static void GivePlayersExp(int amount)
    {
        int splitExp = amount / allPlayers.Count;

        for (int i = 0; i < allPlayers.Count; i++)
        {
            allPlayers[0].AddExp(splitExp);
        }
    }
}
