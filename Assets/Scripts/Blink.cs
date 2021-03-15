using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blink : MonoBehaviour
{
    private float time;
    private float speed;
    private Image image;

    // Start is called before the first frame update
    void Start()
    {
        time = 0;
        speed = 3.0f;
        image = this.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(time);
        //if (time <= 1)
        //{
        //    image.color = GetAlpha(image.color);
        //}
        //else
        //{
        //    image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
        //}
    }

    public void BlinkImage()
    {
        time = 0;
        Debug.Log(time);
        while (time <= 1)
        {
            image.color = GetAlpha(image.color);
        }
        
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
    }

    private Color GetAlpha(Color color)
    {
        time += Time.deltaTime;
        color.a = Mathf.Sin(5.0f * speed * time);

        return color;
    }
}
