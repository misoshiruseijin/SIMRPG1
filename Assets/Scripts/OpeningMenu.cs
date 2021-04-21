using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningMenu : MonoBehaviour
{
    CharacterData ally1, ally2;
    string name1, name2;

    public void Start()
    {
        name1 = "nezumi";
        name2 = "ka";  
    }

    public void StartGame()
    {
        ally1 = InitializeCharacter.DataFromSO(name1, true);
        ally2 = InitializeCharacter.DataFromSO(name2, true);
        GameController.instance.ally1 = ally1;
        GameController.instance.ally2 = ally2;
    }
}
