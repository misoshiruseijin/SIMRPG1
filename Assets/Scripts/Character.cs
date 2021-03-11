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
    //[HideInInspector] public int Maxhp = 100;

    public List<SkillStatus> skillList;

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
        RectTransform rt = this.GetComponent<RectTransform>(); // RectTransformコンポーネントを取得
        float originalX = rt.anchoredPosition.x;
        float originalY = rt.anchoredPosition.y;
        float time = 0;

        while (time <= 1)
        {
            rt.anchoredPosition = new Vector2(originalX, originalY + 10*time);
            Debug.Log(this.name + ": " + time + ", " + rt.anchoredPosition.y);
            time += Time.deltaTime;
        }

        rt.anchoredPosition = new Vector2(originalX, originalY);

    }

}
