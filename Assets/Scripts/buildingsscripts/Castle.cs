using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : Building
{
    public Castle(GameObject parent, Vector3 inPos, GridManager inGridMan)
    {
        go = new GameObject { name = "castle" + BuildingInformation.CastleCounter.ToString() };
        go.transform.SetParent(parent.transform);

        sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("Sprites/Castle");
        sr.sortingLayerName = "Buildings";

        go.transform.position = inPos;
        gridMan = inGridMan;

        MarkTiles(BuildingInformation.TYPE_OF_BUILDING.CASTLE);

        collider = go.AddComponent<BoxCollider2D>();
        rb = go.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;

        BuildingInformation.CastleCounter++;
    }
}
