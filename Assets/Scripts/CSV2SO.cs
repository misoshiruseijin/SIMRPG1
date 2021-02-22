using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class CSV2SO : MonoBehaviour
{
    public static List<string> csvData = new List<string>();
    private static string csvLine;

    private static string PlayerCSVPath = "/SO/CSVs/PlayerUnit.csv";
    private static string EnemyCSVPath = "/SO/CSVs/EnemyUnit.csv";
    private static string SkillsCSVPath = "/SO/CSVs/Skills.csv";
    

    [MenuItem("Utilities/Generate AllData")]
    public static void GenerateAllData()
    {
        GeneratePlayerData();
        GenerateEnemyData();
        GenerateSkillData();
    }

    [MenuItem("Utilities/Generate PlayerData")]
    public static void GeneratePlayerData()
    {
        csvData.Clear();
        csvLine = string.Empty;
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

            // 名前とステータスを獲得
            p.id = splitData[0];
            p.engName = splitData[1];
            p.jpName = splitData[2];
            p.hp = int.Parse(splitData[3]);
            p.atk = int.Parse(splitData[4]);
            p.def = int.Parse(splitData[5]);
            p.spd = int.Parse(splitData[6]);

            // スキルIDを獲得
            List<string> skillData = new List<string>{ splitData[7], splitData[8], splitData[9], splitData[10]};
            p.skills = new List<int>();

            foreach(string s in skillData)
            {
                if (String.IsNullOrEmpty(s))
                {
                    break;
                }

                else
                {
                    int skillID = 10;

                    if (s[s.IndexOf("_")+1].ToString().Equals("0"))
                    {
                        skillID = int.Parse(s[s.Length-1].ToString());
                    }

                    else
                    {
                        skillID = int.Parse(s.Substring(s.IndexOf("_")));
                    }

                    p.skills.Add(skillID);
                }
            }

            AssetDatabase.CreateAsset(p, $"Assets/SO/PlayerUnits/{p.engName}.asset");
        }

        AssetDatabase.SaveAssets();
    }


    [MenuItem("Utilities/Generate EnemyData")]
    public static void GenerateEnemyData()
    {
        csvData.Clear();
        csvLine = string.Empty;
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

            // 名前とステータスを獲得
            e.id = splitData[0];
            e.engName = splitData[1];
            e.jpName = splitData[2];
            e.hp = int.Parse(splitData[3]);
            e.atk = int.Parse(splitData[4]);
            e.def = int.Parse(splitData[5]);
            e.spd = int.Parse(splitData[6]);

            // スキルIDを獲得
            List<string> skillData = new List<string> { splitData[7], splitData[8], splitData[9], splitData[10] };
            e.skills = new List<int>();

            foreach (string s in skillData)
            {
                if (String.IsNullOrEmpty(s))
                {
                    break;
                }

                else
                {
                    int skillID = 10;

                    if (s[s.IndexOf("_") + 1].ToString().Equals("0"))
                    {
                        skillID = int.Parse(s[s.Length - 1].ToString());
                    }

                    else
                    {
                        skillID = int.Parse(s.Substring(s.IndexOf("_")));
                    }

                    e.skills.Add(skillID);
                }
            }

            AssetDatabase.CreateAsset(e, $"Assets/SO/EnemyUnits/{e.engName}.asset");
        }

        AssetDatabase.SaveAssets();
    }


    [MenuItem("Utilities/Generate SkillData")]
    public static void GenerateSkillData()
    {
        csvData.Clear();
        csvLine = string.Empty;
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

            AssetDatabase.CreateAsset(s, $"Assets/SO/Skills/{s.engName}.asset");
        }

        AssetDatabase.SaveAssets();
    }
}
