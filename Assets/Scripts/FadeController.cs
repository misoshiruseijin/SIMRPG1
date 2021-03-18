using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeController : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private bool fadingIn, fadingOut;
    public float fadeTime = 1.0f;

    void Start()
    {
        canvasGroup = this.GetComponent<CanvasGroup>();
        fadingIn = false;
        fadingOut = false;
        
    }

    void Update()
    {
        if (fadingIn)
        {
            canvasGroup.alpha += Time.deltaTime / fadeTime;
            
            if (canvasGroup.alpha >= 1.0f)
            {
                canvasGroup.alpha = 1.0f;
                fadingIn = false;
            }
        }

        else if (fadingOut)
        {
            canvasGroup.alpha -= Time.deltaTime / fadeTime;

            if (canvasGroup.alpha <= 0.0f)
            {
                canvasGroup.alpha = 0.0f;
                fadingOut = false;
            }
        }
    }

    public bool IsFading()
    {
        bool fading = false;

        if (fadingIn || fadingOut)
        {
            fading = true;
        }

        return fading;
    }

    public void FadeIn()
    {
        if (!IsFading())
        {
            fadingIn = true;
        }
    }

    public void FadeOut()
    {
        if (!IsFading())
        {
            fadingOut = true;
        }
    }
}
