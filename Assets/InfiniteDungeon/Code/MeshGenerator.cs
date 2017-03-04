using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator 
{
	public static void EvaluateQuad(int x, int y, int xMax, int yMax, float t)
    {

    }

    public static GameObject GenerateCube(int x, int y, int xMax, int yMax, int size, Color c)
    {
        GameObject go = new GameObject("Cube");
        Mesh mesh = GenerateMesh(size);

        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        MeshFilter mf = go.AddComponent<MeshFilter>();
        mr.material = Resources.Load<Material>("Materials/Grass");
        mf.sharedMesh = mesh;

        return go;
    }

    static Mesh GenerateMesh(int size)
    {
        Mesh mesh = new Mesh();

        mesh.name = "TerrainCube";
        mesh.Clear();

        #region Vertices
        float offset = (float)size / 2;

        Vector3 p0 = new Vector3(-size * .5f, -size * .5f - offset,  size * .5f);
        Vector3 p1 = new Vector3( size * .5f, -size * .5f - offset,  size * .5f);
        Vector3 p2 = new Vector3( size * .5f, -size * .5f - offset, -size * .5f);
        Vector3 p3 = new Vector3(-size * .5f, -size * .5f - offset, -size * .5f);

        Vector3 p4 = new Vector3(-size * .5f,  size * .5f - offset,  size * .5f);
        Vector3 p5 = new Vector3( size * .5f,  size * .5f - offset,  size * .5f);
        Vector3 p6 = new Vector3( size * .5f,  size * .5f - offset, -size * .5f);
        Vector3 p7 = new Vector3(-size * .5f,  size * .5f - offset, -size * .5f);

        Vector3[] vertices = new Vector3[]
        {
	        // Bottom
	        p0, p1, p2, p3,
 
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
	        // Bottom
	        3, 1, 0,
            3, 2, 1,			
 
	        // Left
	        3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
            3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
 
	        // Front
	        3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
            3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
 
	        // Back
	        3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
            3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
 
	        // Right
	        3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
            3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
 
	        // Top
	        3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
            3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,
        };
        #endregion

        #region Colors

        Color y = Color.yellow;
        Color g = Color.green;

        Color[] colors = new Color[24]
        {
            // Bottom
	        y, y, y, y,
 
	        // Left
	        y, y, y, y,
 
	        // Front
	        y, y, y, y,
 
	        // Back
	        y, y, y, y,
 
	        // Right
	        y, y, y, y,
 
	        // Top
	        g, g, g, g
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
