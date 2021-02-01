using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class CSVReader
{

    public static List<string[]> csvData = new List<string[]>();
    private static string csvLine;

    public static void ReadCSV(string filename)
    {
        TextAsset csvFile = Resources.Load(filename) as TextAsset;
        StringReader reader = new StringReader(csvFile.text);

        while (reader.Peek() != -1)
        {
            // read csv line by line
            // csvData = [ [line 1], [line 2], ...]
            csvLine = reader.ReadLine();
            csvData.Add(csvLine.Split(','));
        }

    }

}
