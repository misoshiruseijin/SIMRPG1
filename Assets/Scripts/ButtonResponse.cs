using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonResponse : MonoBehaviour
{
    private bool btnReady; // false = 一度もクリックされていない, true = 一度クリックされて点滅中
    private bool btnActive; // btnReady = true の状態でクリックされるとtrueになる。対応したメソッドを実行
    private Image btnImage;
    private Color btnColor;

    void Start()
    {
        btnReady = false;
        btnActive = false;
        btnImage = GetComponent<Image>();
        Debug.Log(btnImage);
        btnColor = btnImage.GetComponent<Color>();

    }

    void Update()
    {
        if (btnReady)
        {
            float sin = 0.5f * Mathf.Sin(Time.deltaTime) + 0.5f;
            btnColor = new Color(btnColor.r, btnColor.g, btnColor.b, sin);
        }

        if (btnActive)
        {
            btnColor = new Color(btnColor.r, btnColor.g, btnColor.b, 1);
        }
    }

    public void ButtonPressed()
    {
        if (!btnReady)
        {
            // 初めてクリックされた
            btnReady = true;
            Debug.Log("初めてクリックされた");
        }

        if (btnReady && !btnActive)
        {
            btnReady = false;
            btnActive = true;
            Debug.Log("二回目のクリック");
        }
    }
}
