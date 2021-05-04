using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public GameObject msgSpedSliderObj;


    private float messageSpeed;

    void Start()
    {
        msgSpedSliderObj.GetComponent<Slider>().value = GameController.instance.messaegSpeed * 100; 
    }

    public void MessageSpeedChanged()
    {
        Slider slider = msgSpedSliderObj.GetComponent<Slider>();

        if (slider.value == 0)
        {
            messageSpeed = 0;
        }

        else
        {
            messageSpeed = slider.value / 100;
        }

        GameController.instance.messaegSpeed = messageSpeed;
    }


}
