using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public static class ProbabilityCalculator
{
    public static int GrowthFactor(int risk)
    {
        int minX = -100;
        int maxX = 100;
        int factor = 0;

        if(risk < 10)
        {
            return factor;
        }

        float a1 = 15; // mu at risk = 10;
        float a2 = -10; // mu at risk = 100;
        float A = (a2 - a1) / 90;
        float B = a1 - 10 * A;

        float sigma = risk / 2;
        int mu = Convert.ToInt32(A * risk + B);    

        IEnumerable<int> factorsEnum = Enumerable.Range(minX, maxX - minX + 1);
        int[] factors = factorsEnum.ToArray();
        IEnumerable<float> probsEnum = factorsEnum.Select(x => (1 / (sigma * Mathf.Sqrt(Mathf.PI))) * Mathf.Exp(-0.5f * ((x - mu) * (x - mu) / (sigma * sigma))));
        float[] probabilities = factorsEnum.Select(x => (1 / (sigma * Mathf.Sqrt(Mathf.PI))) * Mathf.Exp(-0.5f * ((x - mu) * (x - mu) / (sigma * sigma)))).ToArray();
        int[] probs = probsEnum.Select(x => Convert.ToInt32(1000 * x)).ToArray();
        List<int> factorPool = new List<int>();

        for (int i = 0; i < factors.Length; i++)
        {
            int occurences = probs[i];

            for (int j = 0; j < occurences; j++)
            {
                factorPool.Add(factors[i]);
            }
        }

        int randInt = UnityEngine.Random.Range(0, factorPool.Count);
        factor = factorPool[randInt];

        return factor;
    }
}
