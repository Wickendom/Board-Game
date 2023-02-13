using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GuildController : MonoBehaviour
{
    public static GuildController Instance;

    public PhotonView photonView;

    public List<Quest> quests;

    [SerializeField]
    private Transform[] playerSpawnPoints;

    private int playersSpawned = 0;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
    }

    [PunRPC]
    public void NetworkSpawnCharacter(string CharacterDataID)
    {
        Instantiate(Lists.GetPlayerClass(CharacterDataID).modelPrefab,playerSpawnPoints[playersSpawned]);
        playersSpawned++;
        Debug.Log(CharacterDataID + " spawned");
    }

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            NetworkReadyCheckerContainer.pView.RPC("Initialise", RpcTarget.AllBuffered);
            Debug.Log("Initialised");
        }
    }
}
