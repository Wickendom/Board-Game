using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SceneSetupScript : MonoBehaviour
{
    private void Awake()
    {
        if(PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("Scene Essentials", Vector3.zero, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
