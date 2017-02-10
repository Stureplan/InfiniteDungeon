using UnityEngine;
using UnityEditor;

public class ColorRampShaderGUI : ShaderGUI
{
    public Texture2D tex;
    public ColorKey[] keys;
    public Gradient gradient;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        base.OnGUI(materialEditor, properties);
        
        Material targetMat = (Material)materialEditor.target;

        if (tex != null)
        {
            targetMat.SetTexture("_MainTex", tex);
        } 


        if (GUILayout.Button("Gradient Editor"))
        {
            Rect rect = new Rect(GUIUtility.GUIToScreenPoint(Event.current.mousePosition), new Vector2(100, 200));
            GradientPopup popup = GradientPopup.InitPopup(this, rect, keys, gradient);
            
            if (keys == null || gradient == null)
            {
                popup.DefaultRamp();
            }
            else
            {
                popup.InitializeColors(keys, gradient);
            }
        }
    }
}


public struct ColorKey
{
    public Color c;
    public float t;
    public float a;
}

public class GradientPopup : EditorWindow
{
    static ColorRampShaderGUI materialInstance;


    /* COLOR DATA */
    private Gradient gradient;
    private Texture2D tex;
    private ColorKey[] colorKeys;

    /* GUI CONTENT */
    private Texture GUI_btn;
    private Texture GUI_btn_sel;
    private Vector2 GUI_btn_size;
    int selected = 110;

    static public GradientPopup InitPopup(ColorRampShaderGUI materialGUI, Rect rect, ColorKey[] k, Gradient g)
    {
        GradientPopup popup = CreateInstance<GradientPopup>();
        //GradientPopup popup = (GradientPopup)EditorWindow.GetWindow(typeof(GradientPopup));
        materialInstance = materialGUI;

        popup.InitializeGUI();
        //popup.InitializeColors(k, g);

        popup.name = "ASDChildren";
        popup.titleContent.text = "ffff";
        popup.position = rect;
        popup.ShowUtility();

        return popup;
    }

    private void InitializeGUI()
    {
        GUI_btn     = (Texture)Resources.Load("Editor/Button01");
        GUI_btn_sel = (Texture)Resources.Load("Editor/Button02");

        GUI_btn_size = new Vector2(GUI_btn.width, GUI_btn.height);
    }

    public void InitializeColors(ColorKey[] k, Gradient g)
    {
        //load array and gradient here =>
        //remember to null check (first time)
        colorKeys = new ColorKey[k.Length];
        gradient = new Gradient();

        for (int i = 0; i < k.Length; i++)
        {
            colorKeys[i].c = k[i].c;
            colorKeys[i].a = k[i].a;
            colorKeys[i].t = k[i].t;
        }

        gradient = g;

        SetupColorKeys();
        DrawColorTexture();
    }

    public void DefaultRamp()
    {
        //Defaults to a three-key R|G|B ramp.
        colorKeys = new ColorKey[3];
        colorKeys[0].c = Color.red;
        colorKeys[0].a = 1.0f;
        colorKeys[0].t = 0.0f;

        colorKeys[1].c = Color.green;
        colorKeys[1].a = 1.0f;
        colorKeys[1].t = 0.5f;

        colorKeys[2].c = Color.blue;
        colorKeys[2].a = 1.0f;
        colorKeys[2].t = 1.0f;




        gradient = new Gradient();
        GradientColorKey[] cK = new GradientColorKey[3];
        GradientAlphaKey[] aK = new GradientAlphaKey[3];

        cK[0].color = colorKeys[0].c;
        aK[0].alpha = colorKeys[0].a;

        cK[0].time = colorKeys[0].t;
        aK[0].time = colorKeys[0].t;

        cK[1].color = colorKeys[1].c;
        aK[1].alpha = colorKeys[1].a;

        cK[1].time = colorKeys[1].t;
        aK[1].time = colorKeys[1].t;

        cK[2].color = colorKeys[2].c;
        aK[2].alpha = colorKeys[2].a;

        cK[2].time = colorKeys[2].t;
        aK[2].time = colorKeys[2].t;

        gradient.SetKeys(cK, aK);

        DrawColorTexture();
    }

    private void SaveColorRamp(Texture2D tex)
    {
        WriteFile(tex);
        materialInstance.tex = tex;
        materialInstance.keys = colorKeys;
        materialInstance.gradient = gradient;
    }

    private void OnLostFocus()
    {
        //Close();
    }

    private void OnGUI()
    {
        // Setup Rect
        Rect rampPos = new Rect(20, 45, Screen.width - 40, 25);
        Rect sliderPos = new Rect(15, 90, rampPos.width+10, 10);


        EditorGUILayout.LabelField("Gradient");


        EditorGUI.BeginChangeCheck();
        for(int i = 0; i < colorKeys.Length; i++)
        {
            Rect tmp = rampPos;
            tmp.size = GUI_btn_size;
            tmp.position += new Vector2(-(GUI_btn_size.x / 2), 27);
            tmp.position += new Vector2(colorKeys[i].t * rampPos.size.x, 0);


            if (tmp.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    selected = i;
                }
            }

            if (i == selected)
            {
                GUI.Button(tmp, GUI_btn_sel, GUIStyle.none);
            }
            else
            {
                GUI.Button(tmp, GUI_btn, GUIStyle.none);
            }
        }

        if (selected < colorKeys.Length)
        {
            colorKeys[selected].t = GUI.HorizontalSlider(sliderPos, colorKeys[selected].t, 0.0f, 1.0f);
            colorKeys[selected].c = EditorGUILayout.ColorField("Color " + selected, colorKeys[selected].c);
        }

        if (EditorGUI.EndChangeCheck())
        {
            SetupColorKeys();
            DrawColorTexture();
        }

        // Draw BG
        EditorGUI.DrawRect(new Rect(rampPos.position.x-2, rampPos.position.y-2, rampPos.size.x+4, rampPos.size.y+4), Color.grey);

        // Draw Color Ramp
        EditorGUI.DrawPreviewTexture(new Rect(20, 45, Screen.width - 40, 25), tex);


        GUILayout.BeginArea(new Rect(10, Screen.height - 70, Screen.width-20, Screen.height));
        if (GUILayout.Button("Add Color Key"))
        {
            AddColorKey();
            SetupColorKeys();
            DrawColorTexture();
        }
        if (GUILayout.Button("Apply Texture")) { SaveColorRamp(tex); }
        if (GUILayout.Button("Nevermind")) { Close(); }
        GUILayout.EndArea();

    }

    private void SetupColorKeys()
    {
        GradientColorKey[] cK = new GradientColorKey[colorKeys.Length];
        GradientAlphaKey[] aK = new GradientAlphaKey[colorKeys.Length];

        for (int i = 0; i < colorKeys.Length; i++)
        {
            cK[i].color = colorKeys[i].c;
            cK[i].time  = colorKeys[i].t;
            aK[i].alpha = colorKeys[i].c.a;
        }


        gradient.SetKeys(cK, aK);

    }

    private void AddColorKey()
    {
        ColorKey tmp;
        tmp.c = Color.white;
        tmp.a = 1.0f;
        tmp.t = 0.2f;


        ArrayUtility.Add(ref colorKeys, tmp);
    }

    private void DrawColorTexture()
    {
        int keys = colorKeys.Length;
        tex = new Texture2D(keys, 1);
        for (int i = 0; i < keys; i++)
        {
            float t = (float)i / keys;
            tex.SetPixel(i, 1, gradient.Evaluate(t));
        }


        tex.filterMode = FilterMode.Trilinear;
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.Apply();

    }

    private void WriteFile(Texture2D t)
    {
        string projectPath = Application.dataPath + "/Resources/Editor/Ramps/";
        string[] existingFiles = System.IO.Directory.GetFiles(projectPath, "*.png");

        int index = existingFiles.Length;

        string fileName = "ColorRamp" + ".png";

        byte[] bytes = t.EncodeToPNG();
        Debug.Log(projectPath + fileName);
        System.IO.File.WriteAllBytes(projectPath + fileName, bytes);
    }
}