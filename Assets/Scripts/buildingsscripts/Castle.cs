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

        go.transform.position = inPos.GetWorldPos();

        MarkOrUnmarkTiles(type, inPos, true);

        collider = go.AddComponent<BoxCollider2D>();
        rb = go.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.useFullKinematicContacts = true;

        CreateHealthBar(type);
        CreateToolBar();

        selector = go.AddComponent<Selector>();
        selector.Init(toolBarObject, sr);
        selector.SetOutlineColor(Color.blue);
        selector.SetWidth(5);

        shouldBeRemoved = false;

        BuildingInformation.IncreaseCounter(type);
    }

    public override void Update()
    {
        CheckIfDestroyed();
    }

    void CreateToolBar()
    {
        toolBarObject = new GameObject();
        toolBarObject.name = go.name + "_toolBar";
        toolBarObject.transform.SetParent(go.transform);

        canvasToolBar = toolBarObject.AddComponent<Canvas>();
        srToolBar = toolBarObject.AddComponent<SpriteRenderer>();

        srToolBar.sprite = Resources.Load<Sprite>("Sprites/WoodenBackground");
        srToolBar.drawMode = SpriteDrawMode.Sliced;
        srToolBar.size = new Vector2(2.5f, 2f);

        toolBarObject.transform.localScale = srToolBar.size;
        toolBarObject.GetComponent<RectTransform>().sizeDelta = srToolBar.size;

        float height = 0;
        if (go.transform.GetComponent<BoxCollider2D>() == null)
        {
            Debug.Log("Warning: " + go.name + " does not have a box collider! Could not apply the correct height for the tool bar...");
        }
        else
        {
            height = toolBarObject.transform.parent.GetComponent<BoxCollider2D>().size.y * toolBarObject.transform.parent.localScale.y * 0.8f;
        }

        toolBarObject.transform.position = new Vector3(toolBarObject.transform.parent.position.x, toolBarObject.transform.parent.position.y + height, toolBarObject.transform.parent.position.z);

        toolBarObject.SetActive(false);
    }
}
