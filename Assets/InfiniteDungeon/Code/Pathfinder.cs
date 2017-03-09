using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinder
{
    static List<Cell> RetracePath(Cell start, Cell end)
    {
        List<Cell> retracedPath = new List<Cell>();
        Cell currentNode = end;

        while (!currentNode.IsCell(start))
        {
            retracedPath.Add(currentNode);
            currentNode = currentNode.parent;
        }

        retracedPath.Reverse();

        return retracedPath;
    }

    static Map map;
    public static Cell[] FindPath(Cell target, Cell start)
    {
        if (map == null) { map = Map.FindMap(); }
        if (target.type != 0) { return null; }


        List<Cell> openSet = new List<Cell>();
        List<Cell> closedSet = new List<Cell>();

        openSet.Add(start);

        while(openSet.Count > 0)
        {
            Cell currentCell = openSet[0];

            for(int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost <  currentCell.fCost || 
                    openSet[i].fCost == currentCell.fCost && 
                    openSet[i].hCost <  currentCell.hCost)
                { currentCell = openSet[i]; }
            }

            openSet.Remove(currentCell);
            closedSet.Add(currentCell);

            if (currentCell == target)
            {
                break;
                //return RetracePath(start, target).ToArray();
            }

            foreach (Cell neighbor in map.Neighbors(currentCell))
            {
                if (neighbor.type != 0 || closedSet.Contains(neighbor) || neighbor.occupant == 1)
                {
                    continue;
                }

                int movementCost = currentCell.gCost + neighbor.Distance(currentCell);
                if (movementCost < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = movementCost;
                    neighbor.hCost = neighbor.Distance(target);
                    neighbor.parent = currentCell;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }


        }

        //return;
        return RetracePath(start, target).ToArray();
    }
}
