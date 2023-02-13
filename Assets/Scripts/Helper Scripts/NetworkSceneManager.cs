using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkSceneManager : MonoBehaviour
{
    public static NetworkSceneManager Instance;

    Vector3 playerSpawnPos;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject); 
    }

    public void NetworkLoadSceneAsync(int sceneID)
    {
        if(sceneID == 0)
        {
            //GameManager.exitGameScene();
        }

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LoadLevel(sceneID);
        }
        else
        {
            SceneManager.LoadScene(sceneID);
        }
    }

    public void NetworkLoadSceneAsync(int sceneID, Vector3 playerSpawnPos)
    {
        if (sceneID == 0)
        {
            //GameManager.exitGameScene();
        }

        this.playerSpawnPos = playerSpawnPos;
        SceneManager.sceneLoaded += UpdatePlayerPosition;
        if(PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LoadLevel(sceneID);
        }
        else
        {
            SceneManager.LoadScene(sceneID);
        }
        

    }

    public void UpdatePlayerPosition(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Setting player spawn pos");
        //GameManager.Instance.playerSpawnPos = playerSpawnPos;
        SceneManager.sceneLoaded -= UpdatePlayerPosition;
    }
}
