using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character: MonoBehaviour
{
    public Slider hpSlider;
    public Text nameText;
    public GameObject explosionPrefab;
    public GameObject hpText;
    public int hp;

    public int Maxhp, atk, def, spd;
    [HideInInspector] public string jpName;

    public List<SkillStatus> skillList;

    public bool animFlg = false;

    private RectTransform rt;
    private float originalX, originalY;
    private float time;
    private float posY;
    public void Start()
    {
        rt = this.GetComponent<RectTransform>();
        originalX = rt.anchoredPosition.x;
        originalY = rt.anchoredPosition.y;
        posY = originalY;
        //time = 0;
    }

    //public void Update()
    //{
    //    rt.anchoredPosition = new Vector2(originalX, posY);
    //    //Debug.Log(posY);
    //}

    public void SetStatus()
    {
        nameText.GetComponent<Text>().text = jpName;

        hp = Maxhp;
        if (hpText != null)
        {
            hpText.GetComponent<Text>().text = "HP: " + hp.ToString();
        }

        hpSlider.maxValue = Maxhp;
        hpSlider.value = Maxhp;
    }

    public void UpdateStatus()
    {
        if (hpText != null)
        {
            if (hp >= 0)
            {
                hpText.GetComponent<Text>().text = "HP: " + hp.ToString();
            }

            else
            {
                hpText.GetComponent<Text>().text = "HP: 0";
            }
            
        }
        
        hpSlider.value = hp;

        if (hp <= 0)
        {
            hpSlider.value = 0;
        }
    }

    public void hopUnit()
    {
        // 未完成
        // ユニットををぴょんと跳ねさせる
        time = 0;

        while (time <= 5)
        {
            float sin = 10 * Mathf.Sin(2 * Mathf.PI * time);
            //rt.anchoredPosition = new Vector2(originalX, originalY + sin);
            posY = originalY + sin;
            time += Time.deltaTime;
            Debug.Log(this.name + ": " + posY);
        }

        posY = originalY;
        
    }

}
