using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonEvents : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler
{
    bool buttonPressed;

    void Start()
    {
        buttonPressed = false;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        SetButtonPressed(false);
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            SetButtonPressed(true);
        }
    }

    public void SetButtonPressed(bool pressed)
    {
        buttonPressed = pressed;
    }

    public bool ButtonPressed()
    {
        return buttonPressed;
    }
}
