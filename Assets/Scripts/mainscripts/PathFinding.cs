using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics; // more optimized math, good for multithreading
using UnityEngine;

public static class PathFinding
{
    public static float2 SearchForTarget(float2 startTilePosition, Character.TYPE_OF_CHARACTER target, Character.TYPE_OF_CHARACTER friend, int nrOfSteps, out bool targetFound, out List<float2> path)
    {
        // Starting variables
        Tile startTile = GridManager.GetTile(startTilePosition);
        int pathSize = nrOfSteps;
        targetFound = false;
        path = new List<float2>();

        List<Tile> tilesWithCharacters = GridManager.GetCharacterTiles(target);

        List<Tile> tilesWithObjects = new List<Tile>(); 
        if (friend == Character.TYPE_OF_CHARACTER.Enemy) tilesWithObjects = GridManager.GetObjectTiles();

        // Nothing to hunt, stay put
        if (tilesWithCharacters.Count == 0 && friend == Character.TYPE_OF_CHARACTER.Soldier)
        {
            return startTile.GetWorldPos();
        }
        else if (tilesWithCharacters.Count == 0 && tilesWithObjects.Count == 0 && friend == Character.TYPE_OF_CHARACTER.Enemy)
        {
            return startTile.GetWorldPos();
        }

        Tile currentTile = startTile;
        Tile closestTarget = currentTile;
        float dist = float.MinValue;
        float shortestDistance = float.MaxValue;

        // Look for target
        for (int i = 0; i < tilesWithCharacters.Count; i++)
        {
            currentTile = tilesWithCharacters[i];
            if (currentTile.IsCharacterPresent(target))
            {
                dist = Tools.CalculateVectorDistance(startTilePosition, currentTile.GetTilePosition());
                if (dist < shortestDistance)
                {
                    shortestDistance = dist;
                    closestTarget = currentTile;
                }
            }
        }

        // Let the enemies look for objects and the player
        if (friend == Character.TYPE_OF_CHARACTER.Enemy)
        {
            if (tilesWithObjects.Count != 0)
            {
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
        }

        float attackRange = 5.0f;
        if (shortestDistance <= attackRange)
        {
            targetFound = true;
        }
        else if (shortestDistance == float.MaxValue)
        {
            return startTilePosition;
        }

        float2 destination = closestTarget.GetWorldPos();

        // Look for the fastest path to the destination

        List<float2> destinationList = new List<float2>();
        List<float2> checkList = new List<float2>();
        float distance = float.MaxValue;
        Tile nextTile = startTile;

        float2 currentTilePos = startTilePosition;
        int startX = 0;
        int endX = 0;
        int startY = 0;
        int endY = 0;

        //Unity.Mathematics.Random rnd = new Unity.Mathematics.Random();
        //rnd.InitState();
        //bool down = Convert.ToBoolean(rnd.NextInt(0, 1));

        while (true)
        {
            startX = (int)currentTilePos.x - 1;
            endX = (int)currentTilePos.x + 1;

            startY = (int)currentTilePos.y - 1;
            endY = (int)currentTilePos.y + 1;

            //if (down)
            //{
            //    startY = (int)currentTilePos.y;
            //    endY = (int)currentTilePos.y + 1;
            //}
            //else
            //{
            //    startY = (int)currentTilePos.y - 1;
            //    endY = (int)currentTilePos.y;
            //}

            // Check neighbors
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    // Skip start tile
                    if (x == startTilePosition.x && y == startTilePosition.y)
                    {
                        continue;
                    }
                    // Skip current tile
                    else if (x == currentTilePos.x && y == currentTilePos.y)
                    {
                        continue;
                    }

                    Tile currTile = GridManager.GetTile(new Vector2(x, y));

                    if (currTile == null)
                    {
                        continue;
                    }

                    // Skip tiles inside the current list
                    if (SearchListForTile(checkList, currTile.GetTilePosition()))
                    {
                        continue;
                    }

                    if (friend == Character.TYPE_OF_CHARACTER.Soldier)
                    {
                        if (currTile.IsObjectPresent() || currTile.IsCharacterPresent(friend))
                        {
                            continue;
                        }
                    }
                    else if (friend == Character.TYPE_OF_CHARACTER.Enemy)
                    {
                        if (currTile.IsCharacterPresent(friend))
                        {
                            continue;
                        }
                    }

                    float newDistance = Tools.CalculateFloatDistance(currTile.GetWorldPos(), destination);
                    if (newDistance < distance)
                    {
                        distance = newDistance;
                        nextTile = currTile;
                    }
                }
            }
            
            // This is to prevent the algorithm to "backtrack"
            // Remember that the y-axis grows in the downward direction
            //if (nextTile.GetTilePosition().y > currentTilePos.y)
            //{
            //    down = true;
            //}
            //else
            //{
            //    down = false;
            //}

            currentTilePos = nextTile.GetTilePosition();

            checkList.Add(nextTile.GetTilePosition());
            destinationList.Add(nextTile.GetWorldPos());

            if (destinationList.Count >= pathSize)
            {
                break;
            }
        }

#if DEBUG
        if (Tools.DebugMode)
        {
            //Draw lines to visualize the path
            for (int i = 1; i < destinationList.Count; i++)
            {
                Vector2 start = new Vector3(destinationList[i - 1].x, destinationList[i - 1].y);
                Vector2 end = new Vector3(destinationList[i].x, destinationList[i].y);
                Debug.DrawLine(start, end, Color.white, 0.5f);
            }
        }
#endif

        path = destinationList;

        return destinationList[0];
    }

    static bool SearchListForTile(List<float2> list, float2 inPos)
    {
        bool found = false;

        foreach (var pos in list)
        {
            if (pos.x == inPos.x && pos.y == inPos.y)
            {
                found = true;
            }
        }

        return found;
    }

    static float2 FindPathAroundObject(Tile startTile)
    {
        Tile currTile = startTile;
        Tile oldTile = currTile;
        int counter = 0;
        while (counter != 10)
        {
            if (currTile != null)
            {
                if (currTile.IsObjectPresent())
                {
                    break;
                }
            }
            else return startTile.GetWorldPos();

            oldTile = currTile;
            currTile = GridManager.GetTile(new float2(currTile.GetTilePosition().x, currTile.GetTilePosition().y + 1));
            counter++;

            if (currTile == null)
            {
                return oldTile.GetWorldPos();
            }
        }

        return currTile.GetWorldPos();

        //bool openPathUp = false;
        //bool openPathDown = false;
        //int steps = 1;
        //
        //// If the next tile is inside the playarea and it does not have an object on it 
        //if (startTile.GetTilePosition().x + 1 < GridManager.GetRes().x && !GridManager.GetTile(new float2(startTile.GetTilePosition().x + 1, startTile.GetTilePosition().y)).IsObjectPresent())
        //{
        //    Tile nextTileXFront = GridManager.GetTile(new float2(startTile.GetTilePosition().x + 1, startTile.GetTilePosition().y));
        //    return nextTileXFront.GetWorldPos();
        //}
        //// If the next tile has an object on it. Find the shortest path around it 
        //else if (GridManager.GetTile(new float2(startTile.GetTilePosition().x + 1, startTile.GetTilePosition().y)).IsObjectPresent())
        //{
        //    while (!openPathUp && !openPathDown)
        //    {
        //        if (startTile.GetTilePosition().y + steps < GridManager.GetRes().y && !GridManager.GetTile(new float2(startTile.GetTilePosition().x + 1, startTile.GetTilePosition().y + steps)).IsObjectPresent())
        //        {
        //            openPathUp = true;
        //        }
        //        else if (startTile.GetTilePosition().y - steps > 0 && !GridManager.GetTile(new float2(startTile.GetTilePosition().x + 1, startTile.GetTilePosition().y - steps)).IsObjectPresent())
        //        {
        //            openPathDown = true;
        //        }
        //        else
        //        {
        //            steps++;
        //        }
        //    }
        //
        //    if (openPathUp)
        //    {
        //        Tile nextTileYUp = GridManager.GetTile(new float2(startTile.GetTilePosition().x, startTile.GetTilePosition().y + 1));
        //        return nextTileYUp.GetWorldPos();
        //    }
        //    else
        //    {
        //        Tile nextTileYUp = GridManager.GetTile(new float2(startTile.GetTilePosition().x, startTile.GetTilePosition().y - 1));
        //        return nextTileYUp.GetWorldPos();
        //    }
        //}
        //
        //// Do not leave the borders 
        //return startTile.GetWorldPos();
    }
}
