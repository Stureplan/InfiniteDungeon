using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Miner
{
    public int x;
    public int y;
    bool active;
    Map map;
    public int debug = 0;

    public Miner(int _x, int _y, Map _map)
    {
        x = _x;
        y = _y;
        map = _map;

        active = true;
    }

    public void Mine(bool settler)
    {
        debug++;
        // We've already done our duty.
        if (!active && !settler) { return; }

        int nX = x;
        int nY = y;

        RNGDir(ref nX, ref nY);

        if (map.CellIsEdge(nX, nY))
        {
            active = false;
            return;
        }

        x = nX;
        y = nY;

        map.SetCell(x, y, 0);
    }

    public static void RNGDir(ref int x, ref int y)
    {
        int r = RNG.Range(0, 4);
        if (r == 0)
        {
            //L
            x -= 1;
        }
        if (r == 1)
        {
            y += 1;
        }
        if (r == 2)
        {
            x += 1;
        }
        if (r == 3)
        {
            y -= 1;
        }
    }
}

public class Map : MonoBehaviour
{
    [Range(4, 32)]
    public int sizeX;

    [Range(4, 32)]
    public int sizeY;

    public float space;
    public int minerCount;
    public int cleanupIterations;
    public int seed;
    public bool randomSeed = false;

    Cell[,] grid;
    public GameObject cellPrefab;
    public GameObject obstaclePrefab;
    public GameObject startPrefab;
    public GameObject endPrefab;


    Miner settler;
    List<Miner> miners;
    List<GameObject> allSceneObjects;
    //Miner m01, m02, m03...

    public static Map map;
    public static void Recreate()
    {
        if (map == null)
        {
            map = (Map)FindObjectOfType(typeof(Map));
        }

        map.Generate(true);
    }

    public void CleanAndPrepare()
    {
        for (int i = 0; i < allSceneObjects.Count; i++)
        {
            DestroyImmediate(allSceneObjects[i]);
        }
    }

	public void Generate(bool gridHasChanged)
    {
        // 1. Create grid if not created
        // 2. Create critical path start/end
        // 3. Pick miner spots and mine
        // 4. Cleanup
        // N. Spawn Prefabs

        
        if (randomSeed) { seed = Random.Range(0, 9999); }
        RNG.Init(seed);


        if (gridHasChanged)
        {
            // Allocates grid and sets Cells to empty (0)
            CleanAndPrepare();
            CreateArea();
        }

        CreateMiners(minerCount);
        Mine(minerCount);

        CleanupCrew(cleanupIterations);
        CreateCriticalPath();

        SpawnPrefabs();
    }

	void CreateArea()
    {
        grid = new Cell[sizeX, sizeY];
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                Vector3 pos = new Vector3((x - sizeX / 2) * space, 0, (y - sizeY / 2) * space);
                grid[x, y] = new Cell(pos, 1, x, y);
            }
        }
    }

    void CreateMiners(int amount)
    {
        int midX = sizeX / 2;
        int midY = sizeY / 2;
        SetCell(midX, midY, 0);

        settler = new Miner(midX, midY, this);
        miners = new List<Miner>();

        for (int i = 0; i < amount; i++)
        {
            miners.Add(new Miner(settler.x, settler.y, this));
        }
    }

    void Mine(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            settler.Mine(true);

            for (int j = 0; j < miners.Count; j++)
            {
                miners[j].Mine(false);
            }
        }
    }

    void CleanupCrew(int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if (!CellIsEdge(x, y) &&  Obstacles(x, y) < 2)
                    {
                        // All Cells around (in a plus) were empty.
                        // We clean this Cell up.
                        SetCell(x, y, 0);
                    }
                }
            }
        }
    }

    void CreateCriticalPath()
    {
        for (int y = 0; y < sizeY; y++)
        {
            if (y != 0 && y != sizeY - 1)
            {
                // This creates a 3 square line around the Critical Path.
                //SetHorizontalNeighbors(sizeX / 2, y, 0);
            }

            // A straight line (Critical Path)
            SetCell(sizeX / 2, y, 0);
        }

        SetCell(sizeX / 2, 0, 4);
        SetCell(sizeX / 2, sizeY-1, 5);
    }

    void SpawnPrefabs()
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                if (grid[x, y].type == 0)
                {
                    // EMPTY SPACE (ROAD)

                    Vector3 pos = new Vector3((x - sizeX / 2) * space, 0, (y - sizeY / 2) * space);
                    GameObject c = Instantiate(cellPrefab, pos, Quaternion.identity);
                    c.transform.parent = transform;

                    allSceneObjects.Add(c);
                }
                if (grid[x, y].type == 1)
                {
                    // OBSTACLE I

                    //Vector3 pos = new Vector3((x - sizeX / 2) * space, 0, (y - sizeY / 2) * space);
                    //GameObject c = Instantiate(obstaclePrefab, pos, Quaternion.identity);
                    //c.transform.parent = transform;

                    //allSceneObjects.Add(c);
                }
                if (grid[x, y].type == 2)
                {
                    // OBSTACLE II

                    //Vector3 pos = new Vector3((x - sizeX / 2) * space, 0, (y - sizeY / 2) * space);
                    //GameObject c = Instantiate(obstaclePrefab, pos, Quaternion.identity);
                    //c.transform.parent = transform;

                    //allSceneObjects.Add(c);
                }
                if (grid[x, y].type == 3)
                {
                    // OBSTACLE III

                    //Vector3 pos = new Vector3((x - sizeX / 2) * space, 0, (y - sizeY / 2) * space);
                    //GameObject c = Instantiate(obstaclePrefab, pos, Quaternion.identity);
                    //c.transform.parent = transform;

                    //allSceneObjects.Add(c);
                }
                if (grid[x, y].type == 4)
                {
                    // START

                    Vector3 pos = new Vector3((x - sizeX / 2) * space, 0, (y - sizeY / 2) * space);
                    GameObject c = Instantiate(startPrefab, pos, Quaternion.identity);
                    c.transform.parent = transform;

                    allSceneObjects.Add(c);
                }
                if (grid[x, y].type == 5)
                {
                    // END

                    Vector3 pos = new Vector3((x - sizeX / 2) * space, 0, (y - sizeY / 2) * space);
                    GameObject c = Instantiate(endPrefab, pos, Quaternion.identity);
                    c.transform.parent = transform;

                    allSceneObjects.Add(c);
                }
            }
        }
    }

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

    public int TypeAtPoint(int x, int y)
    {
        return grid[x, y].type;
    }

    public bool CellIsEdge(int x, int y)
    {
        if (x <= 0       || 
            x >= sizeX-1 || 
            y <= 0       || 
            y >= sizeY-1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int Obstacles(int x, int y)
    {
        int x1, x2, y1, y2;

        x1 = x - 1;
        x2 = x + 1;

        y1 = y - 1;
        y2 = y + 1;

        int amount = 0;
        if (grid[x1, y].type == 1) { amount++; }
        if (grid[x2, y].type == 1) { amount++; }
        if (grid[x, y1].type == 1) { amount++; }
        if (grid[x, y2].type == 1) { amount++; }

        return amount;
    }

    public int Obstacles3x3(int x, int y)
    {
        int obstacles = 0;

        for(int h = -1; h < 2; h++)
        {
            for (int v = -1; v < 2; v++)
            {
                if (grid[x + h, x + v].type == 1)
                {
                    obstacles++;
                    if (h == 0 && v == 0) { obstacles--; }
                }
            }
        }

        return obstacles;
    }

    public Cell RandomNeighbor(int x, int y)
    {
        int r = RNG.Range(0, 4);
        if (r == 0)
        {
            return grid[x - 1, y];
        }
        if(r == 1)
        {
            return grid[x, y + 1];
        }
        if (r == 2)
        {
            return grid[x + 1, y];
        }
        if (r == 3)
        {
            return grid[x, y - 1];
        }
        else
        {
            return grid[x, y];
        }
    }

    public void SetCell(int x, int y, int type)
    {
        grid[x, y].type = type;
    }

    public void SetCellNeighbors(int x, int y, int type)
    {
        for(int h = -1; h <= 1; h++)
        {
            SetCell(x + h, y, type);
        }

        for (int v = -1; v <= 1; v++)
        {
            SetCell(x, y + v, type);
        }
    }

    public void SetHorizontalNeighbors(int x, int y, int type)
    {
        for (int h = -1; h <= 1; h++)
        {
            SetCell(x + h, y, type);
        }
    }

    public void SetVerticalNeighbors(int x, int y, int type)
    {
        for (int v = -1; v <= 1; v++)
        {
            SetCell(x, y + v, type);
        }
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
        //DrawDefaultInspector();
        Map map = (Map)target;



        map.sizeX = Mathf.Clamp(EditorGUILayout.IntField("Size X", map.sizeX), 4, 32);
        map.sizeY = Mathf.Clamp(EditorGUILayout.IntField("Size Y", map.sizeY), 4, 32);
        map.space = EditorGUILayout.FloatField("Space between Cells", map.space);
        map.minerCount = EditorGUILayout.IntField("Miner Count", map.minerCount);
        map.cleanupIterations = EditorGUILayout.IntField("Cleanup Iterations", map.cleanupIterations);
        Mathf.Clamp(map.seed = EditorGUILayout.IntField("Seed", map.seed), 0, 10000);
        map.randomSeed = EditorGUILayout.Toggle("Random Seed", map.randomSeed);

        map.cellPrefab = EditorGUILayout.ObjectField("Cell Prefab", map.cellPrefab, typeof(GameObject), false) as GameObject;
        map.obstaclePrefab = EditorGUILayout.ObjectField("Obstacle Prefab", map.obstaclePrefab, typeof(GameObject), false) as GameObject;
        map.startPrefab = EditorGUILayout.ObjectField("Start Prefab", map.startPrefab, typeof(GameObject), false) as GameObject;
        map.endPrefab = EditorGUILayout.ObjectField("End Prefab", map.endPrefab, typeof(GameObject), false) as GameObject;


        if (GUILayout.Button("Generate"))
        {
            map.Generate(true);
        }
        if (GUILayout.Button("Clear"))
        {
            map.CleanAndPrepare();
        }
    }
}