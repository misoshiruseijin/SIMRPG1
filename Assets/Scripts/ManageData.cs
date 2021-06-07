using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class ManageData
{
    public static CharacterData CharacterDataFromSO(string unitName, bool isAlly)
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
            data.unitSprite = SO.unitImg;
            data.personaArray = new int[] { 0, 0, 0, 0 };
        }

        else
        {
            EnemyStatus SO = AssetDatabase.LoadAssetAtPath<EnemyStatus>(enemySOpath + unitName + ".asset");
            data.Maxhp = SO.hp;
            data.jpName = SO.jpName;
            data.atk = SO.atk;
            data.def = SO.def;
            data.spd = SO.spd;
            data.skillList = SO.skillList;
            data.unitSprite = SO.unitImg;
        }

        return data;
    }

    public static List<CharacterData> GenerateEnemyParty()
    {
        List<string> enemyNameList = EnemyPartyData.GetEnemyNameList();
        List<CharacterData> data = new List<CharacterData>();
        
        foreach (string name in enemyNameList)
        {
            data.Add(CharacterDataFromSO(name, false));
        }

        return data;
    }

    public static void SaveCharacterData(List<CharacterData> allyDataList)
    {
        for (int i = 0; i < allyDataList.Count; i++)
        {
            GameController.instance.allyDataArray[i] = allyDataList[i];
        }
    }

    public static List<CharacterData> LoadCharacterData()
    {
        List<CharacterData> data = new List<CharacterData>();
        CharacterData[] savedData = GameController.instance.allyDataArray;
        foreach (CharacterData cd in savedData)
        {
            if (cd == null)
            {
                break;
            }

            else
            {
                data.Add(cd);
            }
        }

        return data;
    }

    public static void SavePartyData(List<CharacterData> partyMemberData)
    {
        GameController.instance.partyMemberData = partyMemberData;
    }

    public static List<CharacterData> LoadPartyData()
    {
        return GameController.instance.partyMemberData;
    }

    public static GeneData GeneDataFromSO(string organismName)
    {
        string geneSOpath = "Assets/SO/Genes/";
        GeneData data = new GeneData();

        GeneStatus SO = AssetDatabase.LoadAssetAtPath<GeneStatus>(geneSOpath + organismName + "DNA.asset");
        data.jpName = SO.jpName;
        data.hp = SO.hp;
        data.atk = SO.atk;
        data.def = SO.def;
        data.spd = SO.spd;
        data.skill = SO.skill;
        data.risk = SO.risk;

        return data;
    }

    public static void SaveGeneData(List<GeneData> geneDataList)
    {
        GameController.instance.geneDataList = geneDataList;
    }

    public static List<GeneData> LoadGeneData()
    {
        return GameController.instance.geneDataList;
    }
}
