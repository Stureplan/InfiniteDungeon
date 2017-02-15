using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinder
{
	//void Update ()
   // {
     //   if (Input.GetKeyDown(KeyCode.Space))
       // {
            /*int x1 = Random.Range(0, map.sizeX);
            int y1 = Random.Range(0, map.sizeY);

            int x2 = Random.Range(0, map.sizeY);
            int y2 = Random.Range(0, map.sizeY);
            */
            //path = FindPath(map.CellAt(x1, y1), map.CellAt(x2, y2));

            //debugPos1 = map.PositionAt(x1, y1);
            //debugPos2 = map.PositionAt(x2, y2);
      //  }
	//}
    //path = FindPath(map.CellAtPoint(0, 0), tCell);

    static List<Cell> RetracePath(Cell start, Cell end)
    {
        List<Cell> retracedPath = new List<Cell>();
        Cell currentNode = end;

        while (currentNode != start)
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
                return RetracePath(start, target).ToArray();
            }

            foreach (Cell neighbor in map.Neighbors(currentCell))
            {
                if (neighbor.type != 0 || closedSet.Contains(neighbor))
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
