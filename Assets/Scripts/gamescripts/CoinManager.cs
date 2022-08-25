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
    GameObject go;
    Text value;
    PopUpMessage message;
    Coin coinUI;

    List<Coin> coinList;

    int nrOfCoins;

    public CoinManager(int startAmount)
    {
        go = new GameObject { name = "coins" };
        go.transform.SetParent(GameManager.GameManagerObject.transform);

        message = go.AddComponent<PopUpMessage>();
        message.Init(go);

        coinUI = new Coin(go);

        coinList = new List<Coin>();

        nrOfCoins = startAmount;

        value = UIManager.CreateText(null, "valueText", nrOfCoins.ToString(), 35, new Vector2(780, 500), new Vector2(100, 100), TextAnchor.MiddleRight);
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

#if DEBUG
        if (Input.GetKey(KeyCode.Z))
        {
            AddCoins(1);
        }
# endif
    }

    public void CreateCoin(Vector3 pos, Vector2 scale, Vector3 dir, float speed, float lifeTime = 2, bool rotate = false)
    {
        Coin coin = new Coin(go, pos, scale, dir, speed, lifeTime, rotate);
        coinList.Add(coin);
        AudioManager.PlayAudio3D("Coin Sound", 0.1f, pos);
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
}
