using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class VLight : MonoBehaviour
{
    public Color color;
    public float range = 1.0f;
    public float strength = 1.0f;

    public int index;
    public int updates = 1;

    private int frame = 0;

    // Shader variables
    public string shaderPos = "GLOBAL_LIGHT_POS_";
    public string shaderCol = "GLOBAL_LIGHT_COL_";


    void Start() { }

    void Update()
    {
        if (frame % updates == 0) { UpdateShaders(); }

        frame++;
    }

    public void UpdateShaderNames()
    {
        shaderPos = "GLOBAL_LIGHT_POS_" + index;
        shaderCol = "GLOBAL_LIGHT_COL_" + index;

        UpdateShaders();
    }

    public void UpdateShaders()
    {
        Vector4 vec = transform.position; vec.w = 1.0f;
        Shader.SetGlobalVector(shaderPos, vec);
        Shader.SetGlobalVector(shaderCol, color);
    }

    private void OnDrawGizmos()
    {
        UpdateShaders();
    }
}

[ExecuteInEditMode]
[CustomEditor(typeof(VLight))]
public class VLightEditor : Editor
{
    VLight light;
    int index;

    public override void OnInspectorGUI()
    {
        FindTarget();
        index = light.index;

        base.OnInspectorGUI();
    }

    private void FindTarget()
    {
        light = (VLight)target;
    }

    private void OnSceneGUI()
    {
        if (light == null) { FindTarget(); }
        Vector3 pos = Vector3.zero;
        if (Selection.activeTransform != null)
        {
            pos = Selection.activeTransform.position;
        }

        Handles.color = light.color;

        // Draw Light handles
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;

        Handles.Label(pos + new Vector3(0.0f, -0.1f, 0.0f), "VLight " + light.index, style);
        Handles.DrawWireDisc(pos, Vector3.right, light.range);
        Handles.DrawWireDisc(pos, Vector3.up, light.range);
        Handles.DrawWireDisc(pos, Vector3.forward, light.range);


        SceneView view = SceneView.currentDrawingSceneView;
        Vector3 screenNor = view.rotation * Vector3.forward;

        // Draw icon
        Handles.DrawWireDisc(pos, screenNor, 0.1f);


        SceneView.RepaintAll();

    }

    void OnDestroy()
    {
        UPDATE_SHADER_NAME(index);
    }

    private static void UPDATE_SHADER_NAME(int i)
    {
        string name = "GLOBAL_LIGHT_POS_" + i;
        Shader.SetGlobalVector(name, new Vector4(0.0f, 500.0f, 0.0f, 1.0f));
    }



    public const int MAX_LIGHTS = 4;
    /*[MenuItem("Vertex Lights/Debug VLight Info")]
    public static void DebugInfo()
    {
        DEBUG = !DEBUG;
    }*/

    [MenuItem("Vertex Lights/Create Light")]
    public static void CreateLight()
    {
        UpdateLights();

        if (GET_LIGHT_AMOUNT() + 1 > MAX_LIGHTS)
        {
            if (!EditorUtility.DisplayDialog(
                "Too many Vertex Lights",
                "You're creating more than " + MAX_LIGHTS + " Vertex Lights.\nPlace anyway?",
                "Place", "Do not place"))
            {
                return;
            }
        }

        GameObject go = new GameObject("VertexLight");
        go.tag = "VLight";

        VLight vl = go.AddComponent<VLight>();
        vl.color = Color.white;
        vl.index = GET_LIGHT_AMOUNT();
        vl.UpdateShaderNames();

        Selection.activeGameObject = go;
    }

    static int GET_LIGHT_AMOUNT()
    {
        VLight[] lights = GameObject.FindObjectsOfType<VLight>();
        return lights.Length;
    }

    [MenuItem("Vertex Lights/Update Lights")]
    public static void UpdateLights()
    {
        VLight[] lights = GameObject.FindObjectsOfType<VLight>();

        for (int i = 0; i < 4; i++)
        {
            Shader.SetGlobalVector("GLOBAL_LIGHT_POS_" + i.ToString(), new Vector4(0.0f, 1000.0f, 0.0f, 1.0f));
            Shader.SetGlobalVector("GLOBAL_LIGHT_COL_" + i.ToString(), Color.white);
        }

        for (int i = 0; i < lights.Length; i++)
        {

            lights[i].UpdateShaders();
        }
    }
}