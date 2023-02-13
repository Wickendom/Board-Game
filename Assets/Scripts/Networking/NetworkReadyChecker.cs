using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkReadyChecker : MonoBehaviour
{
    public static NetworkReadyChecker Instance;

    public List<PlayerNetworkedReadyCheckContainer> playerReadyContainer;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        playerReadyContainer = new List<PlayerNetworkedReadyCheckContainer>();
        
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
    }


    public bool CheckMapLoadedOnAllClients()
    {
        for (int i = 0; i < playerReadyContainer.Count; i++)
        {
            Debug.Log("Player ID " + playerReadyContainer[i].player.ActorNumber + " maploaded = " + playerReadyContainer[i].mapLoaded);
            if(playerReadyContainer[i].mapLoaded == false)
            {
                Debug.Log("Map is not loaded on all clients");
                return false;
            }
        }
        Debug.Log("Map is loaded on all clients");
        return true;
    }

    

    public bool CheckAllPlayersCreatedOnAllClients()
    {
        for (int i = 0; i < playerReadyContainer.Count; i++)
        {
            if (playerReadyContainer[i].playerCreated == false)
            {
                return false;
            }
        }

        return true;
    }

    public PlayerNetworkedReadyCheckContainer GetPlayerReadyContainer(int actorID)
    {
        for (int i = 0; i < playerReadyContainer.Count; i++)
        {
            if(actorID == playerReadyContainer[i].player.ActorNumber)
            {
                return playerReadyContainer[i];
            }
        }

        Debug.Log("Unable to find player ready container with actor ID " + actorID);

        return null;
    }
}

public class PlayerNetworkedReadyCheckContainer
{
    public Photon.Realtime.Player player;
    public bool mapLoaded;
    public bool playerCreated;

    public PlayerNetworkedReadyCheckContainer(Photon.Realtime.Player player)
    {
        this.player = player;
        mapLoaded = false;
        playerCreated = false;
    }
}