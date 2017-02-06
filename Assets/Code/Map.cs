using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class Map : MonoBehaviour
{
    public float space;
    public int sizeX;
    public int sizeY;
    public int obstacleCount;

    Cell[,] grid;

    public GameObject cellPrefab;
    public GameObject obstaclePrefab;

	void Start ()
    {
        Create();
	}

	void Create()
    {
        grid = new Cell[sizeX, sizeY];
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                Vector3 pos = new Vector3((x - sizeX / 2) * space, 0, (y - sizeY / 2) * space);

                GameObject cell = Instantiate(cellPrefab, pos, Quaternion.identity);
                grid[x, y] = new Cell(pos, 0, x, y);
            }
        }

        /*for (int i = 0; i < obstacleCount; i++)
        {
            int x = Random.Range(0, sizeX);
            int y = Random.Range(0, sizeY);

            GameObject obstacle = Instantiate(obstaclePrefab, PositionAt(x, y), Quaternion.identity);
            grid[x, y].type = 1;
        }*/
    }


    //turn everything to static later
    public Cell CellAtPoint(int x, int y)
    {
        return grid[x, y];
    }

    public Cell CellAtPosition(Vector3 position)
    {
        for(int x = 0; x < sizeX; x++)
        {
            for(int y = 0; y < sizeY; y++)
            {
                float distance = Vector3.Distance(position, grid[x, y].position);
                if (Mathf.Abs(distance) < 0.1f)
                {
                    return grid[x, y];
                }
            }
        }

        return Cell.NoCell();
    }

    public Vector3 PositionAt(int x, int y)
    {
        return grid[x, y].position;
    }

    public List<Cell> Neighbors(Cell cell)
    {
        List<Cell> neighbors = new List<Cell>();


        //two loops here instead? 
        //   O
        // O O O
        //   O



        for (int x = -1; x <= 1; x++)
        {
            int checkX = cell.x + x;
            int checkY = cell.y;

            if (x == 0) { continue; }

            if (checkX >= 0 && checkX < sizeX && checkY >= 0 && checkY < sizeY)
            {
                neighbors.Add(grid[checkX, checkY]);
            }
        }



        for (int y = -1; y <= 1; y++)
        {
            int checkX = cell.x;
            int checkY = cell.y + y;

            if (y == 0) { continue; }

            if (checkX >= 0 && checkX < sizeX && checkY >= 0 && checkY < sizeY)
            {
                neighbors.Add(grid[checkX, checkY]);
            }
        }


       /* for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int checkX = cell.x + x;
                int checkY = cell.y + y;

                if (checkX >= 0 && checkX < sizeX && checkY >= 0 && checkY < sizeY)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }
        */
        return neighbors;
    }
}

[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Map map = (Map)target;

        if (GUILayout.Button("Generate"))
        {

        }
    }
}