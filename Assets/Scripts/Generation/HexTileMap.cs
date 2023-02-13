using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Photon.Pun;

[RequireComponent(typeof(MapGenerator))]
public class HexTileMap : MonoBehaviour
{
    static public HexTileMap Instance;

    static public Dictionary<Vector3, Tile> tileMap;//using the cube tile map from redblobgames website. Vector 3 contains x,y,z coords.

    [SerializeField]
    MapGenerator mapGenerator;

    PhotonView pView;

    //public bool initialChunkCreated;

    [SerializeField]
    private LayerMask tileMask;

    public static Vector3 playerTileSpawnPoint;

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

        tileMap = new Dictionary<Vector3, Tile>();

        mapGenerator = GetComponent<MapGenerator>();
        pView = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        GetTileFromMousePosition();
    }

    /*public void BeginMapGeneration()
    {
        //mapGenerator.GenerateMap();
       // Debug.Log("Chunk created successfully");
    }*/

    public static TileExistsData CheckTileExists(Vector3 qrsPos)
    {
        TileExistsData data = new TileExistsData();

        data.doesExist = tileMap.TryGetValue(qrsPos, out data.tile);

        return data;
    }

    public static Tile GetTileAtCoord(Vector3 QRSPos)
    {
        Tile temp = new Tile(0,0,0);

        if (tileMap.TryGetValue(QRSPos,out temp))
        {
            return temp;
        }
        else
        {
            //Debug.LogError("Tile at " + QRSPos.ToString() + " not found in global tile map");
            return temp;
        }
    }

    public static Tile GetTile(Tile tile)
    {
        Tile temp = new Tile();

        if(!tileMap.TryGetValue(new Vector3(tile.q, tile.r, tile.s), out temp))
        {
            Debug.LogError("Unable to find tile at q " + tile.q + " r " + tile.r + " s " + tile.s);
        }

        return temp;
    }

    public static void SetAllTilesHidden()
    {
        List<KeyValuePair<Vector3, Tile>> pairs = tileMap.ToList();

        for (int i = 0; i < pairs.Count; i++)
        {
            pairs[i].Value.SetTileHidden();
        }
    }

    public Tile GetTileFromWorldPos(Vector3 tilePosition)
    {
        /*float vert = tileHeight * 1.25f;
        float horiz = tileWidth;

        //to get the tiles coords we divide the position of the tile by the width then add 0.4 to account for the offset
        int tileXCoord = (int)((tilePosition.x / tileWidth) + 0.4f);
        int tileYCoord = (int)(tilePosition.z / tileHeight);*/

        Debug.LogWarning("Get Tile From World Pos was called, this is not implemented");
        return new Tile(0, 0, 0);//GetTileFromTileCoords(tileXCoord,tileYCoord);
    }

    public Tile GetTileFromMousePosition()
    {
        Tile tile = new Tile(0, 0, 0);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //Debug.DrawRay(ray.origin, ray.direction * 10, Color.white);
        
        if (Physics.Raycast(ray, out hit,10,tileMask))
        {
            Debug.DrawLine(ray.origin, hit.point);
            tile = hit.collider.GetComponentInParent<Tile>();
            //Debug.Log("Tile Found");
            return tile;
        }
        return new Tile(0, 0, 0);
    }

    public static Tile GetRandomTile()
    {
        List<KeyValuePair<Vector3,Tile>> tiles = tileMap.ToList();
        int randomIndex = Random.Range(0, tiles.Count);

        return tiles[randomIndex].Value;
    }

    public static List<Tile> GetRandomTiles(int amount)
    {
        List<Tile> tiles = new List<Tile>();

        List<KeyValuePair<Vector3, Tile>> tilesKeyPairs = tileMap.ToList();

        List<int> randomIndexes = new List<int>();

        for (int i = 0; i < amount; i++)
        {
            randomIndexes.Add(Random.Range(0, tilesKeyPairs.Count));
            tiles.Add(tilesKeyPairs[randomIndexes[i]].Value);
        }

        return tiles;
    }

    public static List<Tile> GetPathableTiles()
    {
        List<KeyValuePair<Vector3, Tile>> tiles = tileMap.ToList();
        List<Tile> pathableTiles = new List<Tile>();

        foreach (KeyValuePair<Vector3, Tile> tile in tiles)
        {
            if (tile.Value.pathable && tile.Value.tileData != null)
            {
                pathableTiles.Add(tile.Value);
            }
        }

        return pathableTiles;
    }

    public static Tile GetRandomPathableTile()
    {
        List<Tile> pathableTiles = GetPathableTiles();

        int randomIndex = Random.Range(0, pathableTiles.Count);
        
        return pathableTiles[randomIndex];
    }

    public void SetPlayerSpawnPoint()
    {
         playerTileSpawnPoint = GetRandomPathableTile().GetTileAxialLocation();

        Debug.Log("Spawn tile location is " + playerTileSpawnPoint);

        if(PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            pView.RPC("SetPlayerSpawnPoint", RpcTarget.Others, (int)playerTileSpawnPoint.x, (int)playerTileSpawnPoint.y, (int)playerTileSpawnPoint.z);
        }
    }

    [PunRPC]
    public void SetPlayerSpawnPoint(int x,int y, int z)
    {
        playerTileSpawnPoint = new Vector3(x, y, z);
        Debug.Log("Spawn tile location is " + playerTileSpawnPoint);
    }

    public static Tile GetPlayerSpawnTile()
    {
        Tile tile;
        tileMap.TryGetValue(playerTileSpawnPoint, out tile);
        return tile;
    }
}


public struct TileExistsData
{
    public Tile tile;
    public bool doesExist;
}