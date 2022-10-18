using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public static class MainMenu
{
    public static GameObject go, startButtonObject, exitButtonObject;
    public static SpriteRenderer sr;
    public static Canvas canvasStart, canvasExit;

    public static void Init(UnityAction startFunc, UnityAction exitFunc)
    {
        go = new GameObject { name = "MainMenu" };

        sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("Sprites/MenuBackground");

        float width = sr.sprite.bounds.size.x;
        float height = sr.sprite.bounds.size.y;

        float worldScreenHeight = CameraManager.GetCamera().orthographicSize * 2.0f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        sr.transform.localScale = new Vector3(worldScreenWidth / width, worldScreenHeight / height, 1);

        // Start button
        startButtonObject = new GameObject { name = "StartButtonObject"};
        startButtonObject.transform.SetParent(go.transform);
        canvasStart = startButtonObject.AddComponent<Canvas>();
        Vector2 offset = new Vector2(0, 2);
        CreateButton(go, startButtonObject, offset, "Sprites/Start", startFunc);

        // Exit button
        exitButtonObject = new GameObject { name = "ExitButtonObject" };
        exitButtonObject.transform.SetParent(go.transform);
        canvasExit = exitButtonObject.AddComponent<Canvas>();
        offset = new Vector2(0, -2);
        CreateButton(go, exitButtonObject, offset, "Sprites/Exit", exitFunc);

    }

    static void CreateButton(GameObject parent, GameObject go, Vector2 offset, string spritePath, UnityAction func)
    {
        go = new GameObject { name = go.name + "_button" };
        go.transform.SetParent(parent.transform);
        go.transform.position = parent.transform.position;
        go.AddComponent<GraphicRaycaster>();
        go.AddComponent<ButtonEvents>();

        Canvas cv = go.GetComponent<Canvas>();
        cv.transform.SetParent(go.transform);
        cv.transform.position = new Vector2(go.transform.position.x, go.transform.position.y + offset.y);
        cv.transform.localScale = new Vector2(0.05f, 0.02f);
        cv.sortingLayerName = "UI";
        cv.sortingOrder = 2;

        Button btn = go.AddComponent<Button>();
        btn.transform.position = cv.transform.position;
        btn.transform.SetParent(cv.transform);

        btn.image = go.AddComponent<Image>();
        btn.image.sprite = Resources.Load<Sprite>(spritePath);
        btn.targetGraphic = btn.image;

        btn.onClick.AddListener(func);
    }

    public static void Remove()
    {
        Object.Destroy(go);
    }

}
