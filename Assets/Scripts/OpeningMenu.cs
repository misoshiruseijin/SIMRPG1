﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningMenu : MonoBehaviour
{
    List<string> newAllyNames;
    List<CharacterData> allyDataList;

    public void Start()
    {
        allyDataList = new List<CharacterData>();
        newAllyNames = new List<string> { "nezumi", "ka" };
        foreach (string s in newAllyNames)
        {
            allyDataList.Add(ManageData.CharacterDataFromSO(s, true));
        }
    }

    public void StartGame()
    {
        ManageData.SaveCharacterData(allyDataList);
        SceneController.ToSimulationScene();
        //GameController.instance.testmethod();
    }
}
