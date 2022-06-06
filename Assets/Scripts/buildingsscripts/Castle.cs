using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Castle : Building
{
    public Castle(GameObject parent, Tile inPos, CoinManager inCoinMan)
    {
        type = BuildingInformation.TYPE_OF_BUILDING.Castle;

        centerTile = inPos;
        coinMan = inCoinMan;

        go = new GameObject { name = "building_" + type.ToString() + BuildingInformation.GetCounter(type) };
        go.transform.SetParent(parent.transform);
        go.layer = LayerMask.NameToLayer("Buildings");

        go.AddComponent<CollisionManager>();

        sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("Sprites/" + type.ToString());
        sr.sortingLayerName = "Buildings";

        selector = go.AddComponent<Selector>();
        selector.Init(sr);
        selector.SetOutlineColor(Color.blue);
        selector.SetWidth(5);

        go.transform.position = inPos.GetWorldPos();

        MarkOrUnmarkTiles(type, inPos, true);

        collider = go.AddComponent<BoxCollider2D>();
        rb = go.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.useFullKinematicContacts = true;

        CreateHealthBar(type);

        shouldBeRemoved = false;

        BuildingInformation.IncreaseCounter(type);
    }

    public override void Update()
    {
        CheckIfDestroyed();
    }
}
