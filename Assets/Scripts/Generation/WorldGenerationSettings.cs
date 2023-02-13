using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WorldGenerationSettings : MonoBehaviourPun
{
    public static int worldSeed;
    public static int resourceSeed;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetNewRandomWorldSeeds()
    {
        worldSeed = Random.Range(0, 9999);
        resourceSeed = Random.Range(0, 9999);
    }
}
