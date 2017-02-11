using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshCollider))]
public class VertexPainter : MonoBehaviour
{
    public MeshCollider v_Collider;
}

enum BrushMode
{
    Painting = 0,
    Shading,
    ErasingPaint,
    ErasingShade
}

struct Brush
{


    public Brush(BrushMode m, float r, Color c, Color s)
    {
        mode = m;
        radius = r;
        color = c;
        shade = s;
    }

    public BrushMode mode;
    public float radius;
    public Color color;
    public Color shade;
}

[ExecuteInEditMode]
[CustomEditor(typeof(VertexPainter))]
public class VertexPainterEditor : Editor 
{
    // VertexPainter
    VertexPainter vp;

    // Scene
    Brush brush;
    Vector3 center;
    Vector3 point;
    Vector3 dir;
    MeshFilter mf;
    Color[] colors = new Color[1];
    Vector2[] uvs = new Vector2[1];

    // Editor-only
    bool paintMode = false;
    int frame = 0;
    int speed = 2;
    Vector2 mousePos;
    bool ctrlIsDown = false;
    bool shiftIsDown = false;



    private void OnEnable()
    {
        vp = (VertexPainter)target;
        mf = vp.GetComponent<MeshFilter>();
        colors = new Color[mf.sharedMesh.vertexCount];
        uvs = new Vector2[colors.Length];

        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = mf.sharedMesh.colors[i];
            uvs[i] = mf.sharedMesh.uv[i];
        }

        paintMode = true;
        SetupReferences();
        SetupPainter();
    }

    private void OnDisable()
    {
        paintMode = false;
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Keyboard));
        Tools.current = Tool.Move;
    }

    private void OnSceneGUI()
    {
        if (paintMode)
        {
            Tools.current = Tool.None;
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        }
        else
        {

        }

        UpdatePainter();
        DrawSceneGUI();
        SceneView.RepaintAll();
    }

    public override void OnInspectorGUI()
    {
        vp = (VertexPainter)target;

        DrawInspectorGUI();
    }

    private void SetupReferences()
    {
        vp.v_Collider = vp.GetComponent<MeshCollider>();
    }

    private void SetupPainter()
    {
        Color b = new Color(0.0f, 0.0f, 0.0f, 0.5f);
        brush = new Brush(BrushMode.Painting, 0.05f, Color.red, b);
        center = vp.transform.position;
        point = Vector3.zero;
        dir = Vector3.zero;
    }

    private void DrawInspectorGUI()
    {
        //Draw inspector stuff => EditorGUILayout etc..
    }

    private void DrawSceneGUI()
    {
        /* 2D Scene GUI */
        Handles.BeginGUI();
        
        if (GUI.Button(new Rect(10, 10, 130, 20), "Mode: " + brush.mode.ToString())) { CycleModes(); }
        if (GUI.Button(new Rect(10, 35, 130, 20), "Wipe Colors"))  { ResetColors(); }
        if (GUI.Button(new Rect(10, 60, 130, 20), "Wipe Shading")) { ResetShading(); }

        Event e = Event.current;
        

        if (e.control && !ctrlIsDown)
        {
            mousePos = Event.current.mousePosition;
            mousePos.x -= 5;
            mousePos.y -= (105 -Mathf.FloorToInt(brush.shade.a * 95.0f));

            ctrlIsDown = true;
        }
        else if (!e.control)
        {
            ctrlIsDown = false;
        }
        if (ctrlIsDown)
        {
            brush.shade.a = GUI.VerticalSlider(new Rect(mousePos, new Vector2(10, 115)), brush.shade.a, 1.0f, 0.0f);
        }

        if (e.shift && !shiftIsDown)
        {
            CycleModes();
            shiftIsDown = true;
        }
        else if (!e.shift)
        {
            shiftIsDown = false;
        }



        brush.color = EditorGUI.ColorField(new Rect(10, 90, 125, 16), brush.color);
        brush.shade = EditorGUI.ColorField(new Rect(10, 110, 125, 16), brush.shade);
        Handles.EndGUI();







        /* 3D Scene GUI */
        //if (!ctrlIsDown)
        {
            Handles.color = Color.cyan;


            Quaternion rot = Quaternion.identity;
            if (dir.magnitude > 0.001f)
            {
                rot = Quaternion.LookRotation(dir);
            }

            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.cyan;
            Handles.Label(point + Vector3.down * 0.075f, brush.mode.ToString(), style);

            Handles.DrawLine(point, point + (dir * 0.1f));
            
            if (brush.mode == BrushMode.Painting)
            {
                Handles.CircleCap(0, center + point, rot, brush.radius);
            }

            else if (brush.mode == BrushMode.Shading)
            {
                float a = 1 - brush.shade.a;
                Handles.color = new Color(a, a, a, 1.0f);
                Handles.CircleCap(0, center + point, rot, 0.05f);
            }

            else if (brush.mode == BrushMode.ErasingPaint)
            {

            }

            else if (brush.mode == BrushMode.ErasingShade)
            {

            }
        }
    }

    private void UpdatePainter()
    {
        frame++;
        if (frame % speed == 0)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                point = hit.point;
                dir = hit.normal;

                if (Event.current.type == EventType.MouseDrag && 
                    !Event.current.alt &&
                    !Event.current.control &&
                    !Event.current.shift)
                {
					if (Event.current.button == 0)
					{
						PaintVertex(brush.mode, hit);
					}
                }
            }


        }

        if (frame > 999) { frame = 0; }
    }

    private void PaintVertex(BrushMode mode, RaycastHit hit)
    {
        if (hit.collider.name == vp.name)
        {
            Mesh mesh = mf.sharedMesh;

            int index = hit.triangleIndex * 3;

            //Vector3 hit1 = mesh.vertices[mesh.triangles[index  ]];
            //Vector3 hit2 = mesh.vertices[mesh.triangles[index+1]];
            //Vector3 hit3 = mesh.vertices[mesh.triangles[index+2]];

            if (mode == BrushMode.Painting)
            {
                colors[mesh.triangles[index]]     = brush.color;
                colors[mesh.triangles[index + 1]] = brush.color;
                colors[mesh.triangles[index + 2]] = brush.color;

                mesh.colors = colors;
            }
            else if (mode == BrushMode.Shading)
            {
                uvs[mesh.triangles[index]].x   = brush.shade.a;
                uvs[mesh.triangles[index+1]].x = brush.shade.a;
                uvs[mesh.triangles[index+2]].x = brush.shade.a;

                mesh.uv = uvs;
            }
            else if (mode == BrushMode.ErasingPaint)
            {
                colors[mesh.triangles[index]]     = Color.clear;
                colors[mesh.triangles[index + 1]] = Color.clear;
                colors[mesh.triangles[index + 2]] = Color.clear;

                mesh.colors = colors;
            }
            else if (mode == BrushMode.ErasingShade)
            {
                uvs[mesh.triangles[index]].x     = 0.0f;
                uvs[mesh.triangles[index + 1]].x = 0.0f;
                uvs[mesh.triangles[index + 2]].x = 0.0f;

                mesh.uv = uvs;
            }
        }
    }

    private void ResetColors()
    {
        int vtx = colors.Length;
        colors = new Color[vtx];
        for (int i = 0; i < vtx; i++)
        {
            colors[i] = Color.clear;
        }

        mf.sharedMesh.colors = colors;
    }

    private void ResetShading()
    {
        int vtx = uvs.Length;
        uvs = new Vector2[vtx];
        for (int i = 0; i < vtx; i++)
        {
            uvs[i] = Vector2.zero;
        }

        mf.sharedMesh.uv = uvs;
    }

    private void CycleModes()
    {
        int b = System.Enum.GetNames(typeof(BrushMode)).Length-1;
        if (brush.mode == (BrushMode)b)
        {
            brush.mode = 0;
        }
        else
        {
            brush.mode++;

        }
    }
}