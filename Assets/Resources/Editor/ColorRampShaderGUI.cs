using UnityEngine;
using UnityEditor;

public class ColorRampShaderGUI : ShaderGUI
{
    public Texture2D tex;


    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        base.OnGUI(materialEditor, properties);
        
        Material targetMat = (Material)materialEditor.target;

        if (tex != null)
        {
            targetMat.SetTexture("_MainTex", tex);
            EditorGUI.DrawPreviewTexture(new Rect(20, 200, 100, 50), tex);

        }


        if (GUILayout.Button("Gradient Editor"))
        {
            Rect rect = new Rect(GUIUtility.GUIToScreenPoint(Event.current.mousePosition), new Vector2(100, 200));
            GradientPopup.InitPopup(this, rect);
        }
    }
}


struct ColorKey
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
    private int amtOfColors = 3;

    /* GUI CONTENT */
    private Texture GUI_btn;
    private Texture GUI_btn_sel;
    private Vector2 GUI_btn_size;
    int selected = 110;

    static public void InitPopup(ColorRampShaderGUI materialGUI, Rect rect)
    {
        GradientPopup popup = CreateInstance<GradientPopup>();
        //GradientPopup popup = (GradientPopup)EditorWindow.GetWindow(typeof(GradientPopup));
        materialInstance = materialGUI;

        popup.InitializeGUI();
        popup.InitializeColors();

        popup.name = "ASDChildren";
        popup.titleContent.text = "ffff";
        popup.position = rect;
        popup.ShowUtility();

    }

    private void InitializeGUI()
    {
        GUI_btn     = (Texture)Resources.Load("Editor/Button01");
        GUI_btn_sel = (Texture)Resources.Load("Editor/Button02");

        GUI_btn_size = new Vector2(GUI_btn.width, GUI_btn.height);
    }

    private void InitializeColors()
    {
        colorKeys = new ColorKey[amtOfColors];
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

        cK[0].color = Color.red;
        aK[0].alpha = 1.0f;

        cK[0].time = 0.0f;
        aK[0].time = 0.0f;

        cK[1].color = Color.green;
        aK[1].alpha = 1.0f;

        cK[1].time = 0.5f;
        aK[1].time = 0.5f;

        cK[2].color = Color.blue;
        aK[2].alpha = 1.0f;

        cK[2].time = 1.0f;
        aK[2].time = 1.0f;

        gradient.SetKeys(cK, aK);

        tex = new Texture2D(3, 1);
        DrawColorTexture();
    }

    private void SetTexture(Texture2D tex)
    {
        materialInstance.tex = tex;
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


        GUILayout.BeginArea(new Rect(10, Screen.height - 60, Screen.width-20, Screen.height));
        if (GUILayout.Button("Add Color Key"))
        {
            AddColorKey();
            SetupColorKeys();
            DrawColorTexture();
        }
        if (GUILayout.Button("Apply Texture")) { SetTexture(tex); }
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
        /*tex = new Texture2D(amtOfColors, 1);
        
        for (int i = 0; i < amtOfColors; i++)
        {
            tex.SetPixel(i, 1, colors[i]);
        }*/



        tex = new Texture2D(100, 1);
        for (int i = 0; i < 100; i++)
        {
            float t = (float)i / 100;
            tex.SetPixel(i, 1, gradient.Evaluate(t));
        }


        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.Apply();
    }
}