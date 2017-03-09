using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TileMaker 
{
    private static readonly Color GRASS_1 = new Color(0.45f, 0.47f, 0.26f, 1.0f);
    private static readonly Color GRASS_2 = new Color(0.50f, 0.53f, 0.27f, 1.0f);
    private static readonly Color DIRT    = new Color(0.30f, 0.21f, 0.12f, 1.0f);

    private static Material GRASS_MATERIAL;
    private static Material GrassMaterial()
    {
        if (GRASS_MATERIAL == null)
        {
            GRASS_MATERIAL = Resources.Load<Material>("Materials/Grass");
        }

        return GRASS_MATERIAL;
    }

    private static GameObject[] OBSTACLES;
    private static GameObject RandomObstacle()
    {
        if (OBSTACLES == null)
        {
            OBSTACLES = Resources.LoadAll<GameObject>("Obstacles");
        }

        int r = RNG.Range(0,OBSTACLES.Length);

        return OBSTACLES[r];
    }

    public static GameObject MakeObstacle(Cell c, Vector3 pos, float size, int meshChance)
    {
        //meshChance: Percent chance of this tile not being "empty" ie not having a mesh attached.
        GameObject go = new GameObject("Obstacle");
        go.transform.localPosition = pos;


        int percent = RNG.Range(1, 101);
        if (percent <= meshChance)
        {
            //hit
            GameObject obstacle = GameObject.Instantiate(RandomObstacle(), pos, Quaternion.identity);
            obstacle.transform.SetParent(go.transform, true);
            obstacle.transform.localRotation = RNG.Q90f(Vector3.up);


            Mesh mesh = GrassMesh(size);
            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            MeshFilter   mf = go.AddComponent<MeshFilter>();
            mr.material = GrassMaterial();
            mf.sharedMesh = mesh;

            return go;
        }
        else
        {
            //miss
            return go;
        }
    }

    public static GameObject MakeGrass(Cell c, Vector3 pos, int neighbors, float size, int type, Color c1, Color c2, Color c3)
    {
        GameObject go = new GameObject("Cube");
        Mesh mesh = GrassMesh(size,type, c1, c2, c3);
        go.transform.localPosition = pos;
        if (type > 2) { go.transform.localRotation = RNG.Qf(Vector3.up, neighbors*2); }

        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        MeshFilter mf = go.AddComponent<MeshFilter>();
        mr.material = Resources.Load<Material>("Materials/Grass");
        mf.sharedMesh = mesh;

        return go;
    }

    static Mesh GrassMesh(float size)
    {
        Mesh mesh = new Mesh();

        mesh.name = "GrassCube";
        mesh.Clear();

        #region Vertices

        float offset = size / 2;

        Vector3 p0 = new Vector3(-size * .5f, -size * .5f - offset, size * .5f);
        Vector3 p1 = new Vector3(size * .5f, -size * .5f - offset, size * .5f);
        Vector3 p2 = new Vector3(size * .5f, -size * .5f - offset, -size * .5f);
        Vector3 p3 = new Vector3(-size * .5f, -size * .5f - offset, -size * .5f);

        Vector3 p4 = new Vector3(-size * .5f, size * .5f - offset, size * .5f);
        Vector3 p5 = new Vector3(size * .5f, size * .5f - offset, size * .5f);
        Vector3 p6 = new Vector3(size * .5f, size * .5f - offset, -size * .5f);
        Vector3 p7 = new Vector3(-size * .5f, size * .5f - offset, -size * .5f);


        Vector3[] vertices = new Vector3[]
        {
	        // Left
	        p7, p4, p0, p3,
 
	        // Front
	        p4, p5, p1, p0,
 
	        // Back
	        p6, p7, p3, p2,
 
	        // Right
	        p5, p6, p2, p1,
 
	        // Top
	        p7, p6, p5, p4
        };
        #endregion

        #region Triangles
        int[] triangles = new int[]
        {
	        // Left
	        3, 1, 0,
            3, 2, 1,			
 
	        // Front
	        3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
            3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
 
	        // Back
	        3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
            3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
 
	        // Right
	        3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
            3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
 
	        // Top
	        3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
            3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
        };
        #endregion

        #region Colors

        Color GRASS;
        int r = Random.Range(0, 2);
        if (r == 0) { GRASS = GRASS_1; }
        else        { GRASS = GRASS_2; }
        Color[] colors = new Color[20]
        {
	        // Left
	        DIRT, DIRT, DIRT, DIRT,
 
	        // Front
	        DIRT, DIRT, DIRT, DIRT,
 
	        // Back
	        DIRT, DIRT, DIRT, DIRT,
 
	        // Right
	        DIRT, DIRT, DIRT, DIRT,
 
	        // Top
	        GRASS, GRASS, GRASS, GRASS
        };

        #endregion

        mesh.vertices = vertices;
        mesh.uv = new Vector2[vertices.Length];
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }

    static Mesh GrassMesh(float size, int type, Color c1, Color c2, Color c3)
    {
        Mesh mesh = new Mesh();

        mesh.name = "TerrainCube";
        mesh.Clear();

        #region Vertices

        Vector3 o0 = Vector3.zero; //bottom left
        Vector3 o1 = Vector3.zero;
        Vector3 o2 = Vector3.zero;
        Vector3 o3 = Vector3.zero; //bottom right
        

        switch (type)
        {
            case 0:
                //nothing
                break;

            case 1:
                //slight up
                /*
                o0 = new Vector3(0.0f, 0.05f, 0.0f);
                o1 = new Vector3(0.0f, 0.05f, 0.0f);
                o2 = new Vector3(0.0f, 0.05f, 0.0f);
                o3 = new Vector3(0.0f, 0.05f, 0.0f);
                */
                break;

            case 2:
                //slight down - TODO: This is a stone tile Cell.
                /*
                o0 = new Vector3(0.0f, -0.05f, 0.0f);
                o1 = new Vector3(0.0f, -0.05f, 0.0f);
                o2 = new Vector3(0.0f, -0.05f, 0.0f);
                o3 = new Vector3(0.0f, -0.05f, 0.0f);
                */
                break;

            case 3:
                //slight lean left
                /*
                o0 = new Vector3(0.0f, -0.05f, 0.0f);
                o1 = new Vector3(0.0f,  0.0f, 0.0f);
                o2 = new Vector3(0.0f,  0.0f, 0.0f);
                o3 = new Vector3(0.0f, -0.05f, 0.0f);
                */            
                break;
                
            case 4:
                //slight lean right
                /*
                o0 = new Vector3(0.0f,  0.0f, 0.0f);
                o1 = new Vector3(0.0f, -0.05f, 0.0f);
                o2 = new Vector3(0.0f, -0.05f, 0.0f);
                o3 = new Vector3(0.0f,  0.0f, 0.0f);
                */
                break;

        }

        float offset = ((float)size + Mathf.Max(o0.y, o1.y, o2.y, o3.y)) / 2;


        Vector3 p0 = new Vector3(-size * .5f, -size * .5f - offset,  size * .5f);
        Vector3 p1 = new Vector3( size * .5f, -size * .5f - offset,  size * .5f);
        Vector3 p2 = new Vector3( size * .5f, -size * .5f - offset, -size * .5f);
        Vector3 p3 = new Vector3(-size * .5f, -size * .5f - offset, -size * .5f);

        Vector3 p4 = new Vector3(-size * .5f,  size * .5f - offset,  size * .5f) + o3;
        Vector3 p5 = new Vector3( size * .5f,  size * .5f - offset,  size * .5f) + o2;
        Vector3 p6 = new Vector3( size * .5f,  size * .5f - offset, -size * .5f) + o1;
        Vector3 p7 = new Vector3(-size * .5f,  size * .5f - offset, -size * .5f) + o0;


        Vector3[] vertices = new Vector3[]
        {
	        // Left
	        p7, p4, p0, p3,
 
	        // Front
	        p4, p5, p1, p0,
 
	        // Back
	        p6, p7, p3, p2,
 
	        // Right
	        p5, p6, p2, p1,
 
	        // Top
	        p7, p6, p5, p4
        };
        #endregion

        #region Triangles
        int[] triangles = new int[]
        {
	        // Left
	        3, 1, 0,
            3, 2, 1,			
 
	        // Front
	        3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
            3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
 
	        // Back
	        3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
            3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
 
	        // Right
	        3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
            3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
 
	        // Top
	        3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
            3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
        };
        #endregion

        #region Colors

        Color grassColor;
        int r = Random.Range(0, 2);
        if (r == 0) { grassColor = c1; }
        else        { grassColor = c3; }
        Color[] colors = new Color[20]
        {
	        // Left
	        c2, c2, c2, c2,
 
	        // Front
	        c2, c2, c2, c2,
 
	        // Back
	        c2, c2, c2, c2,
 
	        // Right
	        c2, c2, c2, c2,
 
	        // Top
	        grassColor, grassColor, grassColor, grassColor
        };

        #endregion

        mesh.vertices = vertices;
        mesh.uv = new Vector2[vertices.Length];
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }
}
