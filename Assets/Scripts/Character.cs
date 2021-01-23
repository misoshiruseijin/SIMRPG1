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
        hp = Maxhp;
        hpText.GetComponent<Text>().text = "HP: " + hp.ToString();
        atk = 10;

        hpSlider.maxValue = Maxhp;
        hpSlider.value = Maxhp;
    }

    public void ReceiveDamage(int _damage)
    {
        GameObject tempParticle = Instantiate(explosionPrefab, this.transform.position, Quaternion.identity) as GameObject;

        hp -= _damage;
        hpText.GetComponent<Text>().text = "HP: " + hp.ToString();
        hpSlider.value = hp;
        if (hp < 0)
        {
            hpSlider.value = 0;
        }
    }

}
