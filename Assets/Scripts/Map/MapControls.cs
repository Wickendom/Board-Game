using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapControls : MonoBehaviour
{
    public bool selectingTile = true;

    public LayerMask tileLayerMask;

    public List<Tile> currentAStarPath;

    private void Start()
    {
        currentAStarPath = new List<Tile>();
    }

    public static List<Tile> GetAllTilesWithinDistance(Tile originTile, int distance)
    {
        List<Tile> tileResults = new List<Tile>();

        for (int dx = -distance; dx < distance; dx++)
        {
            for (int dy = Mathf.Max(-distance,-dx-distance); dy <= Mathf.Min(distance,-dx+distance); dy++)
            {
                Tile tile = new Tile();
                //Debug.Log("Trying to get tile");
                if(HexTileMap.tileMap.TryGetValue(new Vector3(originTile.q + dx, originTile.r + dy, -(originTile.q + dx) - (originTile.r + dy)),out tile))
                {
                    //Debug.Log("Tile Found");
                    tileResults.Add(tile);
                }
            }
        }

        return tileResults;
    }

    public static List<Tile> HighlightTilesInRange(Tile tileOrigin,int range, Color color)
    {
        List<Tile> tiles = GetAllTilesWithinDistance(tileOrigin, range);

        for (int i = 0; i < tiles.Count; i++)
        {
            if(tiles[i].pathable && tiles[i].tileGO)
                tiles[i].tileGO.GetComponent<TileHighlight>().Highlight(color);
        }

        return tiles;
    }

    public static void ClearHighlightedTiles(List<Tile> tiles)
    {
        if(tiles.Count > 0)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].pathable && tiles[i].tileGO)
                    tiles[i].tileGO.GetComponent<TileHighlight>().UnHighlight();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (selectingTile)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 pos = Vector3.zero;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileLayerMask))
                {
                    for (int i = 0; i < currentAStarPath.Count; i++)
                    {
                        Destroy(currentAStarPath[i].tileGO.GetComponentInChildren<TileHighlight>().gameObject);
                    }
                    TileData tileData = hit.collider.GetComponent<TileData>();

                    Tile startingTile = new Tile();
                    Tile targetTile = tileData.tile;

                    HexTileMap.tileMap.TryGetValue(new Vector3(0,0,0), out startingTile);

                    AStarSearch aStar = new AStarSearch(startingTile.GetTileAxialLocation(), targetTile.GetTileAxialLocation(),false,true);

                    currentAStarPath = aStar.FindPath();

                    for (int i = 0; i < currentAStarPath.Count; i++)
                    {
                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

                        GameObject tempTile = currentAStarPath[i].tileGO;
                        cube.transform.localPosition = tempTile.transform.localPosition;
                        cube.transform.SetParent(tempTile.transform);
                        cube.AddComponent<TileHighlight>();
                    }
                }
            }
        }
    }

    public static List<Tile> Pathfind(Tile startingTile, Tile endingTile)
    {
        AStarSearch aStar = new AStarSearch(startingTile.GetTileAxialLocation(), endingTile.GetTileAxialLocation(),false,false);
        return aStar.FindPath();
    }

    public static List<Character> GetPlayersInRange(Tile originTile, int range)
    {
        List<Tile> tiles = GetAllTilesWithinDistance(originTile, range);
        List<Character> players = new List<Character>();

        for (int i = 0; i < tiles.Count; i++)
        {
            for (int j = 0; j < tiles[i].GetPlayersOnTile().Count; j++)
            {
                players.Add(tiles[i].GetPlayersOnTile()[j]);
            }
        }

        return players;
    }

    public static Character GetClosestPlayerInRange(Tile originTile, int range)
    {
        List<Character> players = GetPlayersInRange(originTile, range);

        int distance = 0;
        Character chara = null;

        for (int i = 0; i < players.Count; i++)
        {
            if (i == 0)
            {
                distance = originTile.Distance(players[i].GetTileUnitIsOn());
                chara = players[i];
            }
            else if (originTile.Distance(players[i].GetTileUnitIsOn()) < distance)
            {
                distance = originTile.Distance(players[i].GetTileUnitIsOn());
                chara = players[i];
            }
        }

        return chara;
    }

    public static Character GetRandomPlayerInRange(Tile originTile, int range)
    {
        List<Character> players = GetPlayersInRange(originTile, range);

        int randomPlayerIndex = 0;

        if (players.Count > 0)
        {
            randomPlayerIndex = Random.Range(0, players.Count - 1);
            return players[randomPlayerIndex];
        }

        return null;
    }

    public static Character GetFurthestPlayerInRange(Tile originTile, int range)
    {
        List<Character> players = GetPlayersInRange(originTile, range);

        if (players.Count > 0)
        {
            int distance = 0;
            Character chara = null;
            for (int i = 0; i < players.Count; i++)
            {
                int temp = originTile.Distance(players[i].GetTileUnitIsOn());
                if(temp > distance)
                {
                    chara = players[i];
                }
            }

            return chara;
        }

        return null;
    }

    public static bool CheckLineOfSight(Tile a, Tile b)
    {
        int N = a.Distance(b);//how many tiles there are between the starting tile and the goal tile
        List<Tile> tiles = new List<Tile>();

        for (int i = 0; i < N; i++)
        {
            tiles.Add(FullTileLerp(a, b, 1.0f / N * i));
        }
        
        tiles.Add(b);

        bool tilesStillViewable = true;

        for (int i = 0; i < tiles.Count; i++)
        {
            if (tilesStillViewable)
            {
                if (!tiles[i].pathable)
                {
                    /*if(tiles[i].tileGO)
                        tiles[i].tileGO.GetComponent<TileHighlight>().Highlight(Color.red);*/
                    tiles.RemoveAt(i);
                    tilesStillViewable = false;
                }
                else
                {
                    /* if (tiles[i].tileGO)
                            tiles[i].tileGO.GetComponent<TileHighlight>().Highlight(Color.yellow);*/
                    tiles[i].SetTileViewable();
                }
            }
            else
            {
                //if (tiles[i].tileGO)
                //tiles[i].tileGO.GetComponent<TileHighlight>().Highlight(Color.red);
                tiles.RemoveAt(i);
            }
        }

        return tilesStillViewable;
        
    }

    private static Tile FullTileLerp(Tile startTile, Tile goalTile, float t)
    {
        Tile tile = Tile.Round(
            IndividualLerp(startTile.q, goalTile.q, t),
            IndividualLerp(startTile.r, goalTile.r, t),
            IndividualLerp(startTile.s, goalTile.s, t));

        TileExistsData data = HexTileMap.CheckTileExists(new Vector3(tile.q, tile.r, tile.s));

        return (data.doesExist) ? data.tile : new Tile();
    }

    private static float IndividualLerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    public static Character GetClosestVisiblePlayer(Tile originTile)
    {
        Character chara = null;
        int distance = 0;

        for (int i = 0; i < TurnManager.playerCharacters.Count; i++)
        {
            Debug.Log(TurnManager.playerCharacters[i].GetTileUnitIsOn().coords);
            //if (i==0)
            //{
                if(CheckLineOfSight(originTile, TurnManager.playerCharacters[i].GetTileUnitIsOn()))
                {
                    chara = TurnManager.playerCharacters[i];
                    distance = originTile.Distance(TurnManager.playerCharacters[i].GetTileUnitIsOn());
                }
                
            /*}
            else if(originTile.Distance(TurnManager.playerCharacters[i].GetTileUnitIsOn()) < distance)
            {
                if(CheckLineOfSight(originTile, TurnManager.playerCharacters[i].GetTileUnitIsOn()))
                {
                    chara = TurnManager.playerCharacters[i];
                    distance = originTile.Distance(TurnManager.playerCharacters[i].GetTileUnitIsOn());
                }
            }*/
        }

        if (chara == null)
        {
            Debug.Log("Closest player unable to be selected");
            Debug.Log(TurnManager.playerCharacters.Count);

            for (int i = 0; i < TurnManager.playerCharacters.Count; i++)
            {
                Debug.Log(string.Format("Player found {0}", TurnManager.playerCharacters[i].characterID));
            }
        }

        return chara;
    }
}

public class PriorityQueue<T>
{
    private List<KeyValuePair<T, float>> elements = new List<KeyValuePair<T, float>>();

    public int Count
    {
        get { return elements.Count; }
    }

    public void Enqueue(T item, float priority)
    {
        elements.Add(new KeyValuePair<T, float>(item, priority));
    }

    // Returns the Location that has the lowest priority
    public T Dequeue()
    {
        int bestIndex = 0;

        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i].Value < elements[bestIndex].Value)
            {
                bestIndex = i;
            }
        }

        T bestItem = elements[bestIndex].Key;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }
}
