﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building
{
    protected GameObject go;
    protected GridManager gridMan;
    protected SpriteRenderer sr;
    protected BoxCollider2D collider;
    protected Rigidbody2D rb;
    protected Selector selector;
    protected CoinManager coinMan;
    protected Material outline;
    protected Health health;
    protected Tile centerTile;

    protected BuildingInformation.TYPE_OF_BUILDING type;

    protected bool shouldBeRemoved;

    public Building()
    {
    }

    public virtual void Update() { }

    public void MarkOrUnmarkTiles(BuildingInformation.TYPE_OF_BUILDING type, Tile inPos, bool mark)
    {
        // Vector2(max, min)
        Vector2 height = Vector2.zero;
        Vector2 width = Vector2.zero;

        Vector2 size = BuildingInformation.GetBuildingSize(type);
        int startX = -Mathf.FloorToInt(size.x / 2f);
        int endX = Mathf.CeilToInt(size.x / 2f);
        int startY = -Mathf.FloorToInt(size.y / 2f);
        int endY = Mathf.CeilToInt(size.y / 2f);

        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                Tile currTile = gridMan.GetTile(new Vector2(inPos.GetTilePosition().x + x, inPos.GetTilePosition().y + y));
                if (currTile != null)
                {
                    currTile.ObjectOnTile(mark);

                    //if (x == startX && y == startY)
                    //{
                    //    // Min x and y pos
                    //    height.y = currTile.GetWorldPos().y;
                    //    width.y = currTile.GetWorldPos().x;
                    //}
                    //else if (x == endX && y == endY)
                    //{
                    //    // Max x and y pos
                    //    height.x = currTile.GetWorldPos().y;
                    //    width.x = currTile.GetWorldPos().x;
                    //}
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
                    Tile currTile = gridMan.GetTile(new Vector2(inPos.GetTilePosition().x + x, inPos.GetTilePosition().y + y));
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

    public void CheckIfDestroyed()
    {
        if (health.GetHealth() <= 0)
        {
            shouldBeRemoved = true;
        }
    }

    public bool ShouldBeRemoved()
    {
        return shouldBeRemoved;
    }

    public void Destroy()
    {
        Object.Destroy(go);
    }
}
