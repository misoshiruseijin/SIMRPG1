using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    public Slider hpSlider;

    public int hp;
    public int Maxhp = 100;
    public int atk;

    // Start is called before the first frame update
    void Start()
    {
        hp = Maxhp;
        hpSlider.maxValue = Maxhp;
        hpSlider.value = Maxhp;
        atk = 10;
    }

    public void ReceiveDamage(int _damage)
    {
        hp -= _damage;
        hpSlider.value = hp;
        if (hp < 0)
        {
            hpSlider.value = 0;
        }
    }

}
