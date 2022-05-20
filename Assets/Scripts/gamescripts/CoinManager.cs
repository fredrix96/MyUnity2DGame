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
    PopUpMessage message;

    List<Coin> coinList;

    int nrOfCoins;

    public CoinManager(CameraManager inCam, AudioManager inAm, int startAmount)
    {
        am = inAm;

        go = new GameObject { name = "coins" };
        go.transform.SetParent(GameManager.GameManagerObject.transform);

        message = go.AddComponent<PopUpMessage>();
        message.Init(go, inCam);

        coin = new Coin(go, inCam);

        coinList = new List<Coin>();

        nrOfCoins = startAmount;

        // Canvas
        canvasObject = new GameObject { name = "canvas" };
        canvasObject.transform.parent = go.transform;
        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = inCam.GetCamera();
        canvas.sortingLayerName = "UI";

        cs = canvasObject.AddComponent<CanvasScaler>();
        cs.referenceResolution = new Vector2(1920, 1080);

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

    public void AddCoins(int amount)
    {
        nrOfCoins += amount;
        value.text = nrOfCoins.ToString();
    }

    public void RemoveCoins(int amount)
    {
        nrOfCoins -= amount;
        value.text = nrOfCoins.ToString();
    }

    public bool SpendMoney(int amount)
    {
        if (nrOfCoins >= amount)
        {
            RemoveCoins(amount);
            return true;
        }

        // Error message
        message.SendPopUpMessage("Not enough money!", 1.5f);

        return false;
    }

    void CreateValueUI(CameraManager inCam)
    {
        // Value object
        valueObject = new GameObject { name = "value" };
        valueObject.transform.SetParent(canvas.transform);
        valueObject.transform.localScale = new Vector2(1.0f, 0.5f);

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
        value.rectTransform.anchoredPosition = new Vector3(canvas.pixelRect.width - 155, canvas.pixelRect.height - 30, 0);
    }


}
