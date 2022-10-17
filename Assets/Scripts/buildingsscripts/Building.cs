using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Building
{
    protected GameObject go, canvasObject, toolBarObject, textObject, buttonObject;
    protected SpriteRenderer sr;
    protected BoxCollider2D collider;
    protected Rigidbody2D rb;
    protected Selector selector;
    protected CoinManager coinMan;
    protected Material outline;
    protected Health health;
    protected Tile centerTile;
    protected PopUpMessage message;

    protected Canvas canvas, canvasText, canvasButton;
    protected CanvasScaler scaler;
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

    protected void CreateCanvas()
    {
        canvasObject = new GameObject { name = go.name + "_canvas" };
        canvasObject.transform.SetParent(go.transform);

        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = CameraManager.GetCamera();
        canvas.sortingLayerName = "UI";

        scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
    }

    protected void CreateToolbarObject(Vector2 inSize, float inHeight)
    {
        if (canvasObject == null)
        {
            Debug.Log("There exists no local canvas! You need to call CreateCanvas()");
            return;
        }

        toolBarObject = UIManager.CreateImage(canvasObject, "toolBar", Resources.Load<Sprite>("Sprites/WoodenBackground"), new Vector2(go.transform.position.x, go.transform.position.y + inHeight), inSize);
    }

    protected void CreateInfoText(string inText, int fontSize, TextAnchor inAlignment, Vector2 inSize)
    {
        if (toolBarObject == null)
        {
            Debug.Log("There exists no toolbar object! You need to call CreateToolbarObject()");
            return;
        }

        text = UIManager.CreateText(toolBarObject, "textInfo", inText, fontSize, canvasObject.transform.position, inSize, inAlignment);
    }

    protected void CreateButton(Sprite inSprite, Vector2 alignementFromCenter, Vector2 inSize, UnityAction function)
    {
        if (toolBarObject == null)
        {
            Debug.Log("There exists no toolbar object! You need to call CreateToolbarObject()");
            return;
        }

        button = UIManager.CreateButton(toolBarObject, "button", inSprite, inSize, alignementFromCenter, function);
    }

    public void MarkOrUnmarkTiles(BuildingInformation.TYPE_OF_BUILDING type, Tile inPos, bool mark)
    {
        Vector2 size = BuildingInformation.GetBuildingSize(type);
        int startX = - Mathf.FloorToInt(size.x / 2f);
        int endX = Mathf.CeilToInt(size.x / 2f);
        int startY = 0;// -Mathf.FloorToInt(size.y / 2f);
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
        health.Init(go, "Sprites/SoldierHealth", BuildingInformation.GetBuildingHealth(type), new Vector2(0.6f, 0.3f), 0, true);
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
        UnityEngine.Object.Destroy(go);
    }
}
