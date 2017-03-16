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
    public Text damageText;
    public Text coinsText;
    public Animation damageTextAnim;

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

    public void SetCoins(int c)
    {
        coinsText.text = c.ToString();
    }

    public void FloatingDamageText(int dmg)
    {
        damageText.text = (-dmg).ToString();
        damageTextAnim.Stop();
        damageTextAnim.Play("CombatText1");
    }

    public void ChangeSprite(MOVELIST dir, int occupant)
    {
        Sprite currentGraphic = new Sprite();


        switch(dir)
        {
            case MOVELIST.M_LEFT:
                currentGraphic = moveLeft;
                if (occupant == 1)
                {
                    currentGraphic = attack;
                }
                butLeft.image.sprite = currentGraphic;
                break;

            case MOVELIST.M_FORWARD:
                currentGraphic = moveForward;
                if (occupant == 1)
                {
                    currentGraphic = attack;
                }
                butForward.image.sprite = currentGraphic;
                break;

            case MOVELIST.M_RIGHT:
                currentGraphic = moveRight;
                if (occupant == 1)
                {
                    currentGraphic = attack;
                }
                butRight.image.sprite = currentGraphic;
                break;

            case MOVELIST.M_BACK:
                currentGraphic = moveBack;
                if (occupant == 1)
                {
                    currentGraphic = attack;
                }
                butBack.image.sprite = currentGraphic;
                break;

            case MOVELIST.S_STOMP:
                //TODO: Implement stomp UI
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
