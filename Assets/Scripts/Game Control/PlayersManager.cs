using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;


public class PlayersManager : MonoBehaviour
{
    public static NetworkedPlayerData localPlayer;//not implemented yet
    public static int localPlayerID;
    private List<Player> allNetworkedPlayers;// not implemented yet

    public static List<NetworkedPlayerData> allNetworkedPlayerData;

    //TO DO: Implement photon player management of player data's in this class

    private void Awake()
    {
        allNetworkedPlayerData = new List<NetworkedPlayerData>();
        DontDestroyOnLoad(gameObject);  
    }

    [PunRPC]
    public static void CreateNetworkPlayerData(string playerClass, int actorID)
    {
        if(allNetworkedPlayerData == null)
        {
            allNetworkedPlayerData = new List<NetworkedPlayerData>();
        }

        allNetworkedPlayerData.Add(new NetworkedPlayerData(PhotonNetwork.CurrentRoom.GetPlayer(actorID), Lists.GetPlayerClass(playerClass), actorID));

        

        Debug.Log("Player Data Created");
    }

    public void SyncCreatedPlayerDatas()
    {

    }

    [PunRPC]
    public void SetLocalPlayerByID(int id)
    {
        localPlayerID = id;
        localPlayer = allNetworkedPlayerData[localPlayerID];
    }

    public static NetworkedPlayerData GetLocalPlayerNetworkData()
    {
        for (int i = 0; i < allNetworkedPlayerData.Count; i++)
        {
            if(allNetworkedPlayerData[i].player == PhotonNetwork.LocalPlayer)
            {
                return allNetworkedPlayerData[i];
            }
        }

        return null;
    }

    public static NetworkedPlayerData GetPlayerNetworkData(int playerActorNumber)
    {
        for (int i = 0; i < allNetworkedPlayerData.Count; i++)
        {
            if (allNetworkedPlayerData[i].player.ActorNumber == playerActorNumber)
            {
                return allNetworkedPlayerData[i];
            }
        }

        Debug.LogWarning("no player data found, returning null");
        return null;
    }

    public static void UpdatePlayerData(BoardGame.Player player)
    {
        PlayerData temp = allNetworkedPlayerData[player.playerID].playerData;

        temp.Vitality = player.Vitality;
        temp.CombatEffectiveness = player.CombatEffectiveness;
        temp.Strength = player.Strength;
        temp.Dexterity = player.Dexterity;
        temp.Intelligence = player.Intelligence;
        temp.Speed = player.Speed;
        temp.Perception = player.Perception;

        temp.level = player.playerLevel;
        temp.exp = player.currExp;
        temp.skillPoints = player.currSkillPoints;
        temp.knownSkills = player.knownSkills;

        allNetworkedPlayerData[player.playerID].playerData = temp;
    }
}

public class NetworkedPlayerData
{
    public Player player;
    public PlayerData playerData;
    public NetworkedPlayerData(Player player, PlayerClass playerClass, int playerID)
    {
        this.player = player;
        playerData = new PlayerData(playerClass, playerID);
    }
}
