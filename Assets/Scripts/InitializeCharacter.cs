﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class InitializeCharacter
{
    public static CharacterData DataFromSO(string unitName, bool isAlly)
    {
        string playerSOpath = "Assets/SO/PlayerUnits/";
        string enemySOpath = "Assets/SO/EnemyUnits/";
        CharacterData data = new CharacterData();

        if (isAlly)
        {
            PlayerStatus SO = AssetDatabase.LoadAssetAtPath<PlayerStatus>(playerSOpath + unitName + ".asset");
            data.Maxhp = SO.hp;
            data.jpName = SO.jpName;
            data.atk = SO.atk;
            data.def = SO.def;
            data.spd = SO.spd;
            data.skillList = SO.skillList;
        }

        else
        {
            PlayerStatus SO = AssetDatabase.LoadAssetAtPath<PlayerStatus>(enemySOpath + unitName + ".asset");
            data.Maxhp = SO.hp;
            data.jpName = SO.jpName;
            data.atk = SO.atk;
            data.def = SO.def;
            data.spd = SO.spd;
            data.skillList = SO.skillList;
        }

        return data;
    }
}
