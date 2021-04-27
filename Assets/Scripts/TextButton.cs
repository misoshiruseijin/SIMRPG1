using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class TextButton : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent onClick;

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        Debug.Log(name + "Clicked", this);

        onClick.Invoke();
    }
}
