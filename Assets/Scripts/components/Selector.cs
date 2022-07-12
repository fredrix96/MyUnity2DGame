using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    GameObject toolBarObject, textObject, btnObject;
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
        // Deselect the object if the user clicks somewhere else or if the shop is active, however, dont deselect when the user presses a button
        if (ShopManager.active)
        {
            SelectionActivated(false);
        }
        else if (Input.GetMouseButtonDown(0) && !mouseOnObject)
        {
            if (btnObject == null)
            {
                SelectionActivated(false);
            }
            else
            {
                if (!btnObject.GetComponent<ButtonEvents>().ButtonPressed())
                {
                    SelectionActivated(false);
                }
            }
        }
    }

    void SelectionActivated(bool activated)
    {
        objectPressed = activated;
        Tools.OutlineMaterialSettings.Enable(ref sr, activated);
        toolBarObject.SetActive(activated);

        if (textObject != null) textObject.SetActive(activated);
        if (btnObject != null) btnObject.SetActive(activated);
    }

    public void Init(GameObject inGo, SpriteRenderer inSr, GameObject inTextGo = null, GameObject inBtnGo = null)
    {
        sr = inSr;
        sr.material = Resources.Load<Material>("Materials/Outline");
        toolBarObject = inGo;
        textObject = inTextGo;
        btnObject = inBtnGo;
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
            AudioManager.PlayAudio2D("Select", 0.1f);
            SelectionActivated(true);
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
