using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class House : Building
{
    public House(GameObject parent, Tile inPos, GridManager inGridMan, CoinManager inCoinMan)
    {
        type = BuildingInformation.TYPE_OF_BUILDING.House;

        coinMan = inCoinMan;

        go = new GameObject { name = type.ToString() + BuildingInformation.GetCounter(type).ToString() };
        go.transform.SetParent(parent.transform);

        sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("Sprites/" + type.ToString());
        sr.sortingLayerName = "Buildings";

        selector = go.AddComponent<Selector>();
        selector.Init(sr);
        selector.SetOutlineColor(Color.blue);
        selector.SetWidth(5);

        go.transform.position = inPos.GetWorldPos();
        gridMan = inGridMan;

        MarkTiles(type, inPos);

        collider = go.AddComponent<BoxCollider2D>();
        rb = go.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;

        CreateHealthBar(type);

        BuildingInformation.IncreaseCounter(type);
    }

    public override void Update()
    {
    }
}
