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

    [HideInInspector] public string jpName;
    [HideInInspector] public int hp;
    [HideInInspector] public int Maxhp = 100;

    public List<ScriptableObject> skillList;

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

}
