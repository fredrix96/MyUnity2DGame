using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class CoinCounter
{
    public static int counter = 0;
}

public class CoinManager
{
    GameObject go, canvasObject, valueObject;
    AudioManager am;
    Coin coin;
    Canvas canvas;
    CanvasScaler cs;
    Text value;

    List<Coin> coinList;

    int nrOfCoins;

    public CoinManager(CameraManager inCam, AudioManager inAm)
    {
        am = inAm;

        go = new GameObject { name = "coins" };
        go.transform.SetParent(GameManager.GameManagerObject.transform);

        coin = new Coin(go, inCam);

        coinList = new List<Coin>();

        nrOfCoins = 500;

        CreateValueUI(inCam);
    }

    public void Update()
    {
        for (int i = 0; i < coinList.Count; i++)
        {
            Coin coin = coinList[i];

            coin.Update();

            if (coin.Remove())
            {
                coin.Destroy();
                coinList.Remove(coin);
            }
        }
    }

    public void CreateCoin(Vector3 pos, Vector2 scale, Vector3 dir, float speed, float lifeTime = 2, bool rotate = false)
    {
        Coin coin = new Coin(go, pos, scale, dir, speed, lifeTime, rotate);
        coinList.Add(coin);
        am.PlayAudio3D("Coin Sound", 0.1f, pos, 30, 2);
    }

    public void AddCoins(int nrToAdd)
    {
        nrOfCoins += nrToAdd;
        value.text = nrOfCoins.ToString();
    }

    public void RemoveCoins(int nrToRemove)
    {
        nrOfCoins -= nrToRemove;
        value.text = nrOfCoins.ToString();
    }

    void CreateValueUI(CameraManager inCam)
    {
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

        // Value object
        valueObject = new GameObject { name = "value" };
        valueObject.transform.SetParent(canvas.transform);
        valueObject.transform.localScale = new Vector2(1.5f, 0.8f);

        value = valueObject.AddComponent<Text>();
        value.text = nrOfCoins.ToString();
        value.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        value.fontSize = 40;
        value.alignment = TextAnchor.MiddleRight;

        // Reset anchor
        value.rectTransform.anchorMin = Vector2.zero;
        value.rectTransform.anchorMax = Vector2.zero;
        value.rectTransform.sizeDelta = new Vector2(184, 100);

        // Anchor the image
        value.rectTransform.anchoredPosition = new Vector3(2130, 1030, 1);
    }
}
