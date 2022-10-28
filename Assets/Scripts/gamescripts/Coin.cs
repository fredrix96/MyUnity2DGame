using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coin
{
    GameObject go;
    Canvas canvas;
    CanvasScaler cs;
    Image coin;
    SpriteRenderer sr;

    Vector3 direction; // z coord is towards the camera
    Vector3 originalPosition;
    float speed;
    float lifeTime;
    double timer;
    bool rotate;
    bool shouldBeRemoved;

    // If the coin is used as UI
    public Coin(GameObject inGo)
    {
        go = new GameObject() { name = "coin_UI" };
        go.transform.SetParent(inGo.transform);

        coin = UIManager.CreateImage(null, "coinUIImage", Resources.Load<Sprite>("Sprites/Coin"), new Vector2(910, 500), new Vector2(50, 50)).GetComponent<Image>();
        originalPosition = coin.transform.position;

        speed = 2000f;
        lifeTime = 1f;

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
        sr.sortingLayerName = "UI";

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

    public bool Spin()
    {
        timer += Time.deltaTime;

        coin.transform.Rotate(new Vector3(0, 1, 0) * speed * Time.deltaTime);
        coin.transform.position = new Vector2(coin.transform.position.x, coin.transform.position.y + Time.deltaTime * 8 * Mathf.Cos((float)timer * 10));

        // We add 180 so that we skip the negative degrees
        if (timer > lifeTime)
        {
            coin.transform.rotation = Quaternion.Euler(0, 0, 0);
            coin.transform.position = originalPosition;
            timer = 0;
            return false;
        }

        return true;
    }

    public bool Remove()
    {
        return shouldBeRemoved;
    }

    public void Destroy()
    {
        Object.Destroy(go);
    }
}
