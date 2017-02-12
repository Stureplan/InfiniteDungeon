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
    private string shaderPos = "GLOBAL_LIGHT_POS_";
    private string shaderCol = "GLOBAL_LIGHT_COL_";


    void Start()
    {
        if (index > MAX_LIGHTS)
        {
            Debug.LogWarning("Too many VLights!");
            this.enabled = false;
        }

        UpdateShaderNames();   
    }

    void OnDestroy()
    {
        if (CUR_LIGHTS-1 < 0)
        {
            Debug.LogWarning("Somehow deleting this VLight means " + (CUR_LIGHTS - 1).ToString() + " VLights remain.\nSomething's fucked up.");
            return;
        }

        CUR_LIGHTS--;
        transform.position = Vector3.one * 100.0f;        
        UpdateShaders();
    }

    void OnDrawGizmosSelected()
    {
        Handles.BeginGUI();

        if (DEBUG)
        {
            GUI.Label(new Rect(halfScreen.x, halfScreen.y, 200, 400),
                "CUR_LIGHTS: " + CUR_LIGHTS.ToString());
            GUI.Label(new Rect(halfScreen.x, halfScreen.y + 20, 200, 400),
                "MAX_LIGHTS: " + MAX_LIGHTS.ToString());
        }

        Handles.EndGUI();
    }

    void OnDrawGizmos()
    {
        Vector3 pos = transform.position;

        Gizmos.color = color;

        Gizmos.DrawIcon(pos, "VLight.png", false);
        Gizmos.DrawWireSphere(pos, range);

        SceneView view = SceneView.currentDrawingSceneView;
        Vector3 screenPos = view.camera.WorldToScreenPoint(pos);
        Vector2 size = Vector2.one;

        Handles.BeginGUI();
        GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 200, 400),
        "VLight " + index);
        Handles.EndGUI();

        UpdateShaders();
    }

    void Update()
    {
        if (frame % updates == 0) { UpdateShaders(); }

        frame++;
    }

    void UpdateShaderNames()
    {
        shaderPos = "GLOBAL_LIGHT_POS_" + index.ToString();
        shaderCol = "GLOBAL_LIGHT_COL_" + index.ToString();

        UpdateShaders();
    }

    public void UpdateShaders()
    {
        Vector4 vec = transform.position; vec.w = 1.0f;
        Shader.SetGlobalVector(shaderPos, vec);
        Shader.SetGlobalVector(shaderCol, color);
    }

    public void SetLightIndex(int i)
    {
        index = i;
    }

    public const int MAX_LIGHTS = 4;
    public static int CUR_LIGHTS;
    private static bool DEBUG = false;
    private static Vector2 halfScreen = new Vector2(0 + 20, 0 + 20);

    [MenuItem("Vertex Lights/Debug VLight Info")]
    public static void DebugInfo()
    {
        DEBUG = !DEBUG;
    }

    [MenuItem("Vertex Lights/Create Light")]
    public static void CreateLight()
    {
        UpdateLights();

        if (CUR_LIGHTS+1 > MAX_LIGHTS)
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
        vl.SetLightIndex(CUR_LIGHTS);
        vl.UpdateShaderNames();
        CUR_LIGHTS++;

        Selection.activeGameObject = go;
    }

    [MenuItem("Vertex Lights/Update Lights")]
    public static void UpdateLights()
    {
        VLight[] lights = GameObject.FindObjectsOfType<VLight>();
        CUR_LIGHTS = lights.Length;

        for (int i= 0; i < lights.Length; i++)
        {
            lights[i].UpdateShaders();
        }
    }
}

