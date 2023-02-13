using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterSelectionUIController : MonoBehaviour
{
    [SerializeField]
    private PlayerClass[] classes = new PlayerClass[4];

    int currentLoadedClass = 0;

    [SerializeField]
    private TextMeshProUGUI classTitle;

    void Start()
    {
        LoadClassData(0);
    }

    private void LoadClassData(int value)
    {
        classTitle.text = classes[value].className;

        CharacterSelectionUIStatValue[] temp = GetComponentsInChildren<CharacterSelectionUIStatValue>();

        int[] stats = classes[value].GetStatValues();

        for (int i = 0; i < temp.Length; i++)
        {
            temp[i].GetText().text = stats[i].ToString();
        }
    }

    public void ShowNextClass()
    {
        currentLoadedClass++;
        if (currentLoadedClass > classes.Length - 1)
            currentLoadedClass = 0;

        LoadClassData(currentLoadedClass);
    }

    public void ShowPreviousClass()
    {
        currentLoadedClass--;
        if (currentLoadedClass == 0)
            currentLoadedClass = classes.Length - 1;

        LoadClassData(currentLoadedClass);
    }

    public void SelectCurrentClass()
    {
        DDOLNetworkHelper.pView.RPC("CreateNetworkPlayerData", Photon.Pun.RpcTarget.AllBuffered,classes[currentLoadedClass].ID, Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber);
        NetworkReadyCheckerContainer.pView.RPC("AddPlayer", Photon.Pun.RpcTarget.MasterClient, Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber);
        //PlayersManager.CreateNetworkPlayerData(classes[currentLoadedClass].ID,Photon.Pun.PhotonNetwork.LocalPlayer);
        GuildController.Instance.photonView.RPC("NetworkSpawnCharacter", Photon.Pun.RpcTarget.AllBuffered, PlayersManager.GetLocalPlayerNetworkData().playerData.playerClass.ID);
        GuildUIController.ToggleCharacterSelectionUI();
        //GuildUIController.ToggleQuestUI(); // This is old, this was to show quests after selecting a class.
    }
}
