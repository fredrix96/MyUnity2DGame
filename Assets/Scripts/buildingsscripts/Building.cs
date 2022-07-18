using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building
{
    protected GameObject go, toolBarObject, textObject, buttonObject;
    protected SpriteRenderer sr;
    protected BoxCollider2D collider;
    protected Rigidbody2D rb;
    protected Selector selector;
    protected CoinManager coinMan;
    protected Material outline;
    protected Health health;
    protected Tile centerTile;

    protected Canvas canvasToolBar, canvasText, canvasButton;
    protected Text text;
    protected SpriteRenderer srToolBar;
    protected Button button;

    protected BuildingInformation.TYPE_OF_BUILDING type;

    protected List<Building> buildings;

    protected bool shouldBeRemoved;

    public Building()
    {
    }

    public virtual void Update() { }

    protected void CreateToolbarObject(Vector2 inSize, float inHeight)
    {
        // Toolbar
        toolBarObject = new GameObject { name = go.name + "_toolBar" };
        toolBarObject.transform.SetParent(go.transform);
        toolBarObject.AddComponent<BoxCollider2D>();

        // Canvas
        canvasToolBar = toolBarObject.AddComponent<Canvas>();
        canvasToolBar.transform.localScale = new Vector3(0.1f, 0.05f, 1f);

        // Sprite
        srToolBar = toolBarObject.AddComponent<SpriteRenderer>();
        srToolBar.sortingLayerName = "UI";
        srToolBar.sprite = Resources.Load<Sprite>("Sprites/WoodenBackground");
        srToolBar.drawMode = SpriteDrawMode.Sliced;
        srToolBar.size = inSize * Graphics.resolution;

        float height = 0;
        if (go.transform.GetComponent<BoxCollider2D>() == null)
        {
            Debug.Log("Warning: " + go.name + " does not have a box collider! Could not apply the correct height for the tool bar...");
        }
        else
        {
            height = srToolBar.sprite.bounds.max.y * inHeight;
        }

        toolBarObject.transform.position = new Vector3(toolBarObject.transform.parent.position.x, toolBarObject.transform.parent.position.y + height, toolBarObject.transform.parent.position.z);
        toolBarObject.SetActive(false);
    }

    protected void CreateInfoText(string inText, TextAnchor inAlignement, Vector2 inSize)
    {
        // Text
        textObject = new GameObject { name = go.name + "_textObject" };
        textObject.transform.SetParent(go.transform);
        textObject.transform.position = toolBarObject.transform.position;

        canvasText = textObject.AddComponent<Canvas>();
        canvasText.transform.localScale = new Vector2(0.03f, 0.03f);
        canvasText.sortingLayerName = "UI";
        canvasText.sortingOrder = 2;

        text = textObject.AddComponent<Text>();
        text.text = inText;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = (int)(5.0f * Graphics.resolution);
        text.color = Color.white;
        text.fontStyle = FontStyle.Bold;
        text.alignment = inAlignement;
        text.rectTransform.sizeDelta = inSize * Graphics.resolution;

        textObject.SetActive(false);
    }

    public void MarkOrUnmarkTiles(BuildingInformation.TYPE_OF_BUILDING type, Tile inPos, bool mark)
    {
        Vector2 size = BuildingInformation.GetBuildingSize(type);
        int startX = -Mathf.FloorToInt(size.x / 2f);
        int endX = Mathf.CeilToInt(size.x / 2f);
        int startY = -Mathf.FloorToInt(size.y / 2f);
        int endY = Mathf.CeilToInt(size.y / 2f);

        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                Tile currTile = GridManager.GetTile(new Vector2(inPos.GetTilePosition().x + x, inPos.GetTilePosition().y + y));
                if (currTile != null)
                {
                    currTile.ObjectOnTile(mark);

                    if (mark) GridManager.GetObjectTiles().Add(currTile);
                    else GridManager.GetObjectTiles().Remove(currTile);
                }
            }
        }

        // Make sure that no objects can be built tightly together
        startX -= 1;
        endX += 1;
        startY -= 1;
        endY += 1;
        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                if (x == startX || x == endX - 1 || y == startY || y == endY - 1)
                {
                    Tile currTile = GridManager.GetTile(new Vector2(inPos.GetTilePosition().x + x, inPos.GetTilePosition().y + y));
                    if (currTile != null)
                    {
                        if (mark)
                        {
                            // Claim tile
                            currTile.SetPermissionToBuild(false);
                        }
                        else
                        {
                            // Unclaim tile
                            currTile.SetPermissionToBuild(true);
                        }
                    }
                }
            }
        }

        go.transform.localScale = new Vector3(inPos.GetSize().x * size.x / sr.size.x, inPos.GetSize().y * size.y / sr.size.y, 1);
    }

    public void CreateHealthBar(BuildingInformation.TYPE_OF_BUILDING type)
    {
        health = go.AddComponent<Health>();
        health.Init(go, "Sprites/SoldierHealth", BuildingInformation.GetBuildingHealth(type), true);
    }

    public Vector3 GetPosition()
    {
        return go.transform.position;
    }

    public BuildingInformation.TYPE_OF_BUILDING GetBuildingType()
    {
        return type;
    }

    public Tile GetCenterTile()
    {
        return centerTile;
    }

    public bool ShouldBeRemoved()
    {
        return shouldBeRemoved;
    }

    protected void LookIfIgnored()
    {
        // Ignore mourse clicks whenever the shop is active
        if (ShopManager.active == true)
        {
            go.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
        else
        {
            go.layer = LayerMask.NameToLayer("Buildings");
        }
    }

    public void Destroy()
    {
        Object.Destroy(go);
    }
}
