using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class UIManager
{
    static GameObject go;
    static Canvas canvas;
    static CanvasScaler scaler;

    public static void Init()
    {
        go = new GameObject { name = "UI" };
        go.transform.SetParent(GameManager.GameManagerObject.transform);

        canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.worldCamera = CameraManager.GetCamera();
        canvas.sortingLayerName = "UI";

        scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
    }

    /// <summary> Set parentObject to null if you want it to has the UIManager as parent. The position (0,0) is in the middle of the screen. Set sprite to null if you want a color </summary>
    public static GameObject CreateImage(GameObject parentObject, string inName, Sprite inSprite, Vector2 inPos, Vector2 inSize, Color? color = null)
    {
        GameObject imageGameObject = new GameObject { name = inName };

        if (parentObject == null) imageGameObject.transform.SetParent(go.transform);
        else imageGameObject.transform.SetParent(parentObject.transform);

        Image image = imageGameObject.AddComponent<Image>();

        if (inSprite != null)
        {
            image.sprite = inSprite;
        }
        else
        {
            image.color = color ?? Color.white;
        }

        image.transform.localPosition = inPos;
        image.transform.localScale = new Vector2(inSize.x / 100, inSize.y / 100);

        return imageGameObject;
    }

    /// <summary> Set parentObject to null if you want it to has the UIManager as parent. The position (0,0) is in the middle of the screen </summary>
    public static Text CreateText(GameObject parentObject, string inName, string inText, int fontSize, Vector2 inPos, Vector2 inSize, TextAnchor inAlignment = TextAnchor.MiddleLeft)
    {
        GameObject textGameObject = new GameObject { name = inName };

        if (parentObject == null) textGameObject.transform.SetParent(go.transform);
        else textGameObject.transform.SetParent(parentObject.transform);

        Text text = textGameObject.AddComponent<Text>();
        text.text = inText;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = fontSize;
        text.alignment = inAlignment;

        text.transform.localPosition = inPos;
        text.transform.localScale = new Vector2(inSize.x / 100, inSize.y / 100);
        text.rectTransform.sizeDelta = new Vector2(inSize.x * 2, inSize.y * 2);

        return text;
    }

    /// <summary> The position (0,0) is in the middle of the screen. Set sprite to null if you want a color. This function also outputs the image </summary>
    public static Slider CreateSlider(string sliderName, string imageName, Sprite inSprite, Vector2 inPos, Vector2 inSize, out Image image, Color? color = null, bool anchoredRight = true)
    {
        GameObject sliderGameObject = new GameObject { name = sliderName };
        sliderGameObject.transform.SetParent(go.transform);

        Slider slider = sliderGameObject.AddComponent<Slider>();
        slider.interactable = false;
        slider.transition = Selectable.Transition.None;
        slider.navigation = Navigation.defaultNavigation;

        GameObject imageObject = new GameObject { name = imageName };
        imageObject.transform.SetParent(go.transform);
        
        image = imageObject.AddComponent<Image>();

        if (inSprite != null)
        {
            image.sprite = inSprite;
        }
        else
        {
            image.color = color ?? Color.white;
        }

        image.transform.localPosition = inPos;
        image.transform.localScale = new Vector2(inSize.x / 100, inSize.y / 100);

        if (anchoredRight) image.rectTransform.pivot = new Vector2(0.0f, 0.5f);
        else image.rectTransform.pivot = new Vector2(1.0f, 0.5f);

        slider.fillRect = image.rectTransform;
        slider.value = 1;

        return slider;
    }

    /// <summary> Set parentObject to null if you want it to has the UIManager as parent. The position (0,0) is in the middle of the screen </summary>
    public static Button CreateButton(GameObject parentObject, string buttonName, Sprite inSprite, Vector2 inSize, Vector2 alignementFromCenter, UnityAction function)
    {
        GameObject buttonObject = new GameObject { name = buttonName };
        buttonObject.transform.SetParent(go.transform);

        buttonObject = new GameObject { name = parentObject.name + "_button" };
        buttonObject.transform.SetParent(parentObject.transform);
        buttonObject.transform.position = parentObject.transform.position;
        buttonObject.AddComponent<GraphicRaycaster>();
        buttonObject.AddComponent<ButtonEvents>();

        Canvas canvasButton = buttonObject.GetComponent<Canvas>();
        canvasButton.transform.SetParent(buttonObject.transform);
        canvasButton.transform.position = new Vector2(buttonObject.transform.position.x + alignementFromCenter.x, buttonObject.transform.position.y + alignementFromCenter.y);
        canvasButton.transform.localScale = new Vector2(inSize.x / 100, inSize.y / 100);
        canvasButton.sortingLayerName = "UI";
        canvasButton.sortingOrder = 2;

        Button button = buttonObject.AddComponent<Button>();
        button.transform.position = canvasButton.transform.position;
        button.transform.SetParent(canvasButton.transform);

        button.image = buttonObject.AddComponent<Image>();
        button.image.sprite = inSprite;
        button.targetGraphic = button.image;

        button.onClick.AddListener(function);

        return button;
    }
}
