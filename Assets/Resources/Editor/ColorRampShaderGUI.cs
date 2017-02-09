using UnityEngine;
using UnityEditor;

public class ColorRampShaderGUI : ShaderGUI
{
    public Texture2D tex;


    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        //base.OnGUI(materialEditor, properties);
        
        Material targetMat = (Material)materialEditor.target;



        tex = Texture2D.blackTexture;
        EditorGUI.DrawPreviewTexture(new Rect(0, 200, 100, 50), tex);

        if(GUILayout.Button("Gradient Editor"))
        {
            Rect rect = new Rect(GUIUtility.GUIToScreenPoint(Event.current.mousePosition), Vector2.one * 100);
            GradientPopup.InitPopup(this, rect);
        }
    }
}


struct ColorKey
{
    public Color c;
    public float t;
    public float a;

    public Vector2 p;
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
    private int GUI_btn_w;

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
        GUI_btn = (Texture)Resources.Load("Editor/Button01");
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

        colorKeys[0].p = Vector2.zero;
        colorKeys[1].p = Vector2.zero;
        colorKeys[2].p = Vector2.zero;




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
        Rect rampPos = new Rect(20, 100, Screen.width - 40, 25);
        Rect addedPos = rampPos;
        addedPos.position += new Vector2(-(GUI_btn.width / 2), 27);

        Rect m = rampPos;
        m.position = m.position + new Vector2(-(GUI_btn.width / 2), 27);


        EditorGUILayout.LabelField("Gradient");

        EditorGUI.BeginChangeCheck();
        for(int i = 0; i < colorKeys.Length; i++)
        {
            colorKeys[i].c = EditorGUILayout.ColorField("Color " + i, colorKeys[i].c);

            Rect tmp = new Rect(addedPos);
            tmp.position += colorKeys[i].p;

            float x = rampPos.size.x;

            Vector2 pos = new Vector2(colorKeys[i].t * x, 0);
            tmp.position += pos;
            

            GUI.Button(tmp, GUI_btn, GUIStyle.none);
        }
        if (EditorGUI.EndChangeCheck())
        {
            SetupColorKeys();
            DrawColorTexture();
        }


        // Draw BG
        EditorGUI.DrawRect(new Rect(rampPos.position.x-2, rampPos.position.y-2, rampPos.size.x+4, rampPos.size.y+4), Color.grey);

        // Draw Color Ramp
        EditorGUI.DrawPreviewTexture(new Rect(20, 100, Screen.width - 40, 25), tex);



        GUILayout.BeginArea(new Rect(10, Screen.height - 60, Screen.width-20, Screen.height));
        if (GUILayout.Button("Add Color Key"))
        {
            AddColorKey();
            SetupColorKeys();
            DrawColorTexture();
        }

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
            aK[i].alpha = colorKeys[i].a;
        }


        gradient.SetKeys(cK, aK);

    }

    private void AddColorKey()
    {
        ColorKey tmp;
        tmp.c = Color.white;
        tmp.a = 1.0f;
        tmp.t = 0.2f;
        tmp.p = Vector2.zero;

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