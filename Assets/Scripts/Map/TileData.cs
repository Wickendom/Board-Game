using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData : MonoBehaviour
{
    public Tile tile;
    [SerializeField]
    private int q;
    [SerializeField]
    private int r;
    [SerializeField]
    private int s;

    [SerializeField]
    private GameObject tileBaseGO;

    [SerializeField]
    private GameObject decorations;

    public Character unit;

    private List<MovePosData> movePositions;//this is the positions the units will move to on the tile

    public void SetTile(Tile tile)
    {
        this.tile = tile;
        q = tile.q;
        r = tile.r;
        s = tile.s;
    }

    public void OnEnable()
    {
        FindMovePositions();
    }

    public void FindMovePositions()
    {
        movePositions = new List<MovePosData>();

        TileMovePoint[] movePoints = GetComponentsInChildren<TileMovePoint>();

        for (int i = 0; i < movePoints.Length; i++)
        {
            MovePosData movePos = new MovePosData();
            movePos.positionTransform = movePoints[i].transform;
            //Debug.Log(movePos.positionTransform.name);
            movePositions.Add(movePos);
        }
    }

    public bool CheckCanMoveToTile()
    {
        for (int i = 0; i < movePositions.Count; i++)
        {
            if (movePositions[i].unit == null)
            {
                return true;
            }
        }

        return false;
    }


    public Transform GetDecorationsTransform()
    {
        return decorations.transform;
    }
    public Transform GetMovePosition()
    {
        for (int i = 0; i < movePositions.Count; i++)
        {
            if(movePositions[i].unit == null)
            {
                return movePositions[i].positionTransform;
            }
        }

        Debug.Log("Unable to find move position on tile " + name);

        return null;
    }

    public void SetMovePosition(Character unit)
    {
        for (int i = 0; i < movePositions.Count; i++)
        {
            if (movePositions[i].unit == null)
            {
                movePositions[i].SetUnit(unit);
                break;
            }
        }
    }

    public void RemoveUnitFromMovePosition(Character unit)
    {
        for (int i = 0; i < movePositions.Count; i++)
        {
            if (movePositions[i].unit == unit)
            {
                movePositions[i].unit = null;
            }
        }
    }

    public Tile GetTile()
    {
        return tile;
    }

    public void HideDecorations()
    {
        if(decorations && tileBaseGO)
        {
            decorations.SetActive(false);
            tileBaseGO.SetActive(false);
        }
        else
        {
            Debug.LogError("Tile does not have the decorations or tileBaseGo gameobject set. Open the " + name + " prefab and set the decorations variable in the TileData script on the base tile prefab");
        }
        
    }

    public void ShowDecorations()
    {
        if(decorations && tileBaseGO)
        {
            decorations.SetActive(true);
            tileBaseGO.SetActive(true);
        }
        else
        {
            Debug.LogError("Tile does not have the decorations or tileBase gameobject set. Open the " + name + " prefab and set the decorations variable in the TileData script");
        }
        
    }
}

public class MovePosData
{
    public Transform positionTransform;
    public Character unit; 

    public void SetUnit(Character unit)
    {
        this.unit = unit;
    }
}