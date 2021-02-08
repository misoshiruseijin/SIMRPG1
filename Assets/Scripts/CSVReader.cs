using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class CSVReader
{

    public static List<string[]> csvData = new List<string[]>();
    private static string csvLine;


    public static bool returnLineFlg = false;

    public static List<string[]> ReadCSV(string filename)
    {
        // read csv file in Assets/Resources/ and store content in csvData
        TextAsset csvFile = Resources.Load(filename) as TextAsset;
        StringReader reader = new StringReader(csvFile.text);

        List<string> typesList = new List<string>();

        // skip header
        reader.ReadLine();

        while (reader.Peek() != -1)
        {
            // read csv line by line
            // csvData = [ [line 1], [line 2], ...], where [line n] is an array
            csvLine = reader.ReadLine();

            byte[] bytes = Encoding.Default.GetBytes(csvLine);
            csvLine = Encoding.UTF8.GetString(bytes);
            //Debug.Log("[!!!!!] " + csvLine);

            csvData.Add(csvLine.Split(','));
        }

        return csvData;
        
        //returnLineFlg = true;

    }

    public static string ReturnLine()
    {
        returnLineFlg = false;
        return csvLine;
    }
}
