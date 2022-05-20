using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building
{
    protected GameObject go;
    protected GridManager gridMan;
    protected SpriteRenderer sr;
    protected BoxCollider2D collider;
    protected Rigidbody2D rb;

    public Building()
    {
    }

    protected void MarkTiles(BuildingInformation.TYPE_OF_BUILDING type, Tile inPos)
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
                    currTile.ObjectOnTile(true);

                    if (x == startX && y == startY)
                    {
                        // Min x and y pos
                        height.y = currTile.GetPos().y;
                        width.y = currTile.GetPos().x;
                    }
                    else if (x == endX && y == endY)
                    {
                        // Max x and y pos
                        height.x = currTile.GetPos().y;
                        width.x = currTile.GetPos().x;
                    }
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
                        currTile.SetPermissionToBuild(false);
                    }
                }
            }
        }

        go.transform.localScale = new Vector3(inPos.GetSize().x * size.x / sr.size.x, inPos.GetSize().y * size.y / sr.size.y, 1);
    }
}
