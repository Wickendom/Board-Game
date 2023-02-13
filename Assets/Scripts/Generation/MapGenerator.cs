using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.Mathematics;
using System.Linq;

[RequireComponent(typeof(HexTileMap))]
public class MapGenerator : MonoBehaviourPunCallbacks
{
    public static MapGenerator Instance;

    public static PhotonView myPV;
    HexTileMap hexTileMap;

    [SerializeField]
    private GameObject baseEnemyPrefab;// this is the enemy prefab with the sript on it with no model

    [HideInInspector] public int tileSpawnMapSeed;
    [HideInInspector] public int tileSpawnConnectionPlacementSeed;

    [SerializeField, Range(0,6)]
    int R = 3;

    Noise tileSpawnNoise;// these noise maps need editing to allow biomes to spawn. create noise maps on genedration.
    Noise tileSpawnConnectionNoise;
    float2 riverNoise;

    public MapTypeData MapTypeData;

    [Header("Map Gen Settings")]
    public int mapSize = 0;

    public float elevationMapNoiseScale = 0.08f;

    [Header("River Gen Settings")]
    public float riverGenNoiseScale = 0.01f;
    public float riverFalloffValue = 0.7f;
    [Range(-1,1)]
    public float riverElevationStartFalloffValue = 0.2f;

    public static bool initialMapLoaded;

    
    [SerializeField, Space(10)]
    private Transform tileMapParent;

    private Tile startTile;
    private Tile goalTile;

    private List<int> tileModelTypeIDs;
    private List<int> tileIndividualTypeIDs;
    private bool tileTypeIDsSet = false;
    private int curentTileTypeIndex = 0;

    private bool calledMapGenerationOnOtherClients = false;

    public enum TileType//this is the tile type and how much movement it takes to go through
    {
        Normal = 1,
        Hazard = 2, 
    }

    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }

        //GenerateMap();
        myPV = GetComponent<PhotonView>();
        hexTileMap = GetComponent<HexTileMap>();

        if(WorldGenerationSettings.worldSeed != 0)
        {
            tileSpawnMapSeed = WorldGenerationSettings.worldSeed;
        }
        else
        {
            tileSpawnMapSeed = 0;
        }
    }

    void Start()
    {
        tileModelTypeIDs = new List<int>();
        tileIndividualTypeIDs = new List<int>();
        if(PhotonNetwork.IsConnected && !PhotonNetwork.IsMasterClient)
        {
            return;
        }
        else if(PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
        {
            tileSpawnMapSeed = UnityEngine.Random.Range(0, 99999);
            tileSpawnConnectionPlacementSeed = UnityEngine.Random.Range(0, 99999);
            

            Debug.Log("Tile spawn seed: " + tileSpawnMapSeed);
            Debug.Log("Tile spawn connection seed: " + tileSpawnConnectionPlacementSeed);

            tileSpawnNoise = new Noise(tileSpawnMapSeed);
            tileSpawnConnectionNoise = new Noise(tileSpawnConnectionPlacementSeed);

            GenerateMap();
        }
    }

    public void BeginGenerationOnOtherClients()
    {
        if(!calledMapGenerationOnOtherClients)
        {
            myPV.RPC("ReceiveMapSeedAndGenerate", RpcTarget.Others, tileSpawnMapSeed, tileSpawnConnectionPlacementSeed, tileModelTypeIDs.ToArray(), tileIndividualTypeIDs.ToArray());
            calledMapGenerationOnOtherClients = true;
        }
    }

    [PunRPC]
    public void ReceiveMapSeedAndGenerate(int tileSpawnMapSeed, int tileSpawnConnectionPlacementSeed, int[] tileModelIDs, int[] tileIndividualIDs)
    {
        tileSpawnNoise = new Noise(tileSpawnMapSeed);
        tileSpawnConnectionNoise = new Noise(tileSpawnConnectionPlacementSeed);

        tileModelTypeIDs = tileModelIDs.ToList();
        tileIndividualTypeIDs = tileIndividualIDs.ToList();

        tileTypeIDsSet = true;

        Debug.Log("Tile spawn seed: " + tileSpawnMapSeed);
        Debug.Log("Tile spawn connection seed: " + tileSpawnConnectionPlacementSeed);

        GenerateMap();
    }

    public void GenerateMap()//by this point the chunk will already have its coords and a tilemap created (no data generated though)
    {
        //this.chunkSize = chunkSize;
        //int startingTilePosX = chunkToGenerate.xCoord * chunkSize;
        //int startingTilePosY = chunkToGenerate.yCoord * chunkSize;

        //Debug.Log(chunkToGenerate.xCoord + " " + chunkToGenerate.yCoord);
        float tilePosX = 0;
        float tilePosY = 0;

        Vector3 firstTilePosInChunk = Vector2.zero;

        //List<MeshFilter> chunkTileMeshes = new List<MeshFilter>();

        for (int z = 0; z < mapSize; z++) //Spawns the Tiles
        {
            for (int x = 0; x < mapSize; x++)
            {
                Vector2 elevationEvaluationPointExcludingScale = new Vector2(tilePosX + 1, tilePosY + 1);
                Vector3 elevationEvaluationPoint = new Vector3((tilePosX + 1) * elevationMapNoiseScale, (tilePosY + 1) * elevationMapNoiseScale, 0);

                float elevationNoiseEvaluation = tileSpawnNoise.Evaluate(elevationEvaluationPoint);

                Tile tile = CreateTile(x, z);


                if (elevationNoiseEvaluation >= 0)
                {
                    CreateGameObjectForTile(tile);   
                }
               
                tilePosY++;
            }
            tilePosY = 0;
            tilePosX++;
        }

        ////
        /// MAKE SURE THE START POINT CONNECTS TO THE END POINT
        /// 
        List<Tile> connectionTiles = new List<Tile>();

        for (int xc = 0; xc < mapSize; xc++)
        {
            for (int zc = 0; zc < mapSize; zc++)
            {
                double max = 0;

                float value = 0;
                for (int xn = xc - R; xn <= xc + R; xn++)
                {
                    for (int zn = zc - R; zn <= zc + R; zn++)
                    {
                        value = EvaluateBlueNoise(xn, zn, tileSpawnConnectionNoise);

                        double e = value;
                        if (e > max)
                        {
                            max = e;
                        }
                    }
                }

                if (value == max)
                {
                    int xCoord = xc - zc / 2;
                    int zCoord = zc;

                    Tile tileTemp = HexTileMap.GetTileAtCoord(new Vector3(xCoord,zCoord,-xCoord - zCoord));
                    connectionTiles.Add(tileTemp);

                    //Debug.Log("Connection Tile Generated at q " + xCoord + " r " + zCoord + " s " + (-xCoord - zCoord).ToString());
                }
            }
        }

        //Debug.Log("starting pos = q " + connectionTiles[0].q + " r " + connectionTiles[0].r + " s " + connectionTiles[0].s + " goal tile pos = q " + connectionTiles[1].q + " r " + connectionTiles[1].r + " s " + connectionTiles[1].s);

        for (int i = 1; i < connectionTiles.Count; i++)
        {
            List<Tile> tilePathBetweenConnections = new List<Tile>();

            startTile = connectionTiles[0];
            goalTile = connectionTiles[connectionTiles.Count - 1];

            AStarSearch pathFind = new AStarSearch(connectionTiles[i - 1].GetTileAxialLocation(), connectionTiles[i].GetTileAxialLocation(), true, false);

            tilePathBetweenConnections = pathFind.FindPath();

            for (int j = 0; j < tilePathBetweenConnections.Count; j++)
            {                
                if(!tilePathBetweenConnections[j].pathable)
                {
                    CreateGameObjectForTile(tilePathBetweenConnections[j]);
                }
            }
        }

        CheckAllTilesCanReachGoal();

        Tile startingTile = new Tile();
        HexTileMap.tileMap.TryGetValue(new Vector3(0, 0, 0), out startingTile);
        HexTileMap.SetAllTilesHidden();
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
        {
            hexTileMap.SetPlayerSpawnPoint();
        }

        GameManager.CreateLocalPlayer();

        NetworkReadyCheckerContainer.pView.RPC("SetMapLoaded", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);        

        #region NOT USED STUFF
        /*float resourceSpacingPosX = (float)(chunkSize * tileSize.x) / (float)resourceNoiseMapSize;
        float resourceSpacingPosZ = (float)(chunkSize * (tileSize.y * 0.75f)) / (float)resourceNoiseMapSize;

        float decorationSpacingPosX = (float)(chunkSize * tileSize.x) / (float)decorationNoiseMapSize;
        float decorationSpacingPosZ = (float)(chunkSize * (tileSize.y * 0.75f)) / (float)decorationNoiseMapSize;

        int decorationXPos = 0;
        int decorationYPos = 0;

        for (int x = 0; x < resourceNoiseMapSize; x++)
        {
            for (int y = 0; y < resourceNoiseMapSize; y++)
            {
                
                float xPos = firstTilePosInChunk.x + -(tileSize.x * 0.5f) + (resourceSpacingPosX * (x + 1));//0 + -0.5 + 0.6 = 0.1f
                float zPos = firstTilePosInChunk.z + -(tileSize.y * 0.5f) + (resourceSpacingPosZ * (y + 1));

                decorationXPos = Mathf.FloorToInt(((float)chunkSize / (float)resourceNoiseMapSize) * x) + (chunkToGenerate.xCoord * chunkSize);
                decorationYPos = Mathf.FloorToInt(((float)chunkSize / (float)resourceNoiseMapSize) * y) + (chunkToGenerate.yCoord * chunkSize);

                int tileXPos = (int)(((float)chunkSize / (float)resourceNoiseMapSize) * x);
                int tileYPos = (int)(((float)chunkSize / (float)resourceNoiseMapSize) * y);

                Tile tile = chunkToGenerate.tileMap[tileXPos, tileYPos];

                float spacingForResourceInMapTileNoiseX = (float)chunkSize / (float)resourceNoiseMapSize;
                float spacingForResourceInMapTileNoiseY = (float)chunkSize / (float)resourceNoiseMapSize;

                float resourceNoiseEvaluation = ((resourceMapNoise.Evaluate(new Vector3(((spacingForResourceInMapTileNoiseX * (x + 1f)) + (chunkToGenerate.xCoord * chunkSize)) * resourceGenNoiseScale, ((spacingForResourceInMapTileNoiseY * (y + 1f)) + (chunkToGenerate.yCoord * chunkSize)) * resourceGenNoiseScale, 0f)) + 1f) * (tile.biome.resourceNoiseMapSizeScale) - 1f);

                Vector2 elevationEvaluationPointExcludingScale = new Vector2(decorationXPos + 1, decorationYPos + 1);

                riverNoise = noise.cellular(new float2((elevationEvaluationPointExcludingScale.x + mapRiverPlacementSeed) * riverGenNoiseScale, (elevationEvaluationPointExcludingScale.y + mapRiverPlacementSeed) * riverGenNoiseScale));

                if (riverNoise.x <= riverFalloffValue)
                {
                    if(resourceNoiseEvaluation > tile.biome.resourceNoiseFalloffValue)
                    {
                        double max = 0;

                        float value = 0;
                        for (int xc = x - tile.biome.resourceSpawnRate; xc <= x + tile.biome.resourceSpawnRate; xc++)//Tree placement
                        {
                            for (int yc = y - tile.biome.resourceSpawnRate; yc <= y + tile.biome.resourceSpawnRate; yc++)
                            {
                                value = EvaluateBlueNoise(xc * resourceGenNoiseScale, yc * resourceGenNoiseScale, resourceMapNoise);

                                double e = value;
                                if (e > max)
                                {
                                    max = e;
                                }
                            }
                        }

                        if (value == max)
                        {
                            if (tile.biome.resourcePrefabs.Length > 0)
                            {
                                //float treeLeftoverFalloffValue = 1 - ((tile.biome.resourceNoiseFalloffValue + 1) * 0.5f);
                                //float valuePerTreeToPick = (treeLeftoverFalloffValue * 2) / tile.biome.resourcePrefabs.Length;
                                int resourcePrefabIndex = UnityEngine.Random.Range(0, tile.biome.resourcePrefabs.Length);
                                

                                for (int t = 0; t < tile.biome.resourcePrefabs.Length; t++)
                                {
                                    if (value >= (valuePerTreeToPick * (t + 1)))
                                    {
                                        resourcePrefabIndex = t;
                                    }
                                }

                                float ySpawnPos = 0f;

                                if (chunkToGenerate.tileMap[tileXPos, tileYPos] != null)
                                {
                                    ySpawnPos = chunkToGenerate.tileMap[tileXPos, tileYPos].TopOfTilePosition().y;
                                }

                                GameObject temp = Instantiate(tile.biome.resourcePrefabs[resourcePrefabIndex], new Vector3(xPos, ySpawnPos, zPos), Quaternion.identity, chunkToGenerate.tileMapParent);
                                chunkToGenerate.AddResourceToChunk(temp, resourcePrefabIndex);
                            }
                        }
                    }
                }
            }
        }// Resource Generation */

        /*for (int x = 0; x < decorationNoiseMapSize; x++)
        {
            for (int y = 0; y < decorationNoiseMapSize; y++)
            {
                float xPos = firstTilePosInChunk.x + -(tileSize.x * 0.5f) + (decorationSpacingPosX * (x + 1));//0 + -0.5 + 0.6 = 0.1f
                float zPos = firstTilePosInChunk.z + -(tileSize.y * 0.5f) + (decorationSpacingPosZ * (y + 1));

                decorationXPos = Mathf.FloorToInt(((float)chunkSize / (float)decorationNoiseMapSize) * x) + (chunkToGenerate.xCoord * chunkSize);
                decorationYPos = Mathf.FloorToInt(((float)chunkSize / (float)decorationNoiseMapSize) * y) + (chunkToGenerate.yCoord * chunkSize);

                float spacingForDecorationInMapTileNoiseX = (float)chunkSize / (float)decorationNoiseMapSize;
                float spacingForDecorationInMapTileNoiseY = (float)chunkSize / (float)decorationNoiseMapSize;

                int tileXPos = (int)(((float)chunkSize / (float)decorationNoiseMapSize) * x);
                int tileYPos = (int)(((float)chunkSize / (float)decorationNoiseMapSize) * y);

                Tile tile = chunkToGenerate.tileMap[tileXPos, tileYPos];

                float decorationNoiseEvaluation = ((decorationMapNoise.Evaluate(new Vector3(((spacingForDecorationInMapTileNoiseX * (x + 1f)) + (chunkToGenerate.xCoord * chunkSize)) * decorationGenNoiseScale, ((spacingForDecorationInMapTileNoiseY * (y + 1f)) + (chunkToGenerate.yCoord * chunkSize)) * decorationGenNoiseScale, 0f)) + 1f) * (tile.biome.decorationNoiseMapSizeScale) - 1f);

                Vector3 elevationEvaluationPoint = new Vector3((decorationXPos + 1) * elevationMapNoiseScale, (decorationYPos + 1) * elevationMapNoiseScale, 0);
                float elevationNoiseEvaluation = elevationMapNoise.Evaluate(elevationEvaluationPoint);

                Vector2 elevationEvaluationPointExcludingScale = new Vector2(decorationXPos + 1, decorationYPos + 1);

                riverNoise = noise.cellular(new float2((elevationEvaluationPointExcludingScale.x + mapRiverPlacementSeed) * riverGenNoiseScale, (elevationEvaluationPointExcludingScale.y + mapRiverPlacementSeed) * riverGenNoiseScale));

                if (riverNoise.x < riverFalloffValue ||  elevationNoiseEvaluation > riverElevationStartFalloffValue)// if the river noise does not meet the required amount to spawn a river
                {
                    if(decorationNoiseEvaluation > tile.biome.decorationNoiseFalloffValue)
                    {
                        double max = 0;

                        float value = 0;
                        for (int xc = x - tile.biome.decorationSpawnRate; xc <= x + tile.biome.decorationSpawnRate; xc++)//decoration Placement
                        {
                            for (int yc = y - tile.biome.decorationSpawnRate; yc <= y + tile.biome.decorationSpawnRate; yc++)
                            {
                                value = EvaluateBlueNoise(xc * decorationGenNoiseScale, yc * decorationGenNoiseScale, decorationMapNoise);

                                double e = value;
                                if (e > max)
                                {
                                    max = e;
                                }
                            }
                        }

                        if (value == max)
                        {
                            if (tile.biome.decorationPrefabs.Length > 0)
                            {
                                //float decorationLeftoverFalloffValue = 1 - ((tile.biome.decorationNoiseFalloffValue + 1) * 0.5f);
                                //float valuePerGrassToPick = (decorationLeftoverFalloffValue * 2) / tile.biome.decorationPrefabs.Length;
                                int decorationPrefabIndex = UnityEngine.Random.Range(0,tile.biome.decorationPrefabs.Length);

                                for (int t = 0; t < tile.biome.decorationPrefabs.Length; t++)
                                {
                                    //print(valuePerGrassToPick * (t+1));
                                    if (value >= (valuePerGrassToPick * (t+1)))
                                    {
                                        decorationPrefabIndex = t;
                                    }
                                }

                                float ySpawnPos = 0;

                                if (chunkToGenerate.tileMap[tileXPos, tileYPos] != null)
                                {
                                    ySpawnPos = chunkToGenerate.tileMap[tileXPos, tileYPos].TopOfTilePosition().y;
                                }

                                if(tile.biome == biomes[1])
                                {
                                    GameObject temp = Instantiate(tile.biome.decorationPrefabs[decorationPrefabIndex], new Vector3(xPos, ySpawnPos, zPos), Quaternion.identity, chunkToGenerate.tileMapParent);

                                    float randomScale = UnityEngine.Random.Range(0.7f, 1.15f);
                                    temp.transform.localScale = temp.transform.localScale * randomScale;

                                    /*float randomScale = UnityEngine.Random.Range(0.7f, 1.15f);

                                    curBatch.Add(new GrassData(new Vector3(xPos, ySpawnPos, zPos), Quaternion.identity, new Vector3(randomScale, randomScale, randomScale)));
                                    curGrassMatricesBatches.Add(curBatch[batchIndexNum].matrix);

                                    chunkToGenerate.AddInstancedDecorationToChunk(curBatch[batchIndexNum].matrix);
                                    
                                    batchIndexNum++;
                                    if (batchIndexNum >= 1000)
                                    {
                                        //print("grass batch adding");
                                        grassBatches.Add(curBatch);
                                        grassMatricesBatches.Add(curGrassMatricesBatches);
                                        curBatch = new List<GrassData>();
                                        curGrassMatricesBatches = new List<Matrix4x4>();
                                        batchIndexNum = 0;
                                    }
                                }
                                else
                                {
                                    GameObject temp = Instantiate(tile.biome.decorationPrefabs[decorationPrefabIndex], new Vector3(xPos, ySpawnPos, zPos), Quaternion.identity, chunkToGenerate.tileMapParent);

                                    float randomScale = UnityEngine.Random.Range(0.7f, 1.15f);
                                    temp.transform.localScale = temp.transform.localScale * randomScale;
                                }
                            }
                        }
                    }

                }
            }
        }// Grass Generation*/

        //if(initialMapLoaded == false)
        //initialMapLoaded = true;

        //grassBatches.Add(curBatch);
        //grassMatricesBatches.Add(curGrassMatricesBatches);
        #endregion
    }

    private Tile CreateTile(int x,int z)
    {
        int xCoord = x - z / 2;
        int zCoord = z;

        Tile tile = new Tile(xCoord, zCoord, -xCoord - zCoord);

        tile.SetXZMapCoords(x, z);

        // TMPro.TextMeshProUGUI[] coordUIs = tileGO.GetComponentsInChildren<TMPro.TextMeshProUGUI>();

        // coordUIs[0].text = xCoord.ToString();
        //coordUIs[1].text = zCoord.ToString();
        Tile temp;
        if(!HexTileMap.tileMap.TryGetValue(new Vector3(tile.q,tile.r,tile.s), out temp))
            HexTileMap.tileMap.Add(new Vector3(tile.q, tile.r, tile.s), tile);
        
        return tile;
    }

    private void CreateGameObjectForTile(Tile tile)
    {
        tile.SetTilePathable();

        Vector2 tileIDs = new Vector2();

        if(!tileTypeIDsSet)
        {
            tileIDs = MapTypeData.CreateRandomTileGameObjectIDs();
            tileModelTypeIDs.Add((int)tileIDs.x);
            tileIndividualTypeIDs.Add((int)tileIDs.y);
        }
        else
        {
            
            //Debug.Log("tileModelTypeIDs count is " + (tileModelTypeIDs.Count) + " currentTileTypeIndex " + (curentTileTypeIndex));
            tileIDs = new Vector2(tileModelTypeIDs[curentTileTypeIndex],tileIndividualTypeIDs[curentTileTypeIndex]);

            curentTileTypeIndex++;
        }

        tile.tileModelTypeGroupID = (int)tileIDs.x;
        tile.individualTileModelID = (int)tileIDs.y;

        GameObject tileGO = Instantiate(MapTypeData.GetTileGameObjectByID(tile.tileModelTypeGroupID,tile.individualTileModelID), Vector3.zero, Quaternion.identity, tileMapParent);

        //Debug.Log(tile.individualTileModelID);

        tileGO.layer = 8;//set the tile to the Tile Layer

        tile.SetTileGameObject(tileGO);
        tileGO.GetComponent<TileData>().SetTile(tile);

        BoxCollider bocCol = null;

        if (!tileGO.GetComponent<BoxCollider>())
        {
            bocCol = tileGO.AddComponent<BoxCollider>();
        }
        else
        {
            bocCol = tileGO.GetComponent<BoxCollider>();
        }

        bocCol.center = new Vector3(-0.09852743f, 0, 0.01844186f);
        bocCol.size = new Vector3(3.422277f, 1, 3.867934f);

        Vector3 position;
        position.x = (tile.x + tile.z * 0.5f - tile.z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = tile.z * 4.51f;

        tileGO.transform.localPosition = position;

        HexTileMap.tileMap[new Vector3(tile.q, tile.r, tile.s)] = tile;
    }

    private void DeleteTile(Tile tile)
    {
        Destroy(tile.tileGO);
        tile.tileGO = null;
        tile.pathable = false;
    }

    private float EvaluateBlueNoise(float xPos,float yPos, Noise noiseToEvaluate)
    {
        float value = 0f;
        value = ( 0.5f * (noiseToEvaluate.Evaluate(new Vector2(xPos,yPos)) - noiseToEvaluate.Evaluate(new Vector2(xPos + 1f, yPos + 1f))));
        return value;
    }

    private void CheckAllTilesCanReachGoal()
    {
        List<Tile> allTiles = HexTileMap.GetPathableTiles();

        for (int i = 0; i < allTiles.Count; i++)
        {
            AStarSearch pathfind = new AStarSearch(allTiles[i].GetTileAxialLocation(), goalTile.GetTileAxialLocation(), false, false);

            if(pathfind.FindPath().Count == 0)
            {
                Debug.Log("Found Tile that can't reach goal tile. Deleting Tile");
                DeleteTile(allTiles[i]);
            }
        }
    }

    [PunRPC]
    void SetMapGeneratorSeeds(int mapSeed, Player player)
    {
        Debug.Log("am i the master client " + PhotonNetwork.IsMasterClient);
        tileSpawnMapSeed = mapSeed;//mapSeed;

        tileSpawnNoise = new Noise(tileSpawnMapSeed);

        //chunkLoading.LoadChunk(0, 0);
        //hexTileMap.BeginMapGeneration(0, 0);
    }

    [PunRPC]
    void TellMasterToSyncSeeds(Player player)
    {
        myPV.RPC("SetMapGeneratorSeeds", RpcTarget.Others, tileSpawnMapSeed, player);
    }
}

public struct Tile
{
    public Tile(int q, int r, int s)
    {
        this.q = q;
        this.r = r;
        this.s = s;
        if (q + r + s != 0) Debug.LogError("q + r + s must be 0");

        coords = new Vector3(q, r, s);

        x = 0;
        z = 0;

        tileGO = null;

        tileType = MapGenerator.TileType.Normal;

        pathable = false;

        viewable = false;

        tileModelTypeGroupID = 0;
        individualTileModelID = 0;

        currentUnitsOnTile = 0;
        unitsOnTile = new List<Character>();
        playersOnTile = new List<Character>();
        enemiesOnTile = new List<Enemy>();

        tileData = null;
    }

    public void SetTileGameObject(GameObject GO)
    {
        tileGO = GO;
        tileData = tileGO.GetComponentInChildren<TileData>();
        //tileData.FindMovePositions();
    }

    public void SetTilePathable()
    {
        pathable = true;
    }

    public void SetTileType(MapGenerator.TileType tileType)
    {
        this.tileType = tileType;
    }

    public void SetTileViewable()
    {
        if(tileGO)
        {
            tileGO.GetComponent<TileData>().ShowDecorations();
            if(enemiesOnTile.Count > 0)
            {
                for (int i = 0; i < enemiesOnTile.Count; i++)
                {
                    enemiesOnTile[i].Activate();
                }
            }
        }
            
    }

    public void SetTileHidden()
    {
        if(tileGO)
            tileGO.GetComponent<TileData>().HideDecorations();
    }

    public void SetXZMapCoords(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public readonly int q;
    public readonly int r;
    public readonly int s;
    public readonly Vector3 coords;

    public int x;
    public int z;

    public GameObject tileGO;
    public MapGenerator.TileType tileType;
    public int tileModelTypeGroupID;
    public int individualTileModelID;

    public bool pathable;

    public bool viewable;

    private int currentUnitsOnTile;
    private List<Character> unitsOnTile;
    private List<Character> playersOnTile;
    private List<Enemy> enemiesOnTile;

    public TileData tileData;

    public Vector3 GetTileAxialLocation()
    {
        return new Vector3(q, r, s);
    }

    public void AddPlayerToTile(Character character)
    {
        if(currentUnitsOnTile < 3)
        {
            if(playersOnTile == null)
            {
                playersOnTile = new List<Character>();
            }
            playersOnTile.Add(character);
            AddUnitToTile(character);
        }
        else
        {
            Debug.LogError("Player trying to be added to a tile that already has 3 units on it, unable to add player to tile");
        }
    }

    public void RemovePlayerFromTile(Character character)
    {
        playersOnTile.Remove(character);
        RemoveUnitFromTile(character);
    }

    public void AddUnitToTile(Character character)
    {
        if(unitsOnTile == null)
        {
            unitsOnTile = new List<Character>();
        }
        unitsOnTile.Add(character);
        
    }

    public void RemoveUnitFromTile(Character character)
    {
        unitsOnTile.Remove(character);
    }

    public void AddEnemyToTile(Character character)
    {
        if (currentUnitsOnTile < 3)
        {
            enemiesOnTile.Add((Enemy)character);
            AddUnitToTile(character);
        }
        else
        {
            Debug.LogError("Enemy tryed to be added to a tile that already has 3 units on it, unable to add Enemy to tile");
        }
    }

    public void RemoveEnemyFromTile(Character character)
    {
        enemiesOnTile.Remove((Enemy)character);
        RemoveUnitFromTile(character);
    }

    public List<Character> GetPlayersOnTile()
    {
        return playersOnTile;
    }

    public List<Character> GetUnitsOnTile()
    {
        return unitsOnTile;
    }

    public List<Enemy> GetEnemiesOnTile()
    {
        return enemiesOnTile;
    }

    public Tile Add(Tile b)
    {
        return new Tile(q + b.q, r + b.r, s + b.s);
    }

    public Tile Subtract(Tile b)
    {
        return new Tile(q - b.q, r - b.r, s - b.s);
    }


    public Tile Scale(int k)
    {
        return new Tile(q * k, r * k, s * k);
    }

    public static Tile Scale(Tile a, int k)
    {
        return new Tile(a.q * k, a.r * k, a.s * k);
    }


    public Tile RotateLeft()
    {
        return new Tile(-s, -q, -r);
    }


    public Tile RotateRight()
    {
        return new Tile(-r, -s, -q);
    }

    public static Tile Round(float q, float r, float s)
    {
        int rx = (int)q;
        int ry = (int)r;
        int rz = (int)s;

        float x_diff = Mathf.Abs(rx - q);
        float y_diff = Mathf.Abs(ry - r);
        float z_diff = Mathf.Abs(rz - s);
    
        if(x_diff > y_diff && x_diff > z_diff)
        {
            rx = -ry - rz;
        }
        else if(y_diff > z_diff)
        {
            ry = -rx - rz;
        }
        else
        {
            rz = -rx - ry;
        }

        return new Tile(rx, ry, rz);
    }

    static public List<Tile> directions = new List<Tile> { new Tile(1, 0, -1), new Tile(1, -1, 0), new Tile(0, -1, 1), new Tile(-1, 0, 1), new Tile(-1, 1, 0), new Tile(0, 1, -1) };

    static public Tile Direction(int direction)
    {
        return Tile.directions[direction];
    }

    public Tile Neighbor(int direction)
    {
        return Add(Tile.Direction(direction));
    }

    public List<Tile> Neighbors(bool includeEmptyTiles)
    {
        List<Tile> neighbors = new List<Tile>();

        foreach(Tile dir in directions)
        {
            Tile tile = new Tile();
            if (HexTileMap.tileMap.TryGetValue(new Vector3(q + dir.q, r + dir.r, s + dir.s), out tile))
            {
                if (includeEmptyTiles)
                {
                    neighbors.Add(tile);
                }
                else
                {
                    if(tile.pathable)
                    {
                        neighbors.Add(tile);
                    }
                }
                
            }
        }

        return neighbors;
    }

    static public List<Tile> diagonals = new List<Tile> { new Tile(2, -1, -1), new Tile(1, -2, 1), new Tile(-1, -1, 2), new Tile(-2, 1, 1), new Tile(-1, 2, -1), new Tile(1, 1, -2) };

    public Tile DiagonalNeighbor(int direction)
    {
        return Add(Tile.diagonals[direction]);
    }


    public int Length()
    {
        return (int)((Mathf.Abs(q) + Mathf.Abs(r) + Mathf.Abs(s)) * 0.5f);
    }


    public int Distance(Tile b)
    {
        return Subtract(b).Length();
    }
}