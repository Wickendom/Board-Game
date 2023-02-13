using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    private PlayerData playerData;
    private PlayerEquipmentData playerEquipmentData;

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

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void SavePlayerEquipmentData()
    {
        playerEquipmentData = new PlayerEquipmentData();
    }

    public PlayerData GetPlayerData()
    {
        return playerData;
    }

    public PlayerEquipmentData GetPlayerEquipmentData()
    {
        return playerEquipmentData;
    }
}
