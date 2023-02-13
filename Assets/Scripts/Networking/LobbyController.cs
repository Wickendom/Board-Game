using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class LobbyController : MonoBehaviourPunCallbacks
{
    private PhotonView phView;

    private byte maxPlayersPerRoom = 4;

    //[SerializeField,Tooltip("This is how long before the game will start from when the first player joined the room")]
    //private float countdownTime = 10;

    public InputField playerNameInput;

    //[SerializeField]
    //private Text roomStatus;
    //[SerializeField]
    //private Text playersInRoomAmount;
    //[SerializeField]
    //private Text roomCountdownBeforeGameStarts;

   // private float roomCountdownTimer;

    //private bool isReducingCountdownTimer = false;

    private bool inGame = false;
    
    //this is the clients version number. This seperates players based on what version they are playing.
    string gameVersion = "0.1";

    bool isConnecting;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        phView = GetComponent<PhotonView>();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Update()
    {
        /*if(isReducingCountdownTimer)
        {
            roomCountdownTimer -= Time.deltaTime;
            int timer = (int)roomCountdownTimer;
            //roomCountdownBeforeGameStarts.text = timer.ToString();

            if(roomCountdownTimer <= 0)
            {
                isReducingCountdownTimer = false;

                if (PhotonNetwork.PlayerList.Length > 1)
                {
                    StartGame();
                }
                else
                {
                    Debug.LogError("Only 1 player in game. The game failed to start");
                    roomCountdownTimer = 30;
                    isReducingCountdownTimer = true;
                }
            }
        }*/
    }

    public void Connect()
    {
        isConnecting = true;

        if(!playerNameInput.Equals(""))
        {
            //if the client is connected to the server already then join a room.
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else// if not, connect to the server
            {
                string playerName = playerNameInput.text;
                PhotonNetwork.LocalPlayer.NickName = playerName;

                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }
        else
        {
            Debug.LogError("Player name is set to nothing, you can't have nothing as your name");
        }
        
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN");

        if (isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
            isConnecting = false;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed() was called by PUN. No Random Room Was found. Creating room now");

        PhotonNetwork.CreateRoom(null,
            new RoomOptions {
                MaxPlayers = maxPlayersPerRoom,
                IsOpen = true,
                IsVisible = true
            });

        //roomCountdownTimer = countdownTime;
        //roomCountdownBeforeGameStarts.text = roomCountdownTimer.ToString();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room");

        Debug.Log("Loading the Game Scene");

        // Debug.Log("Is user the master client " + PhotonNetwork.IsMasterClient);

        //roomStatus.text = "Room Connected waiting for other players";

        //playersInRoomAmount.text = PhotonNetwork.PlayerList.Length.ToString();
        //if(PhotonNetwork.IsMasterClient)
        //isReducingCountdownTimer = true;

        StartGame();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //playersInRoomAmount.text = PhotonNetwork.PlayerList.Length.ToString();

        //if(PhotonNetwork.IsMasterClient)
            //phView.RPC("SetRoomCountdownTimer", newPlayer, roomCountdownTimer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //if(!inGame)
            //playersInRoomAmount.text = PhotonNetwork.PlayerList.Length.ToString();
    }

    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void StartGame()
    {
        //PhotonNetwork.CurrentRoom.IsOpen = false;
        inGame = true;

        NetworkSceneManager.Instance.NetworkLoadSceneAsync(1);
    }

    [PunRPC]
    void SetRoomCountdownTimer(float curRoomCountdown)
    {
        Debug.Log("Room Countdown Timer set");
        //roomCountdownTimer = curRoomCountdown;
        //isReducingCountdownTimer = true;
    }
}
