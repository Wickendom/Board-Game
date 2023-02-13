using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarSearch
{
    public Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
    public Dictionary<Tile, float> costSoFar = new Dictionary<Tile, float>();

    private Tile start;
    private Tile goal;

    static public float Heuristic(Tile a, Tile b)
    {
        return Mathf.Abs(a.q - b.q) + Mathf.Abs(a.r - b.r);
    }

    // If the heuristic = 2f, the movement is diagonal
    public float Cost(Tile a, Tile b)
    {
        if (Heuristic(a, b) == 2f)
        {
            return (float)(int)b.tileType * Mathf.Sqrt(2f);
        }
        return (float)(int)b.tileType;
    }

    private void ShowPathfind(Tile tile)
    {
        GameObject highlightObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        
        Vector3 position;
        position.x = (tile.x + tile.z * 0.5f - tile.z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = tile.z * 4.51f;

        highlightObject.transform.localPosition = position;
        highlightObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
    }

        // Conduct the A* search
    public AStarSearch(Vector3 start, Vector3 goal, bool includeUnpathableTiles,bool showPathfinding)
    {
        // start is current sprite Location
        this.start = HexTileMap.GetTileAtCoord(start);
        // goal is sprite destination eg tile user clicked on
        this.goal = HexTileMap.GetTileAtCoord(goal);

        // add the cross product of the start to goal and tile to goal vectors
        // Vector3 startToGoalV = Vector3.Cross(start.vector3,goal.vector3);
        // Location startToGoal = new Location(startToGoalV);
        // Vector3 neighborToGoalV = Vector3.Cross(neighbor.vector3,goal.vector3);

        // frontier is a List of key-value pairs:
        // Location, (float) priority
        var frontier = new PriorityQueue<Tile>();
        // Add the starting location to the frontier with a priority of 0
        frontier.Enqueue(HexTileMap.GetTileAtCoord(start), 0f);

        cameFrom.Add(HexTileMap.GetTileAtCoord(start), HexTileMap.GetTileAtCoord(start)); // is set to start, None in example
        costSoFar.Add(HexTileMap.GetTileAtCoord(start), 0f);

        while (frontier.Count > 0f)
        {
            // Get the Location from the frontier that has the lowest
            // priority, then remove that Location from the frontier
            Tile current = frontier.Dequeue();

            // If we're at the goal Location, stop looking.
            if (current.Equals(HexTileMap.GetTileAtCoord(goal))) break;

            // Neighbors will return a List of valid tile Locations
            // that are next to, diagonal to, above or below current
            
            foreach (var neighbor in (includeUnpathableTiles)?current.Neighbors(true):current.Neighbors(false))
            {

                // If neighbor is diagonal to current, graph.Cost(current,neighbor)
                // will return Sqrt(2). Otherwise it will return only the cost of
                // the neighbor, which depends on its type, as set in the TileType enum.
                // So if this is a normal floor tile (1) and it's neighbor is an
                // adjacent (not diagonal) floor tile (1), newCost will be 2,
                // or if the neighbor is diagonal, 1+Sqrt(2). And that will be the
                // value assigned to costSoFar[neighbor] below.
                float newCost = costSoFar[current] + Cost(current, neighbor);

                // If there's no cost assigned to the neighbor yet, or if the new
                // cost is lower than the assigned one, add newCost for this neighbor
                if (!costSoFar.ContainsKey(neighbor) || newCost < costSoFar[neighbor])
                {

                    // If we're replacing the previous cost, remove it
                    if (costSoFar.ContainsKey(neighbor))
                    {
                        costSoFar.Remove(neighbor);
                        cameFrom.Remove(neighbor);
                    }

                    if(showPathfinding)
                        ShowPathfind(current);

                    costSoFar.Add(neighbor, newCost);
                    cameFrom.Add(neighbor, current);
                    float priority = newCost + Heuristic(neighbor, HexTileMap.GetTileAtCoord(goal));
                    frontier.Enqueue(neighbor, priority);
                }
            }
        }

    }

    // Return a List of Locations representing the found path
    public List<Tile> FindPath()
    {
        List<Tile> path = new List<Tile>();
        Tile current = goal;
        // path.Add(current);

        while (!current.Equals(start))
        {
            if (!cameFrom.ContainsKey(current))
            {
                MonoBehaviour.print("Goal tile " + current.q + " " + current.r + " " + current.s + " could not be reached by A Star Path.");
                return new List<Tile>();
            }
            path.Add(current);
            //current.tileGO.GetComponent<TileHighlight>().Highlight(Color.yellow);
            current = cameFrom[current];
        }
        // path.Add(start);
        path.Reverse();
        return path;
    }
}

