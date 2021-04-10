using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BackButton : MonoBehaviour
{
    public List<int> panelState;
    public GameObject backBtn;

    void Start()
    {
        panelState = new List<int>();
        backBtn = this.gameObject;
    }

    void Update()
    {
        if ((panelState != null) && (panelState.Any()))
        {
            
        }
    }
}
