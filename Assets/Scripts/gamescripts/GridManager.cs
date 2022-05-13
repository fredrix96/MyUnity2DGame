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
        return grid[(int)tilePosition.x, (int)tilePosition.y];
    }

    public Tile GetTileFromWorldPosition(Vector2 pos)
    {
        Tile outTile = grid[0, 0];

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
            bool atThisTile = false;
        
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

    public Tile[,] GetGrid()
    {
        return grid;
    }

    public Vector2 GetRes()
    {
        return res;
    }
}
