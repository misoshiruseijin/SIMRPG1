using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ManageGeneData
{
    public static GeneData DataFromSO(string organismName)
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
}
