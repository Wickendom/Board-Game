using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingManager : MonoBehaviour
{
    public delegate void SavePlayer();
    public static SavePlayer savePlayer;

    public static void SaveAllPlayerData()
    {
        savePlayer();
    }
}
