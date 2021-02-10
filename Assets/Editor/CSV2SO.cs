using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class CSV2SO : MonoBehaviour
{
    public static List<string> csvData = new List<string>();
    private static string csvLine;

    private static string PlayerCSVPath = "/Editor/CSVs/PlayerUnit.csv";
    private static string EnemyCSVPath = "/Editor/CSVs/EnemyUnit.csv";
    private static string SkillsCSVPath = "/Editor/CSVs/Skills.csv";
    
    [MenuItem("Utilities/Generate PlayerData")]
    public static void GeneratePlayerData()
    {
        StreamReader reader = new StreamReader(Application.dataPath + PlayerCSVPath);

        // skip header
        reader.ReadLine();

        while (reader.Peek() != -1)
        {
            // csvData = list containing arrays "csvLine"
            csvLine = reader.ReadLine();

            byte[] bytes = Encoding.Default.GetBytes(csvLine);
            csvLine = Encoding.UTF8.GetString(bytes);

            csvData.Add(csvLine);
        }

        foreach (string csvLine in csvData)
        {
            string[] splitData = csvLine.Split(',');
            PlayerStatus p = ScriptableObject.CreateInstance<PlayerStatus>();
            p.id = splitData[0];
            p.engName = splitData[1];
            p.jpName = splitData[2];
            p.hp = int.Parse(splitData[3]);
            p.atk = int.Parse(splitData[4]);
            p.def = int.Parse(splitData[5]);
            p.spd = int.Parse(splitData[6]);
            p.skill1 = splitData[7];
            p.skill2 = splitData[8];
            p.skill3 = splitData[9];
            p.skill4 = splitData[10];

            AssetDatabase.CreateAsset(p, $"Assets/Editor/PlayerUnits/{p.engName}.asset");
        }

        AssetDatabase.SaveAssets();
    }


    [MenuItem("Utilities/Generate EnemyData")]
    public static void GenerateEnemyData()
    {
        StreamReader reader = new StreamReader(Application.dataPath + EnemyCSVPath);

        // skip header
        reader.ReadLine();

        while (reader.Peek() != -1)
        {
            // csvData = list containing arrays "csvLine"
            csvLine = reader.ReadLine();

            byte[] bytes = Encoding.Default.GetBytes(csvLine);
            csvLine = Encoding.UTF8.GetString(bytes);

            csvData.Add(csvLine);
        }

        foreach (string csvLine in csvData)
        {
            string[] splitData = csvLine.Split(',');
            EnemyStatus e = ScriptableObject.CreateInstance<EnemyStatus>();
            e.id = splitData[0];
            e.engName = splitData[1];
            e.jpName = splitData[2];
            e.hp = int.Parse(splitData[3]);
            e.atk = int.Parse(splitData[4]);
            e.def = int.Parse(splitData[5]);
            e.spd = int.Parse(splitData[6]);
            e.skill1 = splitData[7];
            e.skill2 = splitData[8];
            e.skill3 = splitData[9];
            e.skill4 = splitData[10];

            AssetDatabase.CreateAsset(e, $"Assets/Editor/EnemyUnits/{e.engName}.asset");
        }

        AssetDatabase.SaveAssets();
    }


    [MenuItem("Utilities/Generate SkillData")]
    public static void GenerateSkillData()
    {
        StreamReader reader = new StreamReader(Application.dataPath + SkillsCSVPath);

        // skip header
        reader.ReadLine();

        while (reader.Peek() != -1)
        {
            // csvData = list containing arrays "csvLine"
            csvLine = reader.ReadLine();

            byte[] bytes = Encoding.Default.GetBytes(csvLine);
            csvLine = Encoding.UTF8.GetString(bytes);

            csvData.Add(csvLine);
        }

        foreach (string csvLine in csvData)
        {
            string[] splitData = csvLine.Split(',');
            SkillStatus s = ScriptableObject.CreateInstance<SkillStatus>();
            s.id = splitData[0];
            s.engName = splitData[1];
            s.jpName = splitData[2];
            s.desc = splitData[3];
            s.message1 = splitData[4];
            s.message2 = splitData[5];
            s.message3 = splitData[6];
            s.dmgTarget = splitData[7];
            s.healTarget = splitData[8];
            s.buffTarget = splitData[9];
            s.dmgMult = float.Parse(splitData[10]);
            s.healMult = float.Parse(splitData[11]);
            s.buffMult = float.Parse(splitData[12]);
            GameObject effectPrefab1 = Resources.Load<GameObject>("Effects/" + splitData[13]);
            s.effect1 = effectPrefab1;
            GameObject effectPrefab2 = Resources.Load<GameObject>("Effects/" + splitData[14]);
            s.effect2 = effectPrefab2;

            AssetDatabase.CreateAsset(s, $"Assets/Editor/Skills/{s.engName}.asset");
        }

        AssetDatabase.SaveAssets();
    }
}
