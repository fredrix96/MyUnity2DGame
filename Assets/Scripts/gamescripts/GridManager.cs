using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager
{
    Graphics gfx;
    Tile[,] grid;
    GameObject go;
    Vector2 res;
    Vector2 tilePosition;

    public GridManager(Graphics inGfx, Vector2 inRes)
    {
        go = new GameObject { name = "grid" };
        go.transform.parent = GameManager.GameManagerObject.transform;
        tilePosition = new Vector2(0, 0);

        gfx = inGfx;
        res = inRes;

        grid = new Tile[(int)res.x, (int)res.y];

        GenerateGrid();
    }

    void GenerateGrid()
    {
        // Size of each tile
        float length = Tools.CalculateDistance(gfx.GetLevelLimits().y, gfx.GetLevelLimits().x);
        float height = Tools.CalculateDistance(gfx.GetLevelLimits().w, gfx.GetLevelLimits().z);
        Vector2 tileSize = new Vector2(length / res.x, height / res.y);

        // Start position
        Vector2 currPosition = new Vector2(gfx.GetLevelLimits().x + tileSize.x / 2, gfx.GetLevelLimits().w - tileSize.y / 2);

        for (int y = 0; y < (int)res.y; y++)
        {
            for (int x = 0; x < (int)res.x; x++)
            {
                tilePosition = new Vector2(x, y);

                grid[x, y] = new Tile(go, tilePosition);
                grid[x, y].SetSize(tileSize);
                grid[x, y].SetPos(currPosition);

                currPosition = new Vector2(currPosition.x + tileSize.x, currPosition.y);
            }

            currPosition = new Vector2(gfx.GetLevelLimits().x + tileSize.x / 2, currPosition.y - tileSize.y);
        }
    }

    public Tile GetTile(Vector2 tilePosition)
    {
        // Return null if the incoming tile position is outside of the grid
        if (tilePosition.x < 0 || tilePosition.x > res.x - 1
            || tilePosition.y < 0 || tilePosition.y > res.y - 1)
        {
            return null;
        }

        return grid[(int)tilePosition.x, (int)tilePosition.y];
    }

    public Tile GetTileFromWorldPosition(Vector2 pos)
    {
        Tile outTile = null;

        // Return null if the incoming position is outside of the grid
        if (pos.x < grid[0, 0].GetPos().x || pos.x > grid[(int)res.x - 1, (int)res.y - 1].GetPos().x
            || pos.y > grid[0, 0].GetPos().y || pos.y < grid[(int)res.x - 1, (int)res.y - 1].GetPos().y)
        {
            return outTile;
        }

        // Go through every tile to find where the position is. Worst-case time: O(n^2)
        {
            //for (int y = 0; y < (int)res.y; y++)
            //{
            //    for (int x = 0; x < (int)res.x; x++)
            //    {
            //        bool atThisTile = false;
            //
            //        if (x + 1 >= (int)res.x)
            //        {
            //            atThisTile = grid[x, y].GetPos().x - grid[x, y].GetSize().x / 2 <= pos.x;
            //        }
            //        else
            //        {
            //            atThisTile = grid[x, y].GetPos().x - grid[x, y].GetSize().x / 2 <= pos.x && pos.x < grid[x + 1, y].GetPos().x - grid[x + 1, y].GetSize().x / 2;
            //        }
            //
            //        if (atThisTile)
            //        {
            //            atThisTile = false;
            //
            //            if (y + 1 >= (int)res.y)
            //            {
            //                atThisTile = grid[x, y].GetPos().y - grid[x, y].GetSize().y / 2 <= pos.y;
            //            }
            //            else
            //            {
            //                atThisTile = grid[x, y].GetPos().y - grid[x, y].GetSize().y / 2 <= pos.y && pos.y > grid[x, y + 1].GetPos().y - grid[x, y + 1].GetSize().y / 2;
            //            }
            //
            //            if (atThisTile)
            //            {
            //                outTile = grid[x, y];
            //
            //                goto endloop;
            //            }
            //        }
            //    }
            //}
            //endloop:
        }

        // First find where the position is on the y axis. Worst-case time: O(n+n)
        int y = 0;
        for (; y < (int)res.y; y++)
        {
            bool atThisTile = false;
        
            if (y + 1 >= (int)res.y)
            {
                atThisTile = grid[0, y].GetPos().y - grid[0, y].GetSize().y / 2 <= pos.y;
            }
            else
            {
                atThisTile = grid[0, y].GetPos().y - grid[0, y].GetSize().y / 2 <= pos.y && pos.y > grid[0, y + 1].GetPos().y - grid[0, y + 1].GetSize().y / 2;
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
                atThisTile = grid[x, y].GetPos().x - grid[x, y].GetSize().x / 2 <= pos.x;
            }
            else
            {
                atThisTile = grid[x, y].GetPos().x - grid[x, y].GetSize().x / 2 <= pos.x && pos.x < grid[x + 1, y].GetPos().x - grid[x + 1, y].GetSize().x / 2;
            }
        
            if (atThisTile)
            {
                outTile = grid[x, y];
                break;
            }
        }

        return outTile;
    }

    public Tile FindClosestTile(Vector2 pos)
    {
        bool found = false;
        Tile tile = null;
        float step = 0;

        // Start at left upper corner
        if (pos.x < grid[0, 0].GetPos().x && pos.y > grid[0, 0].GetPos().y)
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
        else if (pos.x > grid[(int)res.x - 1, 0].GetPos().x && pos.y > grid[0, 0].GetPos().y)
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
        else if (pos.x < grid[0, 0].GetPos().x && pos.y < grid[0, (int)res.y - 1].GetPos().y)
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
        else if (pos.x > grid[(int)res.x - 1, 0].GetPos().x && pos.y < grid[0, (int)res.y - 1].GetPos().y)
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
        else if (pos.y > grid[0, 0].GetPos().y)
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
        else if (pos.y < grid[0, (int)res.y - 1].GetPos().y)
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
        else if (pos.x < grid[0, 0].GetPos().x)
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
        else if (pos.x > grid[(int)res.x - 1, 0].GetPos().x)
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

    public Tile[,] GetGrid()
    {
        return grid;
    }

    public Vector2 GetRes()
    {
        return res;
    }
}
