using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character: MonoBehaviour
{
    public Slider hpSlider;
    public Text nameText;
    public Text statusText;
    public int hp;

    public int Maxhp, atk, def, spd;
    public string badStatus;
    [HideInInspector] public string jpName;

    public List<SkillStatus> skillList;

    private Color statusTextColor;

    public void SetStatus()
    {
        nameText.GetComponent<Text>().text = jpName;
        statusTextColor = Color.white;

        hp = Maxhp;
        if (statusText != null)
        {
            statusText.GetComponent<Text>().text = jpName + "\n" + "HP " + hp.ToString() + "\n" + badStatus;
        }

        if (hpSlider != null)
        {
            hpSlider.maxValue = Maxhp;
            hpSlider.value = Maxhp;
        }
    }

    public void UpdateStatus()
    {
        if (statusText != null)
        {
            if (hp > 0.3 * Maxhp)
            {
                // 残りHPが30％以上ある
                if (hp > Maxhp)
                {
                    // 最大HPを超えないようにする
                    hp = Maxhp;
                }

                statusTextColor = Color.white;
            }
            

            else
            {
                if (hp <= 0)
                {
                    // HPがマイナスにならないようにする
                    hp = 0;
                    statusTextColor = Color.red;
                }

                else
                {
                    statusTextColor = Color.yellow;
                }
                
            }

            statusText.GetComponent<Text>().text = jpName + "\n" + "HP " + hp.ToString() + "\n" + badStatus;
            statusText.GetComponent<Text>().color = statusTextColor;

        }

        if (hpSlider != null)
        {
            hpSlider.value = hp;
        }
        
    }

}
