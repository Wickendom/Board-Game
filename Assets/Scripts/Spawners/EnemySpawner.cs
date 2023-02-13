using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner
{
    Quest quest;

    public List<EnemyData> enemiesToSpawn;
    public EnemySpawner(Quest quest)
    {
        this.quest = quest;
        GenerateAllEnemiesFromQuest();
    }

    public void GenerateAllEnemiesFromQuest()
    {
        enemiesToSpawn = quest.enemiesToKill;
        for (int i = 0; i < quest.enemyAmountToKill; i++)
        {
            Tile tile = HexTileMap.GetRandomPathableTile();
            if(tile.coords == new Vector3(0,0,0))
            {
                Debug.Log("Pathable Tile found was set to 0,0,0");
            }
            SpawnUnit(tile.q,tile.r,tile.s);
        }
    }

    public void SpawnUnit(int q,int r, int s)
    {
        Debug.Log("Enemy Spawning");
        GameObject enemy = Photon.Pun.PhotonNetwork.Instantiate("Enemy",Vector3.zero,Quaternion.identity);
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        enemyScript.pView = enemy.GetComponent<Photon.Pun.PhotonView>();
        enemyScript.data = enemiesToSpawn[0];
        enemyScript.characterID = Lists.CreateCharacterID();
        Tile tile = HexTileMap.GetTileAtCoord(new Vector3(q, r, s));
        

        if (enemy.GetComponent<Character>())
        {
            tile.AddEnemyToTile(enemy.GetComponent<Character>());
            enemyScript.pView.RPC("Initialise",Photon.Pun.RpcTarget.All, enemyScript.data.ID, enemyScript.characterID);
            Vector3 spawnPos = tile.tileData.GetMovePosition().position;
            spawnPos.y = 0;
            enemyScript.pView.RPC("SetPosition", Photon.Pun.RpcTarget.All, spawnPos);
            enemyScript.pView.RPC("SetUnitToTile",Photon.Pun.RpcTarget.All,new Vector3(q, r, s));
            
            Debug.Log("Enemy Initialised");
        }
        else
        {
            Debug.LogError("Enemy has no enemy script attached");
        }
            
    }
}
