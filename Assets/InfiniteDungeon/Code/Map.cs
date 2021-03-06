﻿using System.Collections;
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
    public int sizeX;
    public int sizeY;

    public int enemies;
    public float space;
    public int minerCount;
    public int cleanupIterations;
    public int seed;
    public bool randomSeed = false;

    Cell[,] grid;
    Cell[] openCells;
    int enemyAmount = 5;
    Texture2D minimap;
    public GameObject[] groundPrefabs = new GameObject[2];
    public GameObject[] mountainPrefabs = new GameObject[3];
    public GameObject startPrefab;
    public GameObject endPrefab;
    public GameObject slimePrefab;
    public GameObject warlockPrefab;
    public GameObject cauldronPrefab;


    Miner settler;
    List<Miner> miners;
    List<GameObject> allSceneObjects = new List<GameObject>();
    List<Agent> allSceneEnemies = new List<Agent>();

    //Miner m01, m02, m03...
    const float BOTTOM_HEIGHT = -10.0f;
    public static Map map;
    public static Map FindMap()
    {
        if (map == null)
        {
            map = (Map)FindObjectOfType(typeof(Map));
        }

        return map;
    }

    private void Awake()
    {
        //Generate(true);
    }

    public void CleanAndPrepare()
    {
        for (int i = 0; i < allSceneObjects.Count; i++)
        {
            DestroyImmediate(allSceneObjects[i]);
        }

        Transform[] children = GetComponentsInChildren<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i != 0)
            {
                DestroyImmediate(children[i].gameObject);
                //Destroy(children[i].gameObject);
            }
        }

        openCells = new Cell[0];
    }

	public void Generate()
    {
        // 0. Move map down
        // 1. Create grid if not created
        // 2. Create critical path start/end
        // 3. Pick miner spots and mine
        // 4. Cleanup
        // N. Spawn Prefabs
        transform.localPosition = new Vector3(0, BOTTOM_HEIGHT, 0);


        if (randomSeed) { seed = Random.Range(0, 9999); }
        RNG.Init(seed);


        // Allocates grid and sets Cells to empty (0)
        CleanAndPrepare();
        CreateArea();

        CreateMiners(minerCount);
        Mine(minerCount);

        CleanupCrew(cleanupIterations);
        CreateCriticalPath();

        //SetValues();
        
        SpawnPrefabs();
        SpawnEnemies();

        CreateMinimap();
    }

    public void AnimateMap()
    {
        StartCoroutine(MoveMap(1.0f));
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
                    if (!CellIsEdge(x, y))
                    {
                        if (Obstacles(x, y) < 2)
                        {
                            // All Cells around (in a plus) were empty.
                            // We clean this Cell up.
                            SetCell(x, y, 0);
                        }

                        if (Obstacles3x3(x, y, 1) > 4 && Obstacles3x3(x, y, 0) > 2 && grid[x, y].type == 1)
                        {
                            //SetCell(x, y, 2);
                        }
                    }
                }
            }
        }

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                if (!CellIsEdge(x, y))
                {
                    /*if (Obstacles3x3(x, y, 2) > 1 && grid[x, y].type == 2)
                    {
                        SetCell(x, y, 1);
                    }*/

                    if (Obstacles3x3(x, y, 0) > 4 && grid[x, y].type == 1)
                    {
                        //We are probably in a corner, so a mountain looks nice.
                        SetCell(x, y, 2);
                    }

                    if (Obstacles3x3(x, y, 2) > 2)
                    {
                        //If we're near 3 or more mountains, make this a mountain range.
                        SetCell(x, y, 2);
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

        SetCell(sizeX / 2, 0, 0);
        SetCell(sizeX / 2, sizeY-1, 0);
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
                    grid[x, y].occupant = 0;
                    Vector3 pos = new Vector3((x - sizeX / 2) * space, 0, (y - sizeY / 2) * space);
                    //GameObject c = Instantiate(groundPrefabs[Random.Range(0, 2)], pos, Quaternion.identity);

                    int neighbors = 0;
                    if (!CellIsEdge(x, y)) { neighbors = Obstacles3x3(x, y, 1); }
                    else { neighbors = 10; }
                    GameObject c = TileMaker.GrassTile(grid[x,y],pos, space);
                    c.transform.SetParent(transform, false);

                    allSceneObjects.Add(c);
                    ArrayUtility.Add(ref openCells, grid[x,y]);
                }
                if (grid[x, y].type == 1)
                {
                    // OBSTACLE I (EMPTY)
                    grid[x, y].occupant = 0;

                    int r = Random.Range(0, 10);
                    if (r < 4)
                    {
                        Vector3 pos = new Vector3((x - sizeX / 2) * space, 0, (y - sizeY / 2) * space);
                        GameObject c = TileMaker.ObstacleTile(grid[x, y], pos, 1.0f);
                        c.transform.SetParent(transform, false);

                        allSceneObjects.Add(c);
                    }
                }
                if (grid[x, y].type == 2)
                {
                    // OBSTACLE II (MOUNTAINS)
                    grid[x, y].occupant = 0;

                    Vector3 pos = new Vector3((x - sizeX / 2) * space, 0, (y - sizeY / 2) * space);
                    GameObject c = Instantiate(mountainPrefabs[RNG.Range(0, 3)], pos, RNG.Q90f(Vector3.up));
                    c.transform.SetParent(transform, false);


                    allSceneObjects.Add(c);
                }
                if (grid[x, y].type == 3)
                {
                    // OBSTACLE III
                    grid[x, y].occupant = 0;

                    //Vector3 pos = new Vector3((x - sizeX / 2) * space, 0, (y - sizeY / 2) * space);
                    //GameObject c = Instantiate(obstaclePrefabIII, pos, Quaternion.identity);
                    //c.transform.parent = transform;

                    //allSceneObjects.Add(c);
                }
                if (grid[x, y].type == 4)
                {
                    // START
                    grid[x, y].occupant = 2;

                    Vector3 pos = new Vector3((x - sizeX / 2) * space, 0, (y - sizeY / 2) * space);
                    GameObject c = Instantiate(startPrefab, pos, Quaternion.identity);
                    c.transform.SetParent(transform, false);


                    allSceneObjects.Add(c);
                }
                if (grid[x, y].type == 5)
                {
                    // END
                    grid[x, y].occupant = 0;

                    Vector3 pos = new Vector3((x - sizeX / 2) * space, 0, (y - sizeY / 2) * space);
                    GameObject c = Instantiate(endPrefab, pos, Quaternion.identity);
                    c.transform.SetParent(transform, false);


                    allSceneObjects.Add(c);
                }
            }
        }

    }

    void SpawnEnemies()
    {
        //int x, y;
        //x = sizeX / 2; y = sizeY - 2;

        //Instnatitae..
        /*Vector3 pos = new Vector3((x - sizeX / 2) * space, 0, (y - sizeY / 2) * space);
        GameObject go = Instantiate(slimePrefab, pos, Quaternion.identity);
        Agent a = go.AddComponent<Slime>();
        a.SetupEnemy(grid[x, y]);

        allSceneEnemies.Add(a);*/

        int x = sizeX / 2;
        int y = sizeY / 2;

        SpawnEnemy<Cauldron>(cauldronPrefab, grid[x,y]);
        SpawnEnemy<Warlock>(warlockPrefab, grid[x - 1, y]);
        SpawnEnemy<Warlock>(warlockPrefab, grid[x + 1, y]);

        SpawnEnemy<Warlock>(warlockPrefab, grid[x, y - 1]);
        SpawnEnemy<Warlock>(warlockPrefab, grid[x, y - 2]);


        for (int i = 0; i < enemyAmount; i++)
        {
            /*int r = RNG.Range(0, openCells.Length-1);

            if (i == 0)
            {
                SpawnEnemy<Cauldron>(cauldronPrefab, openCells[r]);
            }
            else
            {
                SpawnEnemy<Warlock>(warlockPrefab, openCells[r]);
            }

            ArrayUtility.RemoveAt(ref openCells, r);*/
            //TODO: Implement this instead: System.Array.Resize<Cell>(ref openCells, 1 + 1); (ArrayUtility is an EDITOR class.. nowherein the Build)
            
            /*
             public static void RemoveAt<T>(ref T[] arr, int index)
             {
                 for (int a = index; a < arr.Length - 1; a++)
                 {
                     // moving elements downwards, to fill the gap at [index]
                     arr[a] = arr[a + 1];
                 }
                 // finally, let's decrement Array's size by one
                 Array.Resize(ref arr, arr.Length - 1);
             }
             RemoveAt(ref colors, 2); // removes Color.white.
             */
        }
    }

    public void SpawnEnemy<T>(GameObject prefab, Cell c)
    {
        GameObject go = Instantiate(prefab, c.position, Quaternion.identity);
        Agent a = (Agent)go.AddComponent(typeof(T));
        a.SetupEnemy(c);

        allSceneEnemies.Add(a);
        ArrayUtility.Remove(ref openCells, c);
    }

    public Slime SpawnSlime(Cell cauldron, Cell target)
    {
        GameObject go = Instantiate(slimePrefab, cauldron.position, Quaternion.identity);
        Slime s = (Slime)go.AddComponent(typeof(Slime));
        s.SetupEnemy(target);
        s.CauldronVisualSpawn(target);

        allSceneEnemies.Add(s);
        ArrayUtility.Remove(ref openCells, target);
        return s;
    }

    public Cell StartCell()
    {
        return grid[sizeX / 2, 0];
    }

    public Cell EndCell()
    {
        return grid[sizeX / 2, sizeY - 1];
    }

    public Cell CellAtPoint(int x, int y)
    {
        if (x < 0 || x > sizeX-1 || y < 0 || y > sizeY-1)
        {
            return Cell.NoCell();
        }
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

    public bool CellIsSafe(int x, int y)
    {
        if (x < 0 || x > sizeX - 1 || y < 0 || y > sizeY - 1)
        {
            return false;
        }

        return true;
    }

    public int CellSafeCheck(int x, int y)
    {
        if (x < 0 || x > sizeX - 1 || y < 0 || y > sizeY - 1)
        {
            return 666;
        }

        return 1;
    }

    public int CellSafeCheck(Cell c)
    {
        if (c.x < 0 || c.x > sizeX - 1 || c.y < 0 || c.y > sizeY - 1)
        {
            return 666;
        }

        return 1;
    }

    public static int CellDistance(int x1, int y1, int x2, int y2)
    {
        // Naive Manhattan distance (bird mode)
        int d = (x2 - x1) + (y2 - y1);

        return Mathf.Abs(d);
    }

    public static float Distance(Cell c1, Cell c2)
    {
        return Mathf.Abs(Vector3.Distance(c1.position, c2.position));
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

    public int Obstacles3x3(int x, int y, int type)
    {
        int obstacles = 0;

        for(int h = -1; h < 2; h++)
        {
            for (int v = -1; v < 2; v++)
            {
                if (grid[x + h, y + v].type == type)
                {
                    obstacles++;
                    if (h == 0 && v == 0) { obstacles--; }
                }
            }
        }

        return obstacles;
    }

    public int[] SurroundingOccupants(int x, int y)
    {
        int[] o = new int[4];
        o[0] = 666;
        o[1] = 666;
        o[2] = 666;
        o[3] = 666;


        if (CellSafeCheck(x - 1, y) != 666) { o[0] = grid[x - 1, y].occupant; }
        if (CellSafeCheck(x, y + 1) != 666) { o[1] = grid[x, y + 1].occupant; }
        if (CellSafeCheck(x + 1, y) != 666) { o[2] = grid[x + 1, y].occupant; }
        if (CellSafeCheck(x, y - 1) != 666) { o[3] = grid[x, y - 1].occupant; }

        return o;
    }

    public Agent[] Agents3x3(int x, int y)
    {
        List<Agent> agents = new List<Agent>();

        for (int h = -1; h < 2; h++)
        {
            for (int v = -1; v < 2; v++)
            {
                if (grid[x + h, y + v].occupant == 1)
                {
                    agents.Add(grid[x + h, y + v].enemy);
                }
            }
        }

        return agents.ToArray();
    }

    public static bool CellsAreAlignedH(Cell[] c, int x)
    {
        int amount = c.Length;
        int aligned = 0;

        for (int i = 0; i < amount; i++)
        {
            if (c[i].x == x && c[i].occupant != 1)
            {
                aligned++;
            }
        }

        if (amount == aligned)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool CellsAreAlignedV(Cell[] c, int y)
    {
        int amount = c.Length;
        int aligned = 0;

        for (int i = 0; i < amount; i++)
        {
            if (c[i].y == y && c[i].occupant != 1)
            {
                aligned++;
            }
        }

        if (amount == aligned)
        {
            return true;
        }
        else
        {
            return false;
        }
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

    public List<Agent> Enemies()
    {
        return allSceneEnemies;
    }

    public Cell FirstFreeNeighbor(int x, int y)
    {
        Cell temp;

        temp = grid[x - 1, y];
        if (CellSafeCheck(temp) != 666 && temp.type == 0 && temp.occupant == 0)
        {
            return temp;
        }

        temp = grid[x + 1, y];
        if (CellSafeCheck(temp) != 666 && temp.type == 0 && temp.occupant == 0)
        {
            return temp;
        }

        temp = grid[x, y - 1];
        if (CellSafeCheck(temp) != 666 && temp.type == 0 && temp.occupant == 0)
        {
            return temp;
        }

        temp = grid[x, y + 1];
        if (CellSafeCheck(temp) != 666 && temp.type == 0 && temp.occupant == 0)
        {
            return temp;
        }


        return Cell.NoCell();
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

    public T GetComponentAtPosition<T>(Vector3 position, T component)
    {
        int objects = allSceneObjects.Count;
        for (int i = 0; i < objects; i++)
        {
            float distance = Vector3.Distance(position, allSceneObjects[i].transform.position);
            if (Mathf.Abs(distance) < 0.1f)
            {
                return allSceneObjects[i].GetComponent<T>();
            }

        }

        return default(T);
    }

    public void CreateMinimap()
    {
        minimap = new Texture2D(sizeX, sizeY);
        Color obstacle = Color.clear;
        Color empty = new Color(1, 1, 1, 0.25f);
        Color start = new Color(0, 0, 1, 0.25f);
        Color end = new Color(1, 0, 0, 0.25f);



        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                Cell c = grid[x, y];

                if (c.type == 0)
                {
                    minimap.SetPixel(x, y, empty);
                }

                else if (c.type == 1)
                {
                    minimap.SetPixel(x, y, obstacle);
                }

                else if (c.type == 2)
                {
                    minimap.SetPixel(x, y, obstacle);
                }

                else if (c.type == 4)
                {
                    minimap.SetPixel(x, y, start);
                }

                else if (c.type == 5)
                {
                    minimap.SetPixel(x, y, end);
                }
            }
        }

        minimap.name = "Minimap";
        minimap.filterMode = FilterMode.Point;
        minimap.wrapMode = TextureWrapMode.Clamp;
        minimap.Apply();
    }

    public Texture2D GetMinimap()
    {
        if (minimap == null) { CreateMinimap(); }
        return minimap;
    }

    private IEnumerator MoveMap(float time)
    {
        Vector3 end = Vector3.zero;
        float t = 0.0f;
        float d = Vector3.Distance(transform.localPosition, end);

        while (t < time)
        {
            float delta = (d * Time.deltaTime) / time;

            transform.localPosition = Vector3.MoveTowards(transform.localPosition, end,delta);


            t += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = end;
        Barbarian.FindBarbarian().StartLevel();
    }
}

[ExecuteInEditMode]
[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
    Map map;

    public void OnEnable()
    {
        map = (Map)target;
    }

    public override void OnInspectorGUI()
    {
        //DrawDefaultInspector();

        map.sizeX = Mathf.Clamp(EditorGUILayout.IntField("Size X", map.sizeX), 4, 32);
        map.sizeY = Mathf.Clamp(EditorGUILayout.IntField("Size Y", map.sizeY), 4, 32);
        map.enemies = EditorGUILayout.IntField("Enemy Count", map.enemies);
        map.space = EditorGUILayout.FloatField("Space between Cells", map.space);
        map.minerCount = EditorGUILayout.IntField("Miner Count", map.minerCount);
        map.cleanupIterations = EditorGUILayout.IntField("Cleanup Iterations", map.cleanupIterations);
        Mathf.Clamp(map.seed = EditorGUILayout.IntField("Seed", map.seed), 0, 10000);
        map.randomSeed = EditorGUILayout.Toggle("Random Seed", map.randomSeed);


        // Regular ground
        map.groundPrefabs[0] = EditorGUILayout.ObjectField("Cell Prefab 1", map.groundPrefabs[0], typeof(GameObject), false) as GameObject;
        map.groundPrefabs[1] = EditorGUILayout.ObjectField("Cell Prefab 1", map.groundPrefabs[1], typeof(GameObject), false) as GameObject;


        // Obstacles
        map.mountainPrefabs[0] = EditorGUILayout.ObjectField("Mountain 1", map.mountainPrefabs[0], typeof(GameObject), false) as GameObject;
        map.mountainPrefabs[1] = EditorGUILayout.ObjectField("Mountain 2", map.mountainPrefabs[1], typeof(GameObject), false) as GameObject;
        map.mountainPrefabs[2] = EditorGUILayout.ObjectField("Mountain 3", map.mountainPrefabs[2], typeof(GameObject), false) as GameObject;
        
        // Specials
        map.startPrefab = EditorGUILayout.ObjectField("Start Prefab", map.startPrefab, typeof(GameObject), false) as GameObject;
        map.endPrefab = EditorGUILayout.ObjectField("End Prefab", map.endPrefab, typeof(GameObject), false) as GameObject;

        // Enemies
        map.slimePrefab = EditorGUILayout.ObjectField("Slime Prefab", map.slimePrefab, typeof(GameObject), false) as GameObject;
        map.warlockPrefab = EditorGUILayout.ObjectField("Warlock Prefab", map.warlockPrefab, typeof(GameObject), false) as GameObject;
        map.cauldronPrefab = EditorGUILayout.ObjectField("Cauldron Prefab", map.cauldronPrefab, typeof(GameObject), false) as GameObject;


        if (GUILayout.Button("Generate"))
        {
            map.Generate();
        }
        if (GUILayout.Button("Clear"))
        {
            map.CleanAndPrepare();
        }
    }
}