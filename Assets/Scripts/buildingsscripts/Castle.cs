using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Castle : Building
{
    public Castle(GameObject parent, Tile inPos, GridManager inGridMan, CoinManager inCoinMan)
    {
        coinMan = inCoinMan;

        go = new GameObject { name = "castle" + BuildingInformation.CastleCounter.ToString() };
        go.transform.SetParent(parent.transform);

        sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("Sprites/Castle");
        sr.sortingLayerName = "Buildings";

        selector = go.AddComponent<Selector>();
        selector.Init(sr);
        selector.SetOutlineColor(Color.blue);
        selector.SetWidth(5);

        go.transform.position = inPos.GetPos();
        gridMan = inGridMan;

        MarkTiles(BuildingInformation.TYPE_OF_BUILDING.CASTLE, inPos);

        collider = go.AddComponent<BoxCollider2D>();
        rb = go.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;

        BuildingInformation.CastleCounter++;
    }

    public override void Update()
    {
    }
}
