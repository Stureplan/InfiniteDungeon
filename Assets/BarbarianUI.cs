using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class BarbarianUI : MonoBehaviour 
{
    public RectTransform topPanel, botPanel;

    public Button butLeft, butForward, butRight, butBack;
    public Sprite attack, move;

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

    public void ChangeSprite(MOVE_DIR dir, int occupant)
    {
        Sprite currentGraphic = new Sprite();

        if (occupant == 1)
        {
            currentGraphic = attack;
        }
        else if (occupant == 0)
        {
            currentGraphic = move;
        }

        switch(dir)
        {
            case MOVE_DIR.LEFT:
                butLeft.image.sprite = currentGraphic;
                break;

            case MOVE_DIR.FORWARD:
                butForward.image.sprite = currentGraphic;
                break;

            case MOVE_DIR.RIGHT:
                butRight.image.sprite = currentGraphic;
                break;

            case MOVE_DIR.BACK:
                butBack.image.sprite = currentGraphic;
                break;
        }
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
