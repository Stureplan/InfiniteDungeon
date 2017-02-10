using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshCollider))]
public class VertexPainter : MonoBehaviour
{
    public MeshCollider v_Collider;
}


struct Brush
{
    public Brush(float r)
    {
        radius = r;
    }

    public float radius;
}

[ExecuteInEditMode]
[CustomEditor(typeof(VertexPainter))]
public class VertexPainterEditor : Editor 
{
    // VertexPainter
    VertexPainter vp;
//    MeshCollider e_Collider;
//    Transform e_Transform;

    // Scene
    Brush brush;
    Vector3 center;
    Vector3 point;
    Vector3 dir;
    MeshFilter mf;
    Color[] colors = new Color[1];


    // Editor-only
    bool paintMode = false;
    int frame = 0;
    int speed = 2;

    private void OnEnable()
    {
        vp = (VertexPainter)target;
        mf = vp.GetComponent<MeshFilter>();
        colors = new Color[mf.sharedMesh.vertexCount];

        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.red;
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
            Cursor.visible = false;
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
        //e_Transform = vp.transform;
    }

    private void SetupPainter()
    {
        brush = new Brush(0.1f);
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
        Handles.color = Color.cyan;

        Quaternion rot = Quaternion.identity;
        if (dir.magnitude > 0.001f)
        {
            rot = Quaternion.LookRotation(dir);
        }

        Handles.CircleCap(0, center + point, rot, brush.radius);
        Handles.DrawLine(point, point + (dir * 0.1f));
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


                if (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag)
                {
                    PaintVertex(hit);
                }
            }


        }

        if (frame > 999) { frame = 0; }
    }

    private void PaintVertex(RaycastHit hit)
    {
        if (hit.collider.name == vp.name)
        {
            Mesh mesh = mf.sharedMesh;

            int index = hit.triangleIndex * 3;

            //Vector3 hit1 = mesh.vertices[mesh.triangles[index  ]];
            //Vector3 hit2 = mesh.vertices[mesh.triangles[index+1]];
            //Vector3 hit3 = mesh.vertices[mesh.triangles[index+2]];

            colors[mesh.triangles[index]]   = Color.white;
            colors[mesh.triangles[index+1]] = Color.white;
            colors[mesh.triangles[index+1]] = Color.white;

            mesh.colors = colors;
        }
    }
}