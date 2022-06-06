using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Burst;

public static class GridManager
{
    static Vector2 res;

    static Tile[,] grid;
    static List<Tile> tilesWithEnemies, tilesWithSoldiers, tilesWithObjects;
    static Tile tileWithPlayer;
    static GameObject go, imageObject;
    static Vector2 tilePosition;

    static Image areaImage;
    static Canvas canvas;

    public static void Init(Vector2 inRes)
    {
        res = inRes;

        go = new GameObject { name = "grid" };
        go.transform.SetParent(GameManager.GameManagerObject.transform);
        tilePosition = new Vector2(0, 0);

        grid = new Tile[(int)res.x, (int)res.y];
        tilesWithEnemies = new List<Tile>();
        tilesWithSoldiers = new List<Tile>();
        tilesWithObjects = new List<Tile>();

        GenerateGrid();

        imageObject = new GameObject { name = "areaImage" };
        imageObject.transform.SetParent(go.transform);

        canvas = imageObject.AddComponent<Canvas>();
        canvas.sortingLayerName = "CenterUI";

        Vector2 sizeOfTile = grid[0, 0].GetSize();

        RectTransform rect = imageObject.GetComponent<RectTransform>();
        rect.transform.position = grid[((int)res.x) / 3, ((int)res.y) / 2].GetWorldPos();
        float offSet = grid[0, 0].GetWorldPos().y - grid[0, 1].GetWorldPos().y;
        rect.transform.position = new Vector2(rect.transform.position.x, rect.transform.position.y + offSet / 2);
        rect.pivot = new Vector2(1, rect.pivot.y);
        rect.localScale = new Vector3(sizeOfTile.x * res.x / 300, sizeOfTile.y * res.y / 100, 1);

        areaImage = imageObject.AddComponent<Image>();
        areaImage.sprite = Resources.Load<Sprite>("Sprites/Grid");
        areaImage.color = new Color(0.4f, 1.0f, 0.0f, 0.2f);

        imageObject.SetActive(false);
    }

    public static void ActivateAreaImage(bool active)
    {
        imageObject.SetActive(active);
    }

    static void GenerateGrid()
    {
        // Size of each tile
        float length = Tools.CalculateDistance(Graphics.GetLevelLimits().y, Graphics.GetLevelLimits().x);
        float height = Tools.CalculateDistance(Graphics.GetLevelLimits().w, Graphics.GetLevelLimits().z);
        Vector2 tileSize = new Vector2(length / res.x, height / res.y);

        // Start position
        Vector2 currPosition = new Vector2(Graphics.GetLevelLimits().x + tileSize.x / 2, Graphics.GetLevelLimits().w - tileSize.y / 2);

        for (int y = 0; y < (int)res.y; y++)
        {
            for (int x = 0; x < (int)res.x; x++)
            {
                tilePosition = new Vector2(x, y);

                grid[x, y] = new Tile(go, tilePosition);
                grid[x, y].SetSize(tileSize);
                grid[x, y].SetWorldPos(currPosition);

                currPosition = new Vector2(currPosition.x + tileSize.x, currPosition.y);
            }

            currPosition = new Vector2(Graphics.GetLevelLimits().x + tileSize.x / 2, currPosition.y - tileSize.y);
        }
    }

    /// <summary> Return null if the incoming tile position is outside of the grid </summary>
    public static Tile GetTile(Vector2 tilePosition)
    {
        if (tilePosition.x < 0 || tilePosition.x > res.x - 1
            || tilePosition.y < 0 || tilePosition.y > res.y - 1)
        {
            return null;
        }

        return grid[(int)tilePosition.x, (int)tilePosition.y];
    }

    public static Tile GetTileFromWorldPosition(Vector2 pos)
    {
        Tile outTile = null;

        // Return null if the incoming position is outside of the grid
        if (pos.x < grid[0, 0].GetWorldPos().x || pos.x > grid[(int)res.x - 1, (int)res.y - 1].GetWorldPos().x
            || pos.y > grid[0, 0].GetWorldPos().y || pos.y < grid[(int)res.x - 1, (int)res.y - 1].GetWorldPos().y)
        {
            return outTile;
        }

        // First find where the position is on the y axis
        int y = 0;
        for (; y < (int)res.y; y++)
        {
            bool atThisTile = false;

            if (y + 1 >= (int)res.y)
            {
                atThisTile = grid[0, y].GetWorldPos().y - grid[0, y].GetSize().y / 2 <= pos.y;
            }
            else
            {
                atThisTile = grid[0, y].GetWorldPos().y - grid[0, y].GetSize().y / 2 <= pos.y && pos.y > grid[0, y + 1].GetWorldPos().y - grid[0, y + 1].GetSize().y / 2;
            }

            if (atThisTile)
            {
                break;
            }
        }

        // Then find where the position is on the x axis with the help of the y position
        for (int x = 0; x < (int)res.x; x++)
        {
            bool atThisTile;

            if (x + 1 >= (int)res.x)
            {
                atThisTile = grid[x, y].GetWorldPos().x - grid[x, y].GetSize().x / 2 <= pos.x;
            }
            else
            {
                atThisTile = grid[x, y].GetWorldPos().x - grid[x, y].GetSize().x / 2 <= pos.x && pos.x < grid[x + 1, y].GetWorldPos().x - grid[x + 1, y].GetSize().x / 2;
            }

            if (atThisTile)
            {
                outTile = grid[x, y];
                break;
            }
        }

        return outTile;
    }

    public static List<Tile> GetObjectTiles()
    {
        return tilesWithObjects;
    }

    public static List<Tile> GetCharacterTiles(Character.TYPE_OF_CHARACTER type)
    {
        if (type == Character.TYPE_OF_CHARACTER.Enemy) return tilesWithEnemies;
        else if (type == Character.TYPE_OF_CHARACTER.Soldier) return tilesWithSoldiers;

        Debug.LogWarning("No type of " + type + " could be found! Could not return tiles...");

        return null;
    }

    public static Tile GetPlayerTile()
    {
        return tileWithPlayer;
    }

    public static void SetPlayerTile(Tile tile)
    {
        tileWithPlayer = tile;
    }

    public static Tile FindClosestTile(Vector2 pos)
    {
        bool found = false;
        Tile tile = null;
        float step = 0;

        // Start at left upper corner
        if (pos.x < grid[0, 0].GetWorldPos().x && pos.y > grid[0, 0].GetWorldPos().y)
        {
            while (!found)
            {
                tile = GetTileFromWorldPosition(new Vector2(pos.x + step, pos.y - step));

                step += 0.1f;

                if (tile != null)
                {
                    found = true;
                }
            }
        }
        // Start at right upper corner
        else if (pos.x > grid[(int)res.x - 1, 0].GetWorldPos().x && pos.y > grid[0, 0].GetWorldPos().y)
        {
            while (!found)
            {
                tile = GetTileFromWorldPosition(new Vector2(pos.x - step, pos.y - step));

                step += 0.1f;

                if (tile != null)
                {
                    found = true;
                }
            }
        }
        // Start at left lower corner
        else if (pos.x < grid[0, 0].GetWorldPos().x && pos.y < grid[0, (int)res.y - 1].GetWorldPos().y)
        {
            while (!found)
            {
                tile = GetTileFromWorldPosition(new Vector2(pos.x + step, pos.y + step));

                step += 0.1f;

                if (tile != null)
                {
                    found = true;
                }
            }
        }
        // Start at right lower corner
        else if (pos.x > grid[(int)res.x - 1, 0].GetWorldPos().x && pos.y < grid[0, (int)res.y - 1].GetWorldPos().y)
        {
            while (!found)
            {
                tile = GetTileFromWorldPosition(new Vector2(pos.x - step, pos.y + step));

                step += 0.1f;

                if (tile != null)
                {
                    found = true;
                }
            }
        }
        // Start above grid
        else if (pos.y > grid[0, 0].GetWorldPos().y)
        {
            while (!found)
            {
                tile = GetTileFromWorldPosition(new Vector2(pos.x, pos.y - step));

                step += 0.1f;

                if (tile != null)
                {
                    found = true;
                }
            }
        }
        // Start below grid
        else if (pos.y < grid[0, (int)res.y - 1].GetWorldPos().y)
        {
            while (!found)
            {
                tile = GetTileFromWorldPosition(new Vector2(pos.x, pos.y + step));

                step += 0.1f;

                if (tile != null)
                {
                    found = true;
                }
            }
        }
        // Start to the left of grid
        else if (pos.x < grid[0, 0].GetWorldPos().x)
        {
            while (!found)
            {
                tile = GetTileFromWorldPosition(new Vector2(pos.x + step, pos.y));

                step += 0.1f;

                if (tile != null)
                {
                    found = true;
                }
            }
        }
        // Start to the right of grid
        else if (pos.x > grid[(int)res.x - 1, 0].GetWorldPos().x)
        {
            while (!found)
            {
                tile = GetTileFromWorldPosition(new Vector2(pos.x - step, pos.y));

                step += 0.1f;

                if (tile != null)
                {
                    found = true;
                }
            }
        }
        else
        {
            // Default
            tile = grid[0, 0];
        }

        // Return default
        return tile;
    }

    public static Tile[,] GetGrid()
    {
        return grid;
    }

    public static Vector2 GetRes()
    {
        return res;
    }

    /// <summary> The distance in x and y between two tiles </summary>
    public static Vector2 GetTileDistance()
    {
        float x = Mathf.Abs(grid[1, 0].GetWorldPos().x - grid[0, 0].GetWorldPos().x);
        float y = Mathf.Abs(grid[0, 1].GetWorldPos().y - grid[0, 0].GetWorldPos().y);
        return new Vector2(x, y);
    }

    /// <summary> returns Vector(MinX, MaxX, MinY, MaxY) </summary>
    public static Vector4 GetPlacementAreaBorders()
    {
        Vector4 placementArea = new Vector4(grid[0, 0].GetWorldPos().x, grid[(int)res.x / 3, 0].GetWorldPos().x, grid[0, (int)res.y - 1].GetWorldPos().y, grid[0, 0].GetWorldPos().y);
        return placementArea;
    }
}
