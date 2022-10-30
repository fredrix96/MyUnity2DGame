using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Goldmine : Building
{
    double timer;
    float moneyDelay;
    int moneyToGenerate;

    public Goldmine(GameObject parent, Tile inPos, CoinManager inCoinMan, List<Building> inBuildings)
    {
        type = BuildingInformation.TYPE_OF_BUILDING.Goldmine;

        centerTile = inPos;
        coinMan = inCoinMan;
        buildings = inBuildings;

        moneyDelay = 5.0f;
        moneyToGenerate = 50;

        go = new GameObject { name = "building_" + type.ToString() + BuildingInformation.GetCounter(type).ToString() };
        go.transform.SetParent(parent.transform);
        go.layer = LayerMask.NameToLayer("Buildings");

        go.AddComponent<PositionRendererSorter>();
        go.AddComponent<CollisionManager>();

        message = go.AddComponent<PopUpMessage>();
        message.Init(go);

        sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("Sprites/" + type.ToString());

        go.transform.position = inPos.GetWorldPos();

        MarkOrUnmarkTiles(type, inPos, true);

        collider = go.AddComponent<BoxCollider2D>();

        rb = go.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.useFullKinematicContacts = true;

        CreateHealthBar(type);
        CreateToolBar();

        collider.size = new Vector2(sr.size.x, sr.size.y / 5);
        collider.offset = new Vector2(0, -(sr.size.y / 3));

        selector = go.AddComponent<Selector>();
        selector.Init(toolBarObject, sr, textObject, buttonObject);
        selector.SetOutlineColor(Color.blue);
        selector.SetWidth(5);

        BuildingInformation.IncreaseCounter(type);
    }

    public override void Update()
    {
        CheckIfDestroyed();

        LookIfIgnored();

        timer += Time.deltaTime;
        if (timer > moneyDelay)
        {
            GenerateMoney();
            timer = 0;
        }
    }

    void CreateToolBar()
    {
        CreateCanvas();

        CreateToolbarObject(new Vector2(3f, 1f), 2f);

        CreateInfoText("Goldmine", 30, TextAnchor.MiddleCenter, new Vector2(65, 75));
    }

    public void CheckIfDestroyed()
    {
        if (health.GetHealth() <= 0)
        {
            shouldBeRemoved = true;
        }
    }

    void GenerateMoney()
    {
        coinMan.CreateCoin(go.transform.position, new Vector2(0.03f, 0.03f), Vector3.up, 1.5f, 1.0f, true);
        coinMan.AddCoins(moneyToGenerate);
    }
}
