using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding
{
    GridManager gm;

    public PathFinding(GridManager inGm)
    {
        gm = inGm;
    }

    // Find the closest target
    public Vector2 GetNextTile(Tile startTile, Type target, Type friend, out bool targetFound, bool right)
    {
        targetFound = false;

        if (!startTile.IsCharacterPresent(target))
        {
            // Prioritize the y-axis, then back, then front. Look if the target is on a neighboring tile
            Vector2 tmpPos = GetPosFromNeighboringTile(startTile.GetTilePosition(), target, friend, right);
            if (tmpPos.x != -1)
            {
                targetFound = true;
                return tmpPos;
            }

            // Else, look for the target along the y-axis 
            List<Tile> targets = new List<Tile>();
            for (int y = 0; y < gm.GetRes().y; y++)
            {
                // Do not look at the current tile
                if (y != startTile.GetTilePosition().y)
                {
                    Tile yTile = gm.GetTile(new Vector2(startTile.GetTilePosition().x, y));
                    if (yTile.IsCharacterPresent(target) && !yTile.IsCharacterPresent(friend))
                    {
                        // The closer the target is, the lower value it gets
                        float dist = Tools.CalculateDistance(startTile.GetTilePosition().y, yTile.GetTilePosition().y);
                        yTile.SetValue(dist);
                        targets.Add(yTile);
                    }
                }
            }

            // If targets were found, find the one with the shortest distance
            if (targets.Count > 0)
            {
                targetFound = true;

                targets.Sort(Tools.SortByValue);

                float shortestDist = targets[0].GetValue();

                // Find out if the target is closer on the x-axis if that distance is inside the grid
                if (right)
                {
                    if (startTile.GetTilePosition().x + shortestDist / 2 < gm.GetRes().x)
                    {
                        for (int x = (int)startTile.GetTilePosition().x + 1; x < (int)startTile.GetTilePosition().x + shortestDist / 2; x++)
                        {
                            // If the target is closer on the x-axis, move forward
                            Tile xTile = gm.GetTile(new Vector2(x, startTile.GetTilePosition().y));
                            if (xTile.IsCharacterPresent(target) && !xTile.IsCharacterPresent(friend))
                            {
                                Tile nextTileXFront = gm.GetTile(new Vector2(startTile.GetTilePosition().x + 1, startTile.GetTilePosition().y));
                                return nextTileXFront.GetPos();
                            }
                        }
                    }
                }
                else
                {
                    if (startTile.GetTilePosition().x - shortestDist / 2 > 0)
                    {
                        for (int x = (int)startTile.GetTilePosition().x - 1; x > (int)startTile.GetTilePosition().x - shortestDist / 2; x--)
                        {
                            // If the target is closer on the x-axis, move forward
                            Tile xTile = gm.GetTile(new Vector2(x, startTile.GetTilePosition().y));
                            if (xTile.IsCharacterPresent(target) && !xTile.IsCharacterPresent(friend))
                            {
                                Tile nextTileXFront = gm.GetTile(new Vector2(startTile.GetTilePosition().x - 1, startTile.GetTilePosition().y));
                                return nextTileXFront.GetPos();
                            }
                        }
                    }
                }

                // Else move to the closest target at the y-axis
                if (targets[0].GetTilePosition().y > startTile.GetTilePosition().y)
                {
                    Tile nextTileYUp = gm.GetTile(new Vector2(startTile.GetTilePosition().x, startTile.GetTilePosition().y + 1));
                    if (!nextTileYUp.IsCharacterPresent(friend))
                    {
                        return nextTileYUp.GetPos();
                    }
                }
                else
                {
                    Tile nextTileYDown = gm.GetTile(new Vector2(startTile.GetTilePosition().x, startTile.GetTilePosition().y - 1));
                    if (!nextTileYDown.IsCharacterPresent(friend))
                    {
                        return nextTileYDown.GetPos();
                    }
                }
            }
            // Else if no target was found, move to the next tile
            else
            {
                if (right)
                {
                    if (startTile.GetTilePosition().x + 1 < gm.GetRes().x)
                    {
                        Tile nextTileXFront = gm.GetTile(new Vector2(startTile.GetTilePosition().x + 1, startTile.GetTilePosition().y));
                        return nextTileXFront.GetPos();
                    }
                }
                else
                {
                    if (startTile.GetTilePosition().x - 1 > 0)
                    {
                        Tile nextTileXFront = gm.GetTile(new Vector2(startTile.GetTilePosition().x - 1, startTile.GetTilePosition().y));
                        return nextTileXFront.GetPos();
                    }
                }

                // Do not leave the borders
                return startTile.GetPos();
            }
        }

        // The target is on this tile
        targetFound = true;
        return startTile.GetPos();
    }

    Vector2 GetPosFromNeighboringTile(Vector2 inPos, Type target, Type friend, bool right)
    {
        Vector2 outPos = new Vector2(-1, -1);

        // Avoid tiles that are occupied by others
        if (inPos.y + 1 < gm.GetRes().y)
        {
            Tile nextTileYUp = gm.GetTile(new Vector2(inPos.x, inPos.y + 1));
            if (nextTileYUp.IsCharacterPresent(target) && !nextTileYUp.IsCharacterPresent(friend))
            {
                return nextTileYUp.GetPos();
            }
        }
        if (inPos.y - 1 > 0)
        {
            Tile nextTileYDown = gm.GetTile(new Vector2(inPos.x, inPos.y - 1));
            if (nextTileYDown.IsCharacterPresent(target) && !nextTileYDown.IsCharacterPresent(friend))
            {
                return nextTileYDown.GetPos();
            }
        }

        // Is the main direction to the right?
        if (right)
        {
            if (inPos.x - 1 > 0)
            {
                Tile nextTileXBack = gm.GetTile(new Vector2(inPos.x - 1, inPos.y));
                if (nextTileXBack.IsCharacterPresent(target) && !nextTileXBack.IsCharacterPresent(friend))
                {
                    return nextTileXBack.GetPos();
                }
            }
            else if (inPos.x + 1 < gm.GetRes().x)
            {
                Tile nextTileXFront = gm.GetTile(new Vector2(inPos.x + 1, inPos.y));
                if (nextTileXFront.IsCharacterPresent(target) && !nextTileXFront.IsCharacterPresent(friend))
                {
                    return nextTileXFront.GetPos();
                }
            }
        }
        // Is the main direction to the left?
        else
        {
            if (inPos.x + 1 < gm.GetRes().x)
            {
                Tile nextTileXFront = gm.GetTile(new Vector2(inPos.x + 1, inPos.y));
                if (nextTileXFront.IsCharacterPresent(target) && !nextTileXFront.IsCharacterPresent(friend))
                {
                    return nextTileXFront.GetPos();
                }
            }
            if (inPos.x - 1 > 0)
            {
                Tile nextTileXBack = gm.GetTile(new Vector2(inPos.x - 1, inPos.y));
                if (nextTileXBack.IsCharacterPresent(target) && !nextTileXBack.IsCharacterPresent(friend))
                {
                    return nextTileXBack.GetPos();
                }
            }
        }

        return outPos;
    }

    // Find the closest target by using the A* algorithm
    /*public Vector2 GetNextTile(Tile startTile)
    {
        // Initialize the open list
        List<Tile> openList = new List<Tile>();

        // Initialize the closed list
        List<Tile> closedList = new List<Tile>();

        // Put the starting node on the open list (you can leave its f (value) at zero)
        openList.Add(startTile);

        // Tiles searched
        int tilesSearched = 0;

        int searchDistance = (int)(gm.GetRes().y + gm.GetRes().y * 0.5f);

        Tile q = null;

        // While the open list is not empty
        while (openList.Count > 0 || tilesSearched != searchDistance)
        {
            // Find the node with the least value on the open list, call it "q"
            openList.Sort(Tools.SortByValue);

            // Pop q off the open list
            q = openList[0];
            closedList.Add(q);

            if (q.isEnemyPresent())
            {
                break;
            }

            // Generate q's 8 successors and set their parents to q
            int startY = (int)q.GetTilePosition().y - 1;
            int startX = (int)q.GetTilePosition().x - 1;
            for (int y = startY; y < startY + 3; y++)
            {
                // If the "y-row" is inside the grid
                if (y >= 0 && y < gm.GetRes().y)
                {
                    for (int x = startX; x < startX + 3; x++)
                    {
                        // If current x pos is inside the grid
                        if (x >= 0 && x < gm.GetRes().x)
                        {
                            // Do not count the parent node
                            if (x == (int)q.GetTilePosition().x && y == (int)q.GetTilePosition().y)
                            {
                                continue;
                            }
                            else
                            {
                                Vector2 pos = new Vector2(x, y);
                                Tile successor = gm.GetTile(pos);

                                // If successor is the goal, stop search
                                if (successor.isEnemyPresent())
                                {
                                    successor.SetF(-99999);
                                    closedList.Add(successor);

                                    goto found;
                                }
                                // Else, compute both g and h for successor
                                else
                                {
                                    // Successor.g = q.g + distance between successor and q
                                    float dist = Tools.CalculateDistance(startTile.GetTilePosition().x, successor.GetTilePosition().x);
                                    successor.SetG(-dist);
                                    successor.SetH(-(int)q.GetTilePosition().x);
                                    successor.SetF(successor.GetG() + successor.GetH());
                                }

                                // If a node with the same position as successor is in the OPEN list which has a lower f than successor, skip this successor
                                foreach (Tile openTile in openList)
                                {
                                    if (successor.GetPos() == openTile.GetPos())
                                    {
                                        goto skipTile;
                                    }
                                }

                                // If a node with the same position as successor is in the CLOSED list which has a lower f than successor, skip this successor
                                foreach (Tile closedTile in closedList)
                                {
                                    if (successor.GetPos() == closedTile.GetPos())
                                    {
                                        goto skipTile;
                                    }
                                }

                                // Add the node to the open list
                                openList.Add(successor);

                            skipTile:
                                bool uglyFix = true;
                            }
                        }
                    }
                }
            }
            tilesSearched++;

            openList.Remove(q);
        }

    found:

        // Find the node with the lowest f
        closedList.Sort(Tools.SortByValue);

        // Return the new position
        return closedList[0].GetPos();
    } */
}
