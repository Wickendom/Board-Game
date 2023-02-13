using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    private static int playerSightRange = 3;

    static List<Tile> oldTiles = new List<Tile>();

    private static float IndividualLerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    private static Tile FullTileLerp(Tile startTile, Tile goalTile, float t)
    {
        Tile tile = Tile.Round(
            IndividualLerp(startTile.q,goalTile.q,t),
            IndividualLerp(startTile.r,goalTile.r,t),
            IndividualLerp(startTile.s,goalTile.s,t));

        TileExistsData data = HexTileMap.CheckTileExists(new Vector3(tile.q, tile.r, tile.s));

        return (data.doesExist)?data.tile:new Tile();
    }

    public static void UpdateViewableTiles()
    {
        List<Tile> collectiveTiles = new List<Tile>();

        for (int i = 0; i < GameManager.allPlayers.Count; i++)
        {
            List<Tile> tileRingSightRange = GetRingOfTiles(GameManager.allPlayers[i].GetTileUnitIsOn(), playerSightRange);

            for (int j = 0; j < tileRingSightRange.Count; j++)
            {
                collectiveTiles.AddRange(CheckLineOfSightToTile(GameManager.allPlayers[i].GetTileUnitIsOn(), tileRingSightRange[j]));                
            }
        }

        RemoveNonVisibleTiles();


        for (int i = 0; i < collectiveTiles.Count; i++)
        {
            if (!oldTiles.Contains(collectiveTiles[i]))
            {
                oldTiles.Add(collectiveTiles[i]);
            }
        }
    }

    private static void RemoveNonVisibleTiles()
    {
        List<Tile> tilesPlayersAreOn = new List<Tile>();

        for (int i = 0; i < GameManager.allPlayers.Count; i++)
        {
            tilesPlayersAreOn.Add(GameManager.allPlayers[i].GetTileUnitIsOn());
        }

        for (int i = 0; i < oldTiles.Count; i++)
        {
            bool keepTile = true;

            for (int j = 0; j < tilesPlayersAreOn.Count; j++)
            {
                if(!keepTile)
                {
                    continue;
                }

                if (oldTiles[i].Distance(tilesPlayersAreOn[j]) > (playerSightRange + 4))
                {
                    oldTiles[i].SetTileHidden();
                    oldTiles.RemoveAt(i);
                    keepTile = false;
                }
            }
        }
    }

    private static List<Tile> GetRingOfTiles(Tile centerTile,int radius)
    {
        List<Tile> tiles = new List<Tile>();

        Tile tile = centerTile.Add(Tile.Scale(Tile.Direction(4), radius));

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < radius; j++)
            {
                TileExistsData data = HexTileMap.CheckTileExists(new Vector3(tile.q, tile.r, tile.s));

                if(data.doesExist)
                {
                    tiles.Add(data.tile);
                    /*if (data.tile.tileGO)
                        data.tile.tileGO.GetComponent<TileHighlight>().Highlight(Color.green);*/
                }

                tile = tile.Neighbor(i);
            }
        }

        return tiles;
    }

    private static List<Tile> CheckLineOfSightToTile(Tile startingTile, Tile goalTile)
    {
        int N = startingTile.Distance(goalTile);//how many tiles there are between the starting tile and the goal tile
        List<Tile> tiles = new List<Tile>();

        for (int i = 0; i < N; i++)
        {
            tiles.Add(FullTileLerp(startingTile, goalTile, 1.0f / N * i));
        }
        tiles.Add(goalTile);

        bool tilesStillViewable = true;

        for (int i = 0; i < tiles.Count; i++)
        {
            if(tilesStillViewable)
            {
                if(!tiles[i].pathable)
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

        return tiles;
    }
}