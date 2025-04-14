using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackoutFade : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1.0f;
    bool isFading = false;
    bool blackout = false;
    bool flipAlpha = false;
    private float fadeTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        fadeImage.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isFading)
        {
            fadeTimer += Time.deltaTime;

            fadeImage.color = new Color(0f, 0f, 0f, getAlpha());

            if (fadeTimer >= fadeDuration)
            {
                isFading = false;
                fadeTimer = 0.0f;
            }
        }
        else if (blackout) 
        {
            fadeTimer += Time.deltaTime;
            fadeImage.color = Color.black;

            if (fadeTimer >= fadeDuration)
            {
                blackout = false;
                isFading = true;
                flipAlpha = true;
                fadeTimer = 0.0f;
            }
        }
    }

    public void Fade()
    {
        fadeImage.gameObject.SetActive(true);
        fadeTimer = 0.0f;
        isFading = true;
        blackout = true;
        flipAlpha = false;
    }

    float getAlpha()
    {
        float alpha = Mathf.Clamp01(fadeTimer/fadeDuration);
        return flipAlpha ? 1 - alpha : alpha;
    }

    //public void TurnBlack()
    //{
    //    fadeImage.gameObject.SetActive(true);
    //    fadeImage.color = Color.black;
    //}
    //public void TurnClear()
    //{
    //    fadeImage.gameObject.SetActive(false);
    //}
}
