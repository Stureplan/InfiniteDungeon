using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TileMaker 
{
    private static readonly Color[] GRASS_COLORS =
    {
        new Color(0.45f, 0.47f, 0.26f, 1.0f),
        new Color(0.50f, 0.53f, 0.27f, 1.0f)
    };

    private static readonly Color GRASS_1 = new Color(0.45f, 0.47f, 0.26f, 1.0f);
    private static readonly Color GRASS_2 = new Color(0.50f, 0.53f, 0.27f, 1.0f);
    private static readonly Color DIRT    = new Color(0.30f, 0.21f, 0.12f, 1.0f);

    
    private static Mesh[] GRASS_VARIATIONS;
    private static Mesh GrassVariation(float size)
    {
        if (GRASS_VARIATIONS == null)
        {
            int amt = GRASS_COLORS.Length;
            GRASS_VARIATIONS = new Mesh[amt];

            for (int i = 0; i < amt; i++)
            {
                GRASS_VARIATIONS[i] = GrassMesh(GRASS_COLORS[i], size);
            }
        }

        int r = Random.Range(0, GRASS_VARIATIONS.Length);

        return GRASS_VARIATIONS[r];
    }

    private static Material GRASS_MATERIAL;
    private static Material GrassMaterial()
    {
        if (GRASS_MATERIAL == null)
        {
            GRASS_MATERIAL = Resources.Load<Material>("Materials/Grass");
        }

        return GRASS_MATERIAL;
    }

    private static GameObject[] PROPS;
    private static GameObject RandomProp()
    {
        if (PROPS == null)
        {
            PROPS = Resources.LoadAll<GameObject>("Props");
        }

        int r = RNG.Range(0, PROPS.Length);

        return PROPS[r];
    }

    private static AnimationClip[] ANIMATIONS;
    private static AnimationClip RandomAnimation()
    {
        if (ANIMATIONS == null)
        {
            ANIMATIONS = Resources.LoadAll<AnimationClip>("Animations");
        }

        int r = RNG.Range(0, ANIMATIONS.Length);

        return ANIMATIONS[r];
    }

    public static GameObject GrassTile(Cell c, Vector3 pos, float size)
    {
        GameObject go = new GameObject("Cube");
        Mesh mesh = GrassVariation(size);
        go.transform.localPosition = pos;


        BoxCollider bc = go.AddComponent<BoxCollider>();
        bc.center -= Game.HALF_Y;
        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        MeshFilter mf = go.AddComponent<MeshFilter>();
        mr.material = GrassMaterial();
        mf.sharedMesh = mesh;

        return go;
    }

    public static GameObject ObstacleTile(Cell c, Vector3 pos, float size)
    {
        GameObject empty = new GameObject("Obstacle");
        GameObject model = new GameObject("Model");
        model.transform.SetParent(empty.transform, false);

        Mesh mesh = GrassVariation(size);
        empty.transform.localPosition = pos + Game.HALF_Y;

        Animation a = model.AddComponent<Animation>();
        a.AddClip(RandomAnimation(), "Hover");
        a.Play("Hover");

        // Place prop
        GameObject prop = GameObject.Instantiate(RandomProp());
        prop.transform.SetParent(model.transform, false);
        prop.transform.localPosition += RNG.VXZf(-size/4, size/4);
        prop.transform.localRotation = RNG.Q90f(Vector3.up);

        MeshRenderer mr = model.AddComponent<MeshRenderer>();
        MeshFilter mf = model.AddComponent<MeshFilter>();
        mr.material = GrassMaterial();
        mf.sharedMesh = mesh;

        return empty;
    }

    static Mesh GrassMesh(Color GRASS, float size)
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
    
}
