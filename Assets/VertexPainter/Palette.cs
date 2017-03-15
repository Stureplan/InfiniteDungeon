using System.Collections.Generic;
using UnityEngine;

public class Palette : MonoBehaviour
{
    private Color[]   m_colors;

	public void SavePalette(Color[] colors)
    {
        int n = colors.Length;
        m_colors = new Color[n];

        for (int i = 0; i < n; i++)
        {
            m_colors[i] = colors[i];
        }
    }

    public Texture2D LoadPalette()
    {
        int n = m_colors.Length;

        Texture2D texture = new Texture2D(n, 1);

        texture.SetPixels(m_colors);
        texture.name = "palette";
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();

        return texture;
    }
}
