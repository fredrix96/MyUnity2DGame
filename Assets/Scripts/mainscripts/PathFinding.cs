using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics; // more optimized math, good for multithreading
using UnityEngine;

public static class PathFinding
{
    static System.Random rand = new System.Random();

    public static float2 SearchForTarget(float2 startTilePosition, Character.TYPE_OF_CHARACTER target, Character.TYPE_OF_CHARACTER friend, int nrOfSteps, out bool targetFound, out List<float2> path)
    {
        if (friend == Character.TYPE_OF_CHARACTER.Soldier)
        {
            return SoldierSearchForTarget(startTilePosition, target, nrOfSteps, out targetFound, out path);
        }
        
        return EnemySearchForTarget(startTilePosition, target, nrOfSteps, out targetFound, out path);
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

    public static float2 SoldierSearchForTarget(float2 startTilePosition, Character.TYPE_OF_CHARACTER target, int nrOfSteps, out bool targetFound, out List<float2> path)
    {
        // Starting variables
        Tile startTile = GridManager.GetTile(startTilePosition);
        int pathSize = nrOfSteps;
        targetFound = false;
        path = new List<float2>();

        List<Tile> tilesWithCharacters = GridManager.GetCharacterTiles(target);

        // Nothing to hunt, stay put
        if (tilesWithCharacters.Count == 0)
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

        bool isDistanceUpdated;

        while (true)
        {
            startX = (int)currentTilePos.x - 1;
            endX = (int)currentTilePos.x + 1;

            startY = (int)currentTilePos.y - 1;
            endY = (int)currentTilePos.y + 1;

            isDistanceUpdated = false;

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

                    // Outisde of grid!
                    if (currTile == null)
                    {
                        continue;
                    }

                    // Skip tiles inside the current list
                    if (SearchListForTile(checkList, currTile.GetTilePosition()))
                    {
                        continue;
                    }

                    // Avoid buildings and other soldiers
                    if (currTile.IsObjectPresent() || currTile.IsCharacterPresent(Character.TYPE_OF_CHARACTER.Soldier))
                    {
                        continue;
                    }

                    float newDistance = Tools.CalculateFloatDistance(currTile.GetWorldPos(), destination);

                    // Try to avoid the building before reaching it by punishing the closest tiles values
                    Tile checkTile = GridManager.GetTile(new Vector2(currTile.GetTilePosition().x + 1, currTile.GetTilePosition().y));
                    Tile checkTile2 = GridManager.GetTile(new Vector2(currTile.GetTilePosition().x + 2, currTile.GetTilePosition().y));
                    if (checkTile.IsObjectPresent())
                    {
                        newDistance += GridManager.GetTileWidth();
                    }
                    else if (checkTile2.IsObjectPresent())
                    {
                        newDistance += GridManager.GetTileWidth() / 2;
                    }

                    // Is this tile closer to the target?
                    if (newDistance < distance)
                    {
                        distance = newDistance;
                        nextTile = currTile;
                        isDistanceUpdated = true;
                    }
                }
            }

            if (isDistanceUpdated)
            {
                currentTilePos = nextTile.GetTilePosition();
            }
            // If there is no better path to take
            else
            {
                bool done = false;
                int sideSteps = 3;

                // To get some variety when chosing the new direction
                int[] dirList = new int[]
                {
                    -sideSteps, sideSteps
                };

                int dir = rand.Next(dirList[0], dirList[1]);

                Tile neighborTile = GridManager.GetTile(new Vector2(currentTilePos.x, currentTilePos.y + dir));

                if (neighborTile != null)
                {
                    if (!neighborTile.IsObjectPresent())
                    {
                        done = true;
                        currentTilePos = neighborTile.GetTilePosition();
                        nextTile = neighborTile;
                    }
                }

                // Try opposite direction
                if (!done)
                {
                    neighborTile = GridManager.GetTile(new Vector2(currentTilePos.x, currentTilePos.y + (dir * -1)));

                    if (neighborTile != null)
                    {
                        if (!neighborTile.IsObjectPresent())
                        {
                            currentTilePos = neighborTile.GetTilePosition();
                            nextTile = neighborTile;
                        }
                    }
                }
            }

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

    public static float2 EnemySearchForTarget(float2 startTilePosition, Character.TYPE_OF_CHARACTER target, int nrOfSteps, out bool targetFound, out List<float2> path)
    {
        // Starting variables
        Tile startTile = GridManager.GetTile(startTilePosition);
        int pathSize = nrOfSteps;
        targetFound = false;
        path = new List<float2>();

        List<Tile> tilesWithCharacters = GridManager.GetCharacterTiles(target);
        List<Tile> tilesWithObjects = GridManager.GetObjectTiles();
        Tile playerTile = GridManager.GetPlayerTile();

        // Nothing to hunt, stay put
        if (tilesWithCharacters.Count == 0 && tilesWithObjects.Count == 0 && playerTile == null)
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
        }

        currentTile = playerTile;
        if (currentTile != null)
        {
            dist = Tools.CalculateVectorDistance(startTilePosition, currentTile.GetTilePosition());
            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                closestTarget = currentTile;
            }
        }

        float attackRange = 3.0f;
        float viewRange = 30.0f;
        if (shortestDistance <= attackRange)
        {
            targetFound = true;
        }
        else if (shortestDistance == float.MaxValue)
        {
            return startTilePosition;
        }
        // This is done to make sure that the enemies walk forward and do not gather on the same line
        else if (shortestDistance >= viewRange)
        {
            Tile neighborTile = GridManager.GetTile(new Vector2(startTilePosition.x - 1, startTilePosition.y));

            if (neighborTile != null)
            {
                return neighborTile.GetWorldPos();
            }
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

        while (true)
        {
            startX = (int)currentTilePos.x - 1;
            endX = (int)currentTilePos.x + 1;

            startY = (int)currentTilePos.y - 1;
            endY = (int)currentTilePos.y + 1;

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

                    if (currTile.IsCharacterPresent(Character.TYPE_OF_CHARACTER.Enemy))
                    {
                        continue;
                    }

                    float newDistance = Tools.CalculateFloatDistance(currTile.GetWorldPos(), destination);
                    if (newDistance < distance)
                    {
                        distance = newDistance;
                        nextTile = currTile;
                    }
                }
            }

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
