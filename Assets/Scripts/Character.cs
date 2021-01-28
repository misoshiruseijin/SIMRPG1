using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public Slider hpSlider;
    public GameObject explosionPrefab;
    public GameObject hpText;

    public int hp;
    public int Maxhp = 100;
    public int atk;


    // Start is called before the first frame update
    void Start()
    {
        atk = 10;
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
