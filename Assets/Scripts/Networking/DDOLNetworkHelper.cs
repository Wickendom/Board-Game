using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
[RequireComponent(typeof(PhotonView))]
public class DDOLNetworkHelper : MonoBehaviour
{
    public static PhotonView pView;

    // Start is called before the first frame update
    void Start()
    {
        pView = GetComponent<PhotonView>();
    }

    [PunRPC]
    public void CreateNetworkPlayerData(string playerClass, int actorID)
    {
        PlayersManager.CreateNetworkPlayerData(playerClass, actorID);
    }
}
