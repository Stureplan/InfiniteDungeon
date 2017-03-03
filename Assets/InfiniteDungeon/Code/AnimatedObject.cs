using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedObject : MonoBehaviour 
{
    private Animation animations;
    public void Initialize()
    {
        animations = GetComponent<Animation>();
    }

    public void PlayAnimation(string anim)
    {
        animations.Stop();
        animations.Play(anim);
    }

    public float GetClipLength(string anim)
    {
        return animations.GetClip(anim).length;
    }

    private void FadeAnimation(string anim)
    {
        animations.CrossFade(anim, 0.1f);
    }
}