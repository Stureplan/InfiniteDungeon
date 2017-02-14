using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BarbarianUI : MonoBehaviour 
{
    public RectTransform topPanel, botPanel;

	public void FadePanelsIn()
    {
        StartCoroutine(Fade(topPanel, Vector2.zero, 2.0f));
        StartCoroutine(Fade(botPanel, Vector2.zero, 2.0f));
    }

    public void FadePanelsOut()
    {
        StartCoroutine(Fade(topPanel, new Vector2(0, 70.0f), 2.0f));
        StartCoroutine(Fade(botPanel, new Vector2(0, -128.0f), 2.0f));
    }

    IEnumerator Fade(RectTransform panel, Vector2 end, float time)
    {
        float t = 0.0f;

        while(t < time)
        {
            panel.anchoredPosition = Vector2.Lerp(panel.anchoredPosition, end, t / time);
            t += Time.deltaTime;
            yield return null;
        }
    }
}
