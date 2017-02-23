using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class BarbarianUI : MonoBehaviour 
{
    public RectTransform topPanel, botPanel;

    public Image healthBar;
    public Button butLeft, butForward, butRight, butBack;
    public Sprite attack, moveLeft, moveForward, moveRight, moveBack;
    public Text health;

    public RawImage minimap;

    public void SetMinimap(Texture2D tex)
    {
        minimap.color = Color.white;
        minimap.texture = tex;
    }

    public void SetMinimapPixel(int x, int y)
    {
        //minimap.rectTransform.localPosition
    }

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
    
    public void SetHealth(int h)
    {
        healthBar.fillAmount = (float)h / (float)Barbarian.MAX_HEALTH;
        health.text = h.ToString();
    }

    public void ChangeSprite(MOVE_DIR dir, int occupant)
    {
        Sprite currentGraphic = new Sprite();


        switch(dir)
        {
            case MOVE_DIR.LEFT:
                currentGraphic = moveLeft;
                if (occupant == 1)
                {
                    currentGraphic = attack;
                }
                butLeft.image.sprite = currentGraphic;
                break;

            case MOVE_DIR.FORWARD:
                currentGraphic = moveForward;
                if (occupant == 1)
                {
                    currentGraphic = attack;
                }
                butForward.image.sprite = currentGraphic;
                break;

            case MOVE_DIR.RIGHT:
                currentGraphic = moveRight;
                if (occupant == 1)
                {
                    currentGraphic = attack;
                }
                butRight.image.sprite = currentGraphic;
                break;

            case MOVE_DIR.BACK:
                currentGraphic = moveBack;
                if (occupant == 1)
                {
                    currentGraphic = attack;
                }
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
