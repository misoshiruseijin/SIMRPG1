using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonResponse : MonoBehaviour
{
    public bool btnReady; // false = 一度もクリックされていない, true = 一度クリックされて点滅中
    public bool btnActive; // btnReady = true の状態でクリックされるとtrueになる。対応したメソッドを実行

    private Image btnImage;
    private float time;
    private float maxAlpha;

    void Start()
    {
        btnReady = false;
        btnActive = false;
        btnImage = GetComponent<Image>();
        maxAlpha = 0.5f;
    }

    void Update()
    {
        if (!btnReady && !btnActive)
        {
            btnImage.color = new Color(btnImage.color.r, btnImage.color.g, btnImage.color.b, 0f);
        }

        else if (btnReady && !btnActive)
        {
            btnImage.color = GetAlpha(btnImage.color);
        }

        else if (btnActive)
        {
            btnImage.color = Color.gray;
            time = 0;
        }
    }

    private Color GetAlpha(Color color)
    {
        time += Time.deltaTime;
        color.a = (maxAlpha * 0.5f) * Mathf.Sin(3f * time) + (maxAlpha * 0.5f);

        return color;
    }
    
    public void ResetButton()
    {
        btnReady = false;
        btnActive = false;
        //btnImage.color = new Color(btnImage.color.r, btnImage.color.g, btnImage.color.b, 0f);
    }

    public void SetButton()
    {
        btnReady = false;
        btnActive = true;
        btnImage.color = Color.gray;
    }
}
