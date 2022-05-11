using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coin
{
    GameObject go, canvasObject, imageObject;
    Canvas canvas;
    CanvasScaler cs;
    Image coin;
    SpriteRenderer sr;

    Vector3 direction; // z coord is towards the camera
    float speed;
    float lifeTime;
    double timer;
    bool rotate;
    bool shouldBeRemoved;

    // If the coin is used as UI
    public Coin(GameObject inGo, CameraManager inCam)
    {
        go = new GameObject() { name = "coin_UI" };
        go.transform.SetParent(inGo.transform);

        // Canvas
        canvasObject = new GameObject { name = "canvas" };
        canvasObject.transform.parent = go.transform;

        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = inCam.GetCamera();

        cs = canvasObject.AddComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);
        cs.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        cs.matchWidthOrHeight = 1; // match with height (1)

        // Image object
        imageObject = new GameObject { name = "coin" };
        imageObject.transform.SetParent(canvasObject.transform);
        imageObject.transform.localScale = new Vector2(0.5f, 0.5f);

        coin = imageObject.AddComponent<Image>();
        coin.sprite = Resources.Load<Sprite>("Sprites/Coin");

        // Reset anchor
        coin.rectTransform.anchorMin = Vector2.zero;
        coin.rectTransform.anchorMax = Vector2.zero;

        // Anchor the image
        coin.rectTransform.anchoredPosition = new Vector3(2300, 1031, 0);

        shouldBeRemoved = false;
    }

    // If the coin is used in the world
    public Coin(GameObject inGo, Vector3 pos, Vector2 scale, Vector3 dir, float inSpeed, float inLifeTime, bool inRotate)
    {
        go = new GameObject() { name = "coin_" + CoinCounter.counter };
        go.transform.SetParent(inGo.transform);
        go.transform.position = pos;

        canvas = go.AddComponent<Canvas>();
        sr = go.AddComponent<SpriteRenderer>();

        sr.sprite = Resources.Load<Sprite>("Sprites/Coin");
        sr.size = scale;

        go.transform.localScale = sr.size;
        go.GetComponent<RectTransform>().sizeDelta = sr.size;

        direction = dir;
        speed = inSpeed;
        rotate = inRotate;
        lifeTime = inLifeTime;

        shouldBeRemoved = false;

        CoinCounter.counter++;
    }

    public void Update()
    {
        timer += Time.deltaTime;

        go.transform.position += direction * Time.deltaTime * speed;

        if (rotate)
        {
            go.transform.Rotate(direction * speed * 200 * Time.deltaTime);
        }

        if (timer > lifeTime)
        {
            shouldBeRemoved = true;
        }
    }

    public bool Remove()
    {
        return shouldBeRemoved;
    }

    public void Destroy()
    {
        Object.Destroy(go);
        Object.Destroy(canvasObject);
        Object.Destroy(imageObject);
    }
}
