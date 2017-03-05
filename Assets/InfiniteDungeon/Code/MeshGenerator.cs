using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator 
{
	public static void EvaluateQuad(int x, int y, int xMax, int yMax, float t)
    {

    }

    public static GameObject GenerateCube(Vector3 pos, int neighbors, int size, int type, Color c1, Color c2)
    {
        GameObject go = new GameObject("Cube");
        Mesh mesh = GenerateMesh(size,type, c1, c2);
        go.transform.localPosition = pos;
        if (type > 2) { go.transform.localRotation = RNG.Qf(Vector3.up, 5 + neighbors); }

        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        MeshFilter mf = go.AddComponent<MeshFilter>();
        mr.material = Resources.Load<Material>("Materials/Grass");
        mf.sharedMesh = mesh;

        return go;
    }

    static Mesh GenerateMesh(int size, int type, Color c1, Color c2)
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
                //slight down
                /*
                o0 = new Vector3(0.0f, -0.05f, 0.0f);
                o1 = new Vector3(0.0f, -0.05f, 0.0f);
                o2 = new Vector3(0.0f, -0.05f, 0.0f);
                o3 = new Vector3(0.0f, -0.05f, 0.0f);
                */
                break;

            case 3:
                //slight lean left
                o0 = new Vector3(0.0f, -0.05f, 0.0f);
                o1 = new Vector3(0.0f,  0.0f, 0.0f);
                o2 = new Vector3(0.0f,  0.0f, 0.0f);
                o3 = new Vector3(0.0f, -0.05f, 0.0f);
                break;

            case 4:
                //slight lean right
                o0 = new Vector3(0.0f,  0.0f, 0.0f);
                o1 = new Vector3(0.0f, -0.05f, 0.0f);
                o2 = new Vector3(0.0f, -0.05f, 0.0f);
                o3 = new Vector3(0.0f,  0.0f, 0.0f);
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
	        c1, c1, c1, c1
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
