using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GradientGenerator : MonoBehaviour 
{
    public Gradient gradient;
}

[CustomEditor(typeof(GradientGenerator))]
public class ObjectBuilderEditor : Editor
{
    Texture2D preview;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GradientGenerator generator = (GradientGenerator)target;
        if (GUILayout.Button("Build Texture"))
        {
            preview = GenerateTexture(generator.gradient);
        }


        if (preview != null)
        {
            GUILayout.Label(preview);
            Rect r = EditorGUILayout.GetControlRect();
            GUI.DrawTexture(r, preview);

            if (GUILayout.Button("Save Texture"))
            {
                SaveTexture(preview);
            }
        }


    }

    static Texture2D GenerateTexture(Gradient g)
    {
        int keys = g.colorKeys.Length;
        Texture2D tex = new Texture2D(keys, 1);

        for (int i = 0; i < keys; i++)
        {
            float t = (float)i / (float)(keys-1);
            tex.SetPixel(i, 1, g.Evaluate(t));
        }

        tex.name = "gradient";
        tex.filterMode = FilterMode.Trilinear;
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.Apply();

        return tex;
    }

    static void SaveTexture(Texture2D tex)
    {
        if (tex == null) { return; }
        string filePath = EditorUtility.SaveFilePanel("Save Texture", "Assets/", tex.name, "png");

        tex.wrapMode = TextureWrapMode.Clamp;
        byte[] bytes = tex.EncodeToPNG();
        System.IO.File.WriteAllBytes(filePath, bytes);
    }
}