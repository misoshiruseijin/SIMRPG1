using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public Animator newDayTransition;
    public float transitionTime = 1f;

    private string simulation = "SimulationScene";

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            NewDay();
        }
    }

    public void NewDay()
    {
        StartCoroutine("LoadNewDay");
    }

    IEnumerator LoadNewDay()
    {
        newDayTransition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(simulation);
    }
}
