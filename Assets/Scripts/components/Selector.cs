using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    GameObject toolBarObject;
    SpriteRenderer sr;

    bool mouseOnObject;
    bool objectPressed;

    void Start()
    {
        mouseOnObject = false;
        objectPressed = false;
    }

    void Update()
    {
        // Deselect the object if the user clicks somewhere else
        if (Input.GetMouseButtonDown(0) && !mouseOnObject)
        {
            objectPressed = false;
            Tools.OutlineMaterialSettings.Enable(ref sr, false);
            toolBarObject.SetActive(false);
        }
    }

    public void Init(GameObject inGo, SpriteRenderer inSr)
    {
        sr = inSr;
        sr.material = Resources.Load<Material>("Materials/Outline");
        toolBarObject = inGo;
    }

    public void SetOutlineColor(Color color)
    {
        Tools.OutlineMaterialSettings.SetOutlineColor(ref sr, color);
    }

    public void SetWidth(int width)
    {
        Tools.OutlineMaterialSettings.SetWidth(ref sr, width);
    }

    void OnMouseEnter()
    {
        mouseOnObject = true;
        Tools.OutlineMaterialSettings.Enable(ref sr, true);
    }

    void OnMouseExit()
    {
        mouseOnObject = false;

        if (!objectPressed)
        {
            Tools.OutlineMaterialSettings.Enable(ref sr, false);
        }
    }

    void OnMouseDown()
    {
        if (!ShopManager.active)
        {
            objectPressed = true;
            Tools.OutlineMaterialSettings.Enable(ref sr, true);
            toolBarObject.SetActive(true);
        }
    }

    void OnMouseDrag()
    {
    }

    void OnMouseUp()
    {
    }

    public bool MouseOnObject()
    {
        return mouseOnObject;
    }

    public bool ObjectPressed()
    {
        return objectPressed;
    }
}
