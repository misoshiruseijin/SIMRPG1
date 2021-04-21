using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneController
{
    public static void ToSimulationScene()
    {
        SceneManager.LoadScene("SimulationScene");
    }

    public static void ToBattleScene()
    {
        SceneManager.LoadScene("BattleScene");
    }
}
