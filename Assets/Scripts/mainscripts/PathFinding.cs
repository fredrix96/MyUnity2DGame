using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics; // more optimized math, good for multithreading
using UnityEngine;

public static class PathFinding
{
    /*
    // Find the closest target
    public static float2 SearchForTarget(float2 startTilePosition, Type target, Type friend, out bool targetFound, bool right)
    {
        Tile startTile = GridManager.GetTile(startTilePosition);

        targetFound = false;

        if (!startTile.IsCharacterPresent(target))
        {
            // Prioritize the y-axis, then back, then front. Look if the target is on a neighboring tile
            float2 tmpPos = SearchTargetOnNeighboringTile(startTile.GetTilePosition(), target, friend, right);
            if (tmpPos.x != -1)
            {
                targetFound = true;
                return tmpPos;
            }

            // Else, look for the target along the y-axis 
            List<Tile> targets = new List<Tile>();
            for (int y = 0; y < GridManager.GetRes().y; y++)
            {
                // Do not look at the current tile
                if (y != startTile.GetTilePosition().y)
                {
                    Tile yTile = GridManager.GetTile(new float2(startTile.GetTilePosition().x, y));
                    if (yTile.IsCharacterPresent(target) && !yTile.IsCharacterPresent(friend) && !yTile.IsObjectPresent())
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

                // Find out if there is an obstacle between the character and its target
                if (LookForObstaclesBetweenTiles(targets[0], startTile))
                {
                    // Look for a tile with the most effective path
                    return FindMostEffectivePosition(startTile, right);
                }

                float shortestDist = targets[0].GetValue();

                // Find out if the target is closer on the x-axis if that distance is inside the grid
                if (right)
                {
                    if (startTile.GetTilePosition().x + shortestDist / 2 < GridManager.GetRes().x)
                    {
                        for (int x = (int)startTile.GetTilePosition().x + 1; x < (int)startTile.GetTilePosition().x + shortestDist / 2; x++)
                        {
                            // If the target is closer on the x-axis, move forward
                            Tile xTile = GridManager.GetTile(new float2(x, startTile.GetTilePosition().y));
                            if (xTile.IsCharacterPresent(target) && !xTile.IsCharacterPresent(friend) && !xTile.IsObjectPresent())
                            {
                                Tile nextTileXFront = GridManager.GetTile(new float2(startTile.GetTilePosition().x + 1, startTile.GetTilePosition().y));
                                return nextTileXFront.GetWorldPos();
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
                            Tile xTile = GridManager.GetTile(new float2(x, startTile.GetTilePosition().y));
                            if (xTile.IsCharacterPresent(target) && !xTile.IsCharacterPresent(friend) && !xTile.IsObjectPresent())
                            {
                                Tile nextTileXFront = GridManager.GetTile(new float2(startTile.GetTilePosition().x - 1, startTile.GetTilePosition().y));
                                return nextTileXFront.GetWorldPos();
                            }
                        }
                    }
                }

                // Else move to the closest target at the y-axis
                if (targets[0].GetTilePosition().y > startTile.GetTilePosition().y)
                {
                    Tile nextTileYUp = GridManager.GetTile(new float2(startTile.GetTilePosition().x, startTile.GetTilePosition().y + 1));
                    if (!nextTileYUp.IsCharacterPresent(friend) && !nextTileYUp.IsObjectPresent())
                    {
                        return nextTileYUp.GetWorldPos();
                    }
                }
                else
                {
                    Tile nextTileYDown = GridManager.GetTile(new float2(startTile.GetTilePosition().x, startTile.GetTilePosition().y - 1));
                    if (!nextTileYDown.IsCharacterPresent(friend) && !nextTileYDown.IsObjectPresent())
                    {
                        return nextTileYDown.GetWorldPos();
                    }
                }
            }
            // Else if no target was found, move to the next tile
            else
            {
                // Look for a tile with the most effective path
                return FindMostEffectivePosition(startTile, right);
            }
        }

        // The target is on this tile
        targetFound = true;
        return startTile.GetWorldPos();
    }

    public static float2 SearchForBuidling(float2 startTilePosition, Type friend, out bool buildingFound, bool right)
    {
        Tile startTile = GridManager.GetTile(startTilePosition);

        buildingFound = false;

        if (!startTile.IsObjectPresent())
        {
            // Prioritize the y-axis, then back, then front. Look if the target is on a neighboring tile
            float2 tmpPos = SearchBuildingOnNeighboringTile(startTile.GetTilePosition(), friend, right);
            if (tmpPos.x != -1)
            {
                buildingFound = true;
                return tmpPos;
            }

            // Else, look for the target along the y-axis 
            List<Tile> targets = new List<Tile>();
            for (int y = 0; y < GridManager.GetRes().y; y++)
            {
                // Do not look at the current tile
                if (y != startTile.GetTilePosition().y)
                {
                    Tile yTile = GridManager.GetTile(new float2(startTile.GetTilePosition().x, y));
                    if (yTile.IsObjectPresent() && !yTile.IsCharacterPresent(friend))
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
                buildingFound = true;

                targets.Sort(Tools.SortByValue);

                // Find out if there is an obstacle between the character and its target
                if (LookForObstaclesBetweenTiles(targets[0], startTile))
                {
                    // Look for a tile with the most effective path
                    return FindMostEffectivePosition(startTile, right);
                }

                float shortestDist = targets[0].GetValue();

                // Find out if the target is closer on the x-axis if that distance is inside the grid
                if (right)
                {
                    if (startTile.GetTilePosition().x + shortestDist / 2 < GridManager.GetRes().x)
                    {
                        for (int x = (int)startTile.GetTilePosition().x + 1; x < (int)startTile.GetTilePosition().x + shortestDist / 2; x++)
                        {
                            // If the target is closer on the x-axis, move forward
                            Tile xTile = GridManager.GetTile(new float2(x, startTile.GetTilePosition().y));
                            if (xTile.IsObjectPresent() && !xTile.IsCharacterPresent(friend))
                            {
                                Tile nextTileXFront = GridManager.GetTile(new float2(startTile.GetTilePosition().x + 1, startTile.GetTilePosition().y));
                                return nextTileXFront.GetWorldPos();
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
                            Tile xTile = GridManager.GetTile(new float2(x, startTile.GetTilePosition().y));
                            if (xTile.IsObjectPresent() && !xTile.IsCharacterPresent(friend))
                            {
                                Tile nextTileXFront = GridManager.GetTile(new float2(startTile.GetTilePosition().x - 1, startTile.GetTilePosition().y));
                                return nextTileXFront.GetWorldPos();
                            }
                        }
                    }
                }

                // Else move to the closest target at the y-axis
                if (targets[0].GetTilePosition().y > startTile.GetTilePosition().y)
                {
                    Tile nextTileYUp = GridManager.GetTile(new float2(startTile.GetTilePosition().x, startTile.GetTilePosition().y + 1));
                    if (nextTileYUp.IsObjectPresent() && !nextTileYUp.IsCharacterPresent(friend))
                    {
                        return nextTileYUp.GetWorldPos();
                    }
                }
                else
                {
                    Tile nextTileYDown = GridManager.GetTile(new float2(startTile.GetTilePosition().x, startTile.GetTilePosition().y - 1));
                    if (nextTileYDown.IsObjectPresent() && !nextTileYDown.IsCharacterPresent(friend))
                    {
                        return nextTileYDown.GetWorldPos();
                    }
                }
            }
            // Else if no target was found, move to the next tile
            else
            {
                // Look for a tile with the most effective path
                return FindMostEffectivePosition(startTile, right);
            }
        }

        // The target is on this tile
        buildingFound = true;
        return startTile.GetWorldPos();
    }

    static bool LookForObstaclesBetweenTiles(Tile tile1, Tile tile2)
    {
        bool obstacle = false;

        if (tile1.GetTilePosition().y < tile2.GetTilePosition().y)
        {
            for (int y = (int)tile1.GetTilePosition().y; y < (int)tile2.GetTilePosition().y; y++)
            {
                Tile tile = GridManager.GetTile(new float2(tile2.GetTilePosition().x, y));
                if (tile.IsObjectPresent())
                {
                    obstacle = true;
                }
            }
        }
        else
        {
            for (int y = (int)tile2.GetTilePosition().y; y < (int)tile1.GetTilePosition().y; y++)
            {
                Tile tile = GridManager.GetTile(new float2(tile2.GetTilePosition().x, y));
                if (tile.IsObjectPresent())
                {
                    obstacle = true;

                }
            }
        }

        return obstacle;
    }

    static float2 SearchTargetOnNeighboringTile(float2 inPos, Type target, Type friend, bool right)
    {
        float2 outPos = new float2(-1, -1);

        // Avoid tiles that are occupied by others
        if (inPos.y + 1 < GridManager.GetRes().y)
        {
            Tile nextTileYUp = GridManager.GetTile(new float2(inPos.x, inPos.y + 1));
            if (nextTileYUp.IsCharacterPresent(target) && !nextTileYUp.IsCharacterPresent(friend) && !nextTileYUp.IsObjectPresent())
            {
                return nextTileYUp.GetWorldPos();
            }
        }
        if (inPos.y - 1 > 0)
        {
            Tile nextTileYDown = GridManager.GetTile(new float2(inPos.x, inPos.y - 1));
            if (nextTileYDown.IsCharacterPresent(target) && !nextTileYDown.IsCharacterPresent(friend) && !nextTileYDown.IsObjectPresent())
            {
                return nextTileYDown.GetWorldPos();
            }
        }

        // Is the main direction to the right?
        if (right)
        {
            if (inPos.x - 1 > 0)
            {
                Tile nextTileXBack = GridManager.GetTile(new float2(inPos.x - 1, inPos.y));
                if (nextTileXBack.IsCharacterPresent(target) && !nextTileXBack.IsCharacterPresent(friend) && !nextTileXBack.IsObjectPresent())
                {
                    return nextTileXBack.GetWorldPos();
                }
            }
            else if (inPos.x + 1 < GridManager.GetRes().x)
            {
                Tile nextTileXFront = GridManager.GetTile(new float2(inPos.x + 1, inPos.y));
                if (nextTileXFront.IsCharacterPresent(target) && !nextTileXFront.IsCharacterPresent(friend) && !nextTileXFront.IsObjectPresent())
                {
                    return nextTileXFront.GetWorldPos();
                }
            }
        }
        // Is the main direction to the left?
        else
        {
            if (inPos.x + 1 < GridManager.GetRes().x)
            {
                Tile nextTileXFront = GridManager.GetTile(new float2(inPos.x + 1, inPos.y));
                if (nextTileXFront.IsCharacterPresent(target) && !nextTileXFront.IsCharacterPresent(friend) && !nextTileXFront.IsObjectPresent())
                {
                    return nextTileXFront.GetWorldPos();
                }
            }
            if (inPos.x - 1 > 0)
            {
                Tile nextTileXBack = GridManager.GetTile(new float2(inPos.x - 1, inPos.y));
                if (nextTileXBack.IsCharacterPresent(target) && !nextTileXBack.IsCharacterPresent(friend) && !nextTileXBack.IsObjectPresent())
                {
                    return nextTileXBack.GetWorldPos();
                }
            }
        }

        return outPos;
    }

    static float2 SearchBuildingOnNeighboringTile(float2 inPos, Type friend, bool right)
    {
        float2 outPos = new float2(-1, -1);

        // Avoid tiles that are occupied by others
        if (inPos.y + 1 < GridManager.GetRes().y)
        {
            Tile nextTileYUp = GridManager.GetTile(new float2(inPos.x, inPos.y + 1));
            if (nextTileYUp.IsObjectPresent() && !nextTileYUp.IsCharacterPresent(friend))
            {
                return nextTileYUp.GetWorldPos();
            }
        }
        if (inPos.y - 1 > 0)
        {
            Tile nextTileYDown = GridManager.GetTile(new float2(inPos.x, inPos.y - 1));
            if (nextTileYDown.IsObjectPresent() && !nextTileYDown.IsCharacterPresent(friend))
            {
                return nextTileYDown.GetWorldPos();
            }
        }

        // Is the main direction to the right?
        if (right)
        {
            if (inPos.x - 1 > 0)
            {
                Tile nextTileXBack = GridManager.GetTile(new float2(inPos.x - 1, inPos.y));
                if (nextTileXBack.IsObjectPresent() && !nextTileXBack.IsCharacterPresent(friend))
                {
                    return nextTileXBack.GetWorldPos();
                }
            }
            else if (inPos.x + 1 < GridManager.GetRes().x)
            {
                Tile nextTileXFront = GridManager.GetTile(new float2(inPos.x + 1, inPos.y));
                if (nextTileXFront.IsObjectPresent() && !nextTileXFront.IsCharacterPresent(friend))
                {
                    return nextTileXFront.GetWorldPos();
                }
            }
        }
        // Is the main direction to the left?
        else
        {
            if (inPos.x + 1 < GridManager.GetRes().x)
            {
                Tile nextTileXFront = GridManager.GetTile(new float2(inPos.x + 1, inPos.y));
                if (nextTileXFront.IsObjectPresent())
                {
                    return nextTileXFront.GetWorldPos();
                }
            }
            if (inPos.x - 1 > 0)
            {
                Tile nextTileXBack = GridManager.GetTile(new float2(inPos.x - 1, inPos.y));
                if (nextTileXBack.IsObjectPresent())
                {
                    return nextTileXBack.GetWorldPos();
                }
            }
        }

        return outPos;
    }
    */

    // Pseudo code: https://www.researchgate.net/figure/A-search-algorithm-Pseudocode-of-the-A-search-algorithm-operating-with-open-and-closed_fig8_232085273
    public static float2 SearchForTargetAStar(float2 startTilePosition, Character.TYPE_OF_CHARACTER target, Character.TYPE_OF_CHARACTER friend, out bool targetFound, bool right)
    {
        Tile startTile = GridManager.GetTile(startTilePosition);

        targetFound = false;

        float shortestDistance = 99999;
        Tile closestTarget = startTile;

        List<Tile> tilesWithCharacters = GridManager.GetCharacterTiles(target);
        Tile currentTile = startTile;
        float dist = 0;
        for (int i = 0; i < tilesWithCharacters.Count; i++)
        {
            currentTile = tilesWithCharacters[i];
            dist = Tools.CalculateVectorDistance(startTilePosition, currentTile.GetTilePosition());
            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                closestTarget = currentTile;
            }
        }

        if (friend == Character.TYPE_OF_CHARACTER.Enemy)
        {
            List<Tile> tilesWithObjects = GridManager.GetObjectTiles();
            for (int i = 0; i < tilesWithObjects.Count; i++)
            {
                currentTile = tilesWithObjects[i];
                dist = Tools.CalculateVectorDistance(startTilePosition, currentTile.GetTilePosition());
                if (dist < shortestDistance)
                {
                    shortestDistance = dist;
                    closestTarget = currentTile;
                }
            }

            currentTile = GridManager.GetPlayerTile();
            if (currentTile != null)
            {
                dist = Tools.CalculateVectorDistance(startTilePosition, currentTile.GetTilePosition());
                if (dist < shortestDistance)
                {
                    shortestDistance = dist;
                    closestTarget = currentTile;
                }
            }
        }

        // No targets available, don't move
        if (shortestDistance == 99999)
        {
            return startTile.GetWorldPos();
        }

        float3 maxValues = new float3(0, 0, 9999); // x = g, y = h, z = f

        // Open list
        List<Tuple<Tile, float3>> openList = new List<Tuple<Tile, float3>>();
        Tuple<Tile, float3> start = new Tuple<Tile, float3>(startTile, maxValues);
        openList.Add(start);

        // Closed list
        List<Tuple<Tile, float3>> closedList = new List<Tuple<Tile, float3>>();

        Tuple<Tile, float3> currentTuple = new Tuple<Tile, float3>(startTile, maxValues);

        int maxItr = 10;
        int counter = 0;

        Vector2 currentTilePos = startTilePosition;

        while (openList.Count > 0 && counter < maxItr)
        {
            openList = openList.OrderByDescending(t => t.Item2.z).ToList();

            currentTuple = openList[openList.Count - 1];

            closedList.Add(currentTuple);

            // To lower the function calls
            currentTile = currentTuple.Item1;
            currentTilePos = currentTile.GetTilePosition();

            if (currentTile.IsCharacterPresent(target) ||
                (currentTile.IsObjectPresent() && friend == Character.TYPE_OF_CHARACTER.Enemy))
            {
                currentTile = closedList[closedList.Count - 1].Item1;
                targetFound = true;
                break;
            }
            else
            {
                openList.Remove(currentTuple);
                
                int startX = (int)currentTilePos.x - 1;
                int endX = (int)currentTilePos.x + 1;
                int startY = (int)currentTilePos.y - 1;
                int endY = (int)currentTilePos.y + 1;
                
                // Check neighbors
                for (int x = startX; x <= endX; x++)
                {
                    for (int y = startY; y <= endY; y++)
                    {
                        // Skip current tile
                        if (x == (int)currentTilePos.x && y == (int)currentTilePos.y)
                        {
                            continue;
                        }
                
                        Tile neighborTile = GridManager.GetTile(new float2(x, y));

                        // Skip the neighbor if it is outside the area
                        if (neighborTile == null)
                        {
                            continue;
                        }
                        else
                        {
                            float3 values;
                            bool inClosedList = SearchListForTile(closedList, neighborTile.GetName(), out values);
                            bool inOpenList = false;
                            if (!inClosedList)
                            {
                                inOpenList = SearchListForTile(openList, neighborTile.GetName(), out values);
                            }
                            else
                            {
                                continue;
                            }

                            if (!inClosedList && !inOpenList)
                            {
                                values = maxValues;
                            }
                
                            Tuple<Tile, float3> neighborTuple = new Tuple<Tile, float3>(neighborTile, values);

                            float cost = neighborTuple.Item2.x + Tools.CalculateVectorDistance(neighborTile.GetTilePosition(), closestTarget.GetTilePosition());
                
                            if (inOpenList && cost < neighborTuple.Item2.x)
                            {
                                openList.Remove(neighborTuple);
                            }
                
                            if (inClosedList && cost < neighborTuple.Item2.x)
                            {
                                closedList.Remove(neighborTuple);
                            }
                
                            if (!inOpenList && !inClosedList)
                            {
                                float g = cost;
                                float h = CalculateHeuristic(neighborTuple.Item1, closestTarget, ref shortestDistance, target, friend);
                                float f = g + h;
                
                                float3 newValues = new float3(g, h, f);

                                Tuple<Tile, float3> modifiedChild = new Tuple<Tile, float3>(neighborTile, newValues);
                
                                openList.Add(modifiedChild);
                            }
                        }
                    }
                }
            }

            counter++;
        }

        // Extra precaution to not walk through objects
        if (friend == Character.TYPE_OF_CHARACTER.Soldier)
        {
            Tile nextTileFrontX = GridManager.GetTile(new float2(startTilePosition.x + 1, startTilePosition.y));
            Tile nextTileBackX = GridManager.GetTile(new float2(startTilePosition.x - 1, startTilePosition.y));
            Tile nextTileUpY = GridManager.GetTile(new float2(startTilePosition.x, startTilePosition.y - 1));
            Tile nextTileDownY = GridManager.GetTile(new float2(startTilePosition.x, startTilePosition.y + 1));

            if (nextTileFrontX != null)
            {
                if (nextTileFrontX.IsObjectPresent())
                {
                    return FindMostEffectivePosition(startTile, right);
                }
            }
        
            if (nextTileBackX != null)
            {
                if (nextTileBackX.IsObjectPresent())
                {
                    return FindMostEffectivePosition(startTile, right);
                }
            }
        
            if (nextTileUpY != null)
            {
                if (nextTileUpY.IsObjectPresent())
                {
                    return FindMostEffectivePosition(startTile, right);
                }
            }
        
            if (nextTileDownY != null)
            {
                if (nextTileDownY.IsObjectPresent())
                {
                    return FindMostEffectivePosition(startTile, right);
                }
            }
        }

        return currentTile.GetWorldPos();
    }

    static bool SearchListForTile(List<Tuple<Tile, float3>> list, string nameToFind, out float3 valuesOut)
    {
        bool found = false;
        valuesOut = float3.zero;

        foreach (var tuple in list)
        {
            if (tuple.Item1.GetName() == nameToFind)
            {
                found = true;
                valuesOut = tuple.Item2;
            }
        }

        return found;
    }

    static float CalculateHeuristic(Tile neighborTile, Tile closestTarget, ref float shortestDistance, Character.TYPE_OF_CHARACTER target, Character.TYPE_OF_CHARACTER friend)
    {
        float distx = GridManager.GetTileDistance().x;
        float newDist = Tools.CalculateVectorDistance(neighborTile.GetTilePosition(), closestTarget.GetTilePosition());
        float heuristic = 0;

        if (neighborTile.IsCharacterPresent(friend))
        {
            heuristic = 6 * distx;
        }
        else if (neighborTile.IsObjectPresent())
        {
            if (friend == Character.TYPE_OF_CHARACTER.Soldier) heuristic = 7 * distx;
            else if (friend == Character.TYPE_OF_CHARACTER.Enemy) heuristic = 2 * distx;
        }
        else if (neighborTile.IsCharacterPresent(Character.TYPE_OF_CHARACTER.Player) && friend == Character.TYPE_OF_CHARACTER.Enemy)
        {
            heuristic = distx;
        }
        else if (newDist < shortestDistance)
        {
            heuristic = 4 * distx;
            shortestDistance = newDist;
        }
        else if (neighborTile.IsCharacterPresent(target))
        {
            heuristic = 3 * distx;
        }
        else
        {
            heuristic = 5 * distx;
        }

        return Mathf.Pow(heuristic, 2) * 10;
    }

    static float2 FindMostEffectivePosition(Tile startTile, bool right)
    {
        bool openPathUp = false;
        bool openPathDown = false;
        int steps = 1;

        // If the character moves to the right
        if (right)
        {
            // If the next tile is inside the playarea and it does not have an object on it
            if (startTile.GetTilePosition().x + 1 < GridManager.GetRes().x && !GridManager.GetTile(new float2(startTile.GetTilePosition().x + 1, startTile.GetTilePosition().y)).IsObjectPresent())
            {
                Tile nextTileXFront = GridManager.GetTile(new float2(startTile.GetTilePosition().x + 1, startTile.GetTilePosition().y));
                return nextTileXFront.GetWorldPos();
            }
            // If the next tile has an object on it. Find the shortest path around it
            else if (GridManager.GetTile(new float2(startTile.GetTilePosition().x + 1, startTile.GetTilePosition().y)).IsObjectPresent())
            {
                while (!openPathUp && !openPathDown)
                {
                    if (startTile.GetTilePosition().y + steps < GridManager.GetRes().y && !GridManager.GetTile(new float2(startTile.GetTilePosition().x + 1, startTile.GetTilePosition().y + steps)).IsObjectPresent())
                    {
                        openPathUp = true;
                    }
                    else if (startTile.GetTilePosition().y - steps > 0 && !GridManager.GetTile(new float2(startTile.GetTilePosition().x + 1, startTile.GetTilePosition().y - steps)).IsObjectPresent())
                    {
                        openPathDown = true;
                    }
                    else
                    {
                        steps++;
                    }
                }

                if (openPathUp)
                {
                    Tile nextTileYUp = GridManager.GetTile(new float2(startTile.GetTilePosition().x, startTile.GetTilePosition().y + 1));
                    return nextTileYUp.GetWorldPos();
                }
                else
                {
                    Tile nextTileYUp = GridManager.GetTile(new float2(startTile.GetTilePosition().x, startTile.GetTilePosition().y - 1));
                    return nextTileYUp.GetWorldPos();
                }
            }
        }
        // If the character moves to the left
        else
        {
            // If the next tile is inside the playarea and it does not have an object on it
            if (startTile.GetTilePosition().x - 1 > 0 && !GridManager.GetTile(new float2(startTile.GetTilePosition().x - 1, startTile.GetTilePosition().y)).IsObjectPresent())
            {
                Tile nextTileXFront = GridManager.GetTile(new float2(startTile.GetTilePosition().x - 1, startTile.GetTilePosition().y));
                return nextTileXFront.GetWorldPos();
            }
            // If the next tile has an object on it. Find the shortest path around it
            else if (GridManager.GetTile(new float2(startTile.GetTilePosition().x - 1, startTile.GetTilePosition().y)).IsObjectPresent())
            {
                while (!openPathUp && !openPathDown)
                {
                    if (startTile.GetTilePosition().y + steps < GridManager.GetRes().y && !GridManager.GetTile(new float2(startTile.GetTilePosition().x - 1, startTile.GetTilePosition().y + steps)).IsObjectPresent())
                    {
                        openPathUp = true;
                    }
                    else if (startTile.GetTilePosition().y - steps > 0 && !GridManager.GetTile(new float2(startTile.GetTilePosition().x - 1, startTile.GetTilePosition().y - steps)).IsObjectPresent())
                    {
                        openPathDown = true;
                    }
                    else
                    {
                        steps++;
                    }
                }

                if (openPathUp)
                {
                    Tile nextTileYUp = GridManager.GetTile(new float2(startTile.GetTilePosition().x, startTile.GetTilePosition().y + 1));
                    return nextTileYUp.GetWorldPos();
                }
                else
                {
                    Tile nextTileYDown = GridManager.GetTile(new float2(startTile.GetTilePosition().x, startTile.GetTilePosition().y - 1));
                    return nextTileYDown.GetWorldPos();
                }
            }
        }

        // Do not leave the borders
        return startTile.GetWorldPos();
    }

    public static float2 SearchForTarget(float2 startTilePosition, Character.TYPE_OF_CHARACTER target, Character.TYPE_OF_CHARACTER friend, out bool targetFound, bool right)
    {
        Tile startTile = GridManager.GetTile(startTilePosition);

        targetFound = false;

        if (startTile.IsCharacterPresent(target) && friend == Character.TYPE_OF_CHARACTER.Soldier)
        {
            targetFound = true;
            return startTilePosition;
        }
        else if (friend == Character.TYPE_OF_CHARACTER.Enemy)
        {
            if (startTile.IsCharacterPresent(target) || startTile.IsObjectPresent() || startTile.IsCharacterPresent(Character.TYPE_OF_CHARACTER.Player))
            {
                targetFound = true;
                return startTilePosition;
            }
        }

        Vector2 currentTilePos = startTilePosition;

        int gridLength = 10;
        int startX = (int)currentTilePos.x - gridLength;
        int endX = (int)currentTilePos.x + gridLength;

        // Check neighbors
        Tile currentTile = startTile;
        List<float2> targetPositions = new List<float2>();
        for (int x = startX; x <= endX; x++)
        {
            for (int y = 0; y < GridManager.GetRes().y; y++)
            {
                currentTile = GridManager.GetTile(new Vector2(x, y));

                // If the tile is inside the grid
                if (currentTile != null)
                {
                    if (currentTile.IsCharacterPresent(target) && friend == Character.TYPE_OF_CHARACTER.Soldier)
                    {
                        targetPositions.Add(currentTile.GetWorldPos());
                    }
                    else if (friend == Character.TYPE_OF_CHARACTER.Enemy)
                    {
                        if (currentTile.IsCharacterPresent(target) || currentTile.IsObjectPresent() || currentTile.IsCharacterPresent(Character.TYPE_OF_CHARACTER.Player))
                        {
                            targetPositions.Add(currentTile.GetWorldPos());
                        }
                    }
                }
            }
        }

        float2 nextPos = startTile.GetWorldPos();
        List<Tuple<float2, float>> distList = new List<Tuple<float2, float>>();
        if (targetPositions.Count > 0)
        {
            targetFound = true;

            foreach (float2 position in targetPositions)
            {
                Tuple<float2, float> tmp = new Tuple<float2, float>(position, Tools.CalculateVectorDistance(startTilePosition, position));
                distList.Add(tmp);
            }

            distList = distList.OrderByDescending(t => t.Item2).ToList();
            nextPos = distList[0].Item1;
        }
        else
        {
            //nextPos = GetClosestTargetPosition(startTile, target, friend);
            nextPos = SearchFreeTile(startTilePosition, friend);
        }

        // Extra precaution to not walk through objects
        if (friend == Character.TYPE_OF_CHARACTER.Soldier)
        {
            Tile nextTileFrontX = GridManager.GetTile(new float2(startTilePosition.x + 1, startTilePosition.y));
            if (nextTileFrontX != null)
            {
                if (nextTileFrontX.IsObjectPresent())
                {
                    return FindMostEffectivePosition(startTile, right);
                }
            }

            Tile nextTileBackX = GridManager.GetTile(new float2(startTilePosition.x - 1, startTilePosition.y));
            if (nextTileBackX != null)
            {
                if (nextTileBackX.IsObjectPresent())
                {
                    return FindMostEffectivePosition(startTile, right);
                }
            }

            Tile nextTileUpY = GridManager.GetTile(new float2(startTilePosition.x, startTilePosition.y - 1));
            if (nextTileUpY != null)
            {
                if (nextTileUpY.IsObjectPresent())
                {
                    return FindMostEffectivePosition(startTile, right);
                }
            }

            Tile nextTileDownY = GridManager.GetTile(new float2(startTilePosition.x, startTilePosition.y + 1));
            if (nextTileDownY != null)
            {
                if (nextTileDownY.IsObjectPresent())
                {
                    return FindMostEffectivePosition(startTile, right);
                }
            }
        }

        return nextPos;
    }

    static float2 SearchFreeTile(float2 inPos, Character.TYPE_OF_CHARACTER friend)
    {
        float2 outPos = inPos;

        // Avoid tiles that are occupied by others
        if (friend == Character.TYPE_OF_CHARACTER.Soldier)
        {
            Tile nextTileXFront = GridManager.GetTile(new float2(inPos.x + 1, inPos.y));
            if (nextTileXFront != null)
            {
                if (!nextTileXFront.IsCharacterPresent(friend) && !nextTileXFront.IsObjectPresent())
                {
                    return nextTileXFront.GetWorldPos();
                }
            }

            Tile nextTileXBack = GridManager.GetTile(new float2(inPos.x - 1, inPos.y));
            if (nextTileXBack != null)
            {
                if (!nextTileXBack.IsCharacterPresent(friend) && !nextTileXBack.IsObjectPresent())
                {
                    return nextTileXBack.GetWorldPos();
                }
            }
        }
        else
        {
            Tile nextTileXFront = GridManager.GetTile(new float2(inPos.x - 1, inPos.y));
            if (nextTileXFront != null)
            {
                if (!nextTileXFront.IsCharacterPresent(friend) && !nextTileXFront.IsObjectPresent())
                {
                    return nextTileXFront.GetWorldPos();
                }
            }
            Tile nextTileXBack = GridManager.GetTile(new float2(inPos.x + 1, inPos.y));
            if (nextTileXBack != null)
            {
                if (!nextTileXBack.IsCharacterPresent(friend) && !nextTileXBack.IsObjectPresent())
                {
                    return nextTileXBack.GetWorldPos();
                }
            }
        }

        if (inPos.y + 1 < GridManager.GetRes().y)
        {
            Tile nextTileYUp = GridManager.GetTile(new float2(inPos.x, inPos.y + 1));
            if (nextTileYUp != null)
            {
                if (!nextTileYUp.IsCharacterPresent(friend) && !nextTileYUp.IsObjectPresent())
                {
                    return nextTileYUp.GetWorldPos();
                }
            }
        }
        if (inPos.y - 1 > 0)
        {
            Tile nextTileYDown = GridManager.GetTile(new float2(inPos.x, inPos.y - 1));
            if (nextTileYDown != null)
            {
                if (!nextTileYDown.IsCharacterPresent(friend) && !nextTileYDown.IsObjectPresent())
                {
                    return nextTileYDown.GetWorldPos();
                }
            }
        }

        return outPos;
    }

    static float2 GetClosestTargetPosition(Tile startTile, Character.TYPE_OF_CHARACTER target, Character.TYPE_OF_CHARACTER friend)
    {
        float shortestDistance = 99999;
        Tile closestTarget = startTile;
        Tile currentTile = startTile;
        float2 startTilePosition = startTile.GetTilePosition();

        List<Tile> tilesWithCharacters = GridManager.GetCharacterTiles(target);
        float dist = 0;
        for (int i = 0; i < tilesWithCharacters.Count; i++)
        {
            currentTile = tilesWithCharacters[i];
            dist = Tools.CalculateVectorDistance(startTilePosition, currentTile.GetTilePosition());
            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                closestTarget = currentTile;
            }
        }

        if (friend == Character.TYPE_OF_CHARACTER.Enemy)
        {
            List<Tile> tilesWithObjects = GridManager.GetObjectTiles();
            for (int i = 0; i < tilesWithObjects.Count; i++)
            {
                currentTile = tilesWithObjects[i];
                dist = Tools.CalculateVectorDistance(startTilePosition, currentTile.GetTilePosition());
                if (dist < shortestDistance)
                {
                    shortestDistance = dist;
                    closestTarget = currentTile;
                }
            }

            currentTile = GridManager.GetPlayerTile();
            if (currentTile != null)
            {
                dist = Tools.CalculateVectorDistance(startTilePosition, currentTile.GetTilePosition());
                if (dist < shortestDistance)
                {
                    shortestDistance = dist;
                    closestTarget = currentTile;
                }
            }
        }

        // No targets available, don't move
        if (shortestDistance == 99999)
        {
            return startTile.GetWorldPos();
        }

        return closestTarget.GetWorldPos();
    }
}
