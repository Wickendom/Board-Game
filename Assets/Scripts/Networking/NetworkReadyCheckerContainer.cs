using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkReadyCheckerContainer : MonoBehaviour
{
    public static PhotonView pView;

    // Start is called before the first frame update
    void Awake()
    {
        pView = GetComponent<PhotonView>();
    }

    [PunRPC]
    public void Initialise()
    {
        NetworkReadyChecker.Instance.playerReadyContainer = new List<PlayerNetworkedReadyCheckContainer>();

        Debug.Log("New List created");
    }

    [PunRPC]
    public void AddPlayer(int actorID)
    {
        Debug.Log("Player added to Ready Checker List with actorID " + actorID);
        NetworkReadyChecker.Instance.playerReadyContainer.Add(new PlayerNetworkedReadyCheckContainer(PhotonNetwork.CurrentRoom.GetPlayer(actorID)));
        Debug.Log(NetworkReadyChecker.Instance.playerReadyContainer.Count);
    }

    [PunRPC]
    public void SetMapLoaded(int actorID)
    {
        Debug.Log("player ready containers = " + NetworkReadyChecker.Instance.playerReadyContainer.Count);
        NetworkReadyChecker.Instance.GetPlayerReadyContainer(actorID).mapLoaded = true;
        Debug.Log("Checking if map has loaded on all clients");
        if (NetworkReadyChecker.Instance.CheckMapLoadedOnAllClients())
        {
            Debug.Log("Map loaded on all clients");
            for (int i = 0; i < GameManager.allPlayers.Count; i++)
            {
                Debug.Log("Calling secondary Initialise on " + GameManager.allPlayers[i].playerID + " client");
                GameManager.allPlayers[i].pView.RPC("InitialiseAfterAllPlayersCreated", RpcTarget.All);
            }

            StartCoroutine(StartEnemyCreation());
            //GameManager.localPlayer.pView.RPC("InitialiseAfterAllPlayersCreated", RpcTarget.All);
        }

    }

    private IEnumerator StartEnemyCreation()
    {
        yield return 0;
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
            new EnemySpawner(QuestTracker.GetQuest());
    }

    [PunRPC]
    public void SetPlayerCreated(int actorID)
    {
        NetworkReadyChecker.Instance.GetPlayerReadyContainer(actorID).playerCreated = true;

        if(PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            MapGenerator.Instance.BeginGenerationOnOtherClients();
        }

        if (NetworkReadyChecker.Instance.CheckAllPlayersCreatedOnAllClients())
        {
            TurnManager.pView.RPC("BeginGame", RpcTarget.All);
        }
    }
}
