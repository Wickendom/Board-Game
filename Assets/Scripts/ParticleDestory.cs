using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDestory : MonoBehaviour
{
    private ParticleSystem[] thisParticleSystem; 

    // Start is called before the first frame update
    void Start()
    {
        thisParticleSystem = GetComponentsInChildren<ParticleSystem>();  
    }

    // Update is called once per frame
    void Update()
    {
        bool deathCheck = false;
        bool aliveParticleFound = false;

        for (int i = 0; i < thisParticleSystem.Length; i++)
        {
            //Debug.Log(thisParticleSystem[i].isPlaying);
            if (thisParticleSystem[i].isPlaying || aliveParticleFound)
            {
                aliveParticleFound = true;
                continue;

            }
             if (i == thisParticleSystem.Length - 1)
            {
                deathCheck = true;
            }


        }

        if (deathCheck == true)
        {
            Destroy(gameObject);
        }
        


    }

}
