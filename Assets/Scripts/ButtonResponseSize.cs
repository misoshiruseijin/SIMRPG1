using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonResponseSize : MonoBehaviour
{
    public bool btnReady;
    
    private float speed = 2f;
    private float scale; 
    private float maxScale = 1.15f; // 最大倍率
    private float minScale = 1.0f; // 最小倍率
    private float time;
    private Image image;
    private float width, height;
    private RectTransform rt;

    void Start()
    {
        btnReady = false;
        image = this.GetComponent<Image>();
        rt = this.GetComponent<RectTransform>();
        width = rt.rect.width;
        height = rt.rect.height;
    }

    void Update()
    {
        if (!btnReady)
        {
            // 通常状態
            image.rectTransform.sizeDelta = new Vector2(width, height);
            time = 0f;
        }

        else
        {
            // サイズを変える
            scale = GetImageScale();
            image.rectTransform.sizeDelta = new Vector2(scale * width, scale * height);
        }
    }

    private float GetImageScale()
    {
        time += Time.deltaTime;
        scale = (0.5f * (maxScale - minScale)) * Mathf.Sin(speed * time) + (0.5f * (maxScale - minScale)) + minScale;

        return scale;
    }
}
