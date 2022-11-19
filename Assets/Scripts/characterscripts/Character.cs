using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics; // more optimized math, good for multithreading
using Unity.Collections;
using System.Reflection;

public class Character
{
    public enum TYPE_OF_CHARACTER
    {
        Enemy, Soldier, Player
    }
    public TYPE_OF_CHARACTER type;

    protected struct STATS
    {
        public float critChance; // calculate in percent (0.1 = 10%)
        public int maxHealth;
        public float gatheringArea;
        public int level;
        public int damage;
        public float attackSpeed; // lower is faster
        public float walkSpeed;
        public int healthGen;
    }

    public PositionHandler ph;

    protected GameObject go;
    protected SpriteManager sm;
    protected CollisionManager cm;
    protected Tile currTile;
    protected Health health;

    protected float lastXPos;
    protected float pivotHeightDiff;
    protected int direction;
    protected bool isDead;
    protected bool shouldBeRemoved;

    protected Vector2 boundingBoxOffset;
    protected Vector2 spawnTile;

    int pathCounter;
    List<float2> path;
    int nrOfSteps;

    public Character()
    {
        isDead = false;
        shouldBeRemoved = false;

        ph.posReached = true;

        path = new List<float2>();

        // Get the length of the struct
        FieldInfo[] fields = typeof(PathStruct).GetFields();
        nrOfSteps = fields.Length;
        pathCounter = nrOfSteps - 1;
    }

    public virtual void Update() { }

    public Vector2 GetPosition()
    {
        return go.transform.position;
    }

    public virtual void Destroy() { }

    public string GetName()
    {
        return go.name;
    }

    public void MarkTile(TYPE_OF_CHARACTER type)
    {
        Vector3 correctPivotToTilePos = new Vector3(go.transform.position.x, go.transform.position.y - pivotHeightDiff, go.transform.position.z);
        Tile newTile = GridManager.GetTileFromWorldPosition(correctPivotToTilePos);

        if (newTile != currTile)
        {
            currTile.DecreaseCharacters(this);
            GridManager.GetCharacterTiles(type).Remove(currTile);

            if (newTile != null)
            {
                newTile.IncreaseCharacters(this);
                GridManager.GetCharacterTiles(type).Add(newTile);

                currTile = newTile;
            }
        }
    }

    public bool IsWalking()
    {
        return sm.IsWalking();
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void UpdatePositionHandler(float walkSpeed)
    {
        // In
        ph.tilePosition = currTile.GetTilePosition();
        ph.speed = walkSpeed;
        ph.isDead = IsDead();
        ph.isAttacking = sm.IsAttacking();

        // Update path
        path.Clear();
        path.Add(ph.path.one);
        path.Add(ph.path.two);
        path.Add(ph.path.three);
        path.Add(ph.path.four);
        path.Add(ph.path.five);
        path.Add(ph.path.six);
        path.Add(ph.path.seven);
        path.Add(ph.path.eight);
        path.Add(ph.path.nine);
        path.Add(ph.path.ten);

        // Out
        if (ph.isIdle)
        {
            sm.StartIdle();
        }
        else if (ph.isWalking)
        {
            sm.StartWalking();
        }
        else if (ph.targetFound)
        {
            sm.StartAttacking();
            ph.targetFound = false;
        }
    }

    protected void WalkToNewPosition(float walkSpeed)
    {
        // Make sure that the path is not interrupted by a newly built building or other characters of the same type
        Tile checkTile = GridManager.GetTileFromWorldPosition(path[pathCounter]);
        if (checkTile != null)
        {
            if (checkTile.IsObjectPresent() || checkTile.IsCharacterPresent(type))
            {
                ph.posReached = true;
                pathCounter = 0;
                path.Clear();
                return;
            }
        }

        if (path[1].x != float.MaxValue)
        {
            // Notice how we adjust the height based on the pivot difference
            go.transform.position = new float3(Mathf.MoveTowards(go.transform.position.x, path[pathCounter].x, walkSpeed * Time.deltaTime),
                Mathf.MoveTowards(go.transform.position.y, path[pathCounter].y + pivotHeightDiff, walkSpeed * Time.deltaTime), 0);

            if (pathCounter == nrOfSteps - 1)
            {
                ph.posReached = true;
                pathCounter = 0;
                path.Clear();
            }
            else if (go.transform.position.x == path[pathCounter].x && go.transform.position.y == path[pathCounter].y + pivotHeightDiff)
            {
                pathCounter++;
            }
        }
        else
        {
            // Notice how we adjust the height based on the pivot difference
            go.transform.position = new float3(Mathf.MoveTowards(go.transform.position.x, path[0].x, walkSpeed * Time.deltaTime),
                Mathf.MoveTowards(go.transform.position.y, path[0].y + pivotHeightDiff, walkSpeed * Time.deltaTime), 0);
            
            if (go.transform.position.x == path[0].x && go.transform.position.y == path[0].y + pivotHeightDiff)
            {
                ph.posReached = true;
                pathCounter = 0;
                path.Clear();
            }
        }
    }

    protected void CheckDirection(TYPE_OF_CHARACTER type)
    {
        if (type == TYPE_OF_CHARACTER.Enemy)
        {
            if (go.transform.position.x < lastXPos && direction == 1)
            {
                direction = -1;
                sm.FlipX();
            }
            else if (go.transform.position.x > lastXPos && direction == -1)
            {
                direction = 1;
                sm.FlipX();
            }
        }
        else if (type == TYPE_OF_CHARACTER.Soldier)
        {
            if (go.transform.position.x > lastXPos && direction == -1)
            {
                direction = 1;
                sm.FlipX();
            }
            else if (go.transform.position.x < lastXPos && direction == 1)
            {
                direction = -1;
                sm.FlipX();
            }
        }

        lastXPos = go.transform.position.x;
    }

    public void SetPositionHandler(PositionHandler inPh)
    {
        ph = inPh;
    }

    // This struct is used as a list because Unity.jobs can not handle unmanaged types... if it works, it works
    // Just make sure that the struct has as many fields as float2s in the pathList
    public struct PathStruct
    {
        public float2 one;
        public float2 two;
        public float2 three;
        public float2 four;
        public float2 five;
        public float2 six;
        public float2 seven;
        public float2 eight;
        public float2 nine;
        public float2 ten;
    }

    public struct PositionHandler
    {
        public TYPE_OF_CHARACTER type;
        public PathStruct path;
        public float2 position;
        public float2 tilePosition;
        public bool targetFound;
        public bool isDead;
        public bool isAttacking;
        public bool isIdle;
        public bool isWalking;
        public bool posReached;
        public float speed;
        public float dist;

        public void UpdatePosition()
        {
            // Only search for targets if the character is alive
            if (!isDead && !isAttacking && posReached)
            {
                isIdle = false;
                isWalking = false;
                posReached = false;
                List<float2> pathList = new List<float2>();

                // Get the length of the struct
                FieldInfo[] fields = typeof(PathStruct).GetFields();
                int nrOfSteps = fields.Length;

                if (type == TYPE_OF_CHARACTER.Enemy)
                {
                    position = PathFinding.SearchForTarget(tilePosition, TYPE_OF_CHARACTER.Soldier, TYPE_OF_CHARACTER.Enemy, nrOfSteps, out targetFound, out pathList);
                }
                else if (type == TYPE_OF_CHARACTER.Soldier)
                {
                    position = PathFinding.SearchForTarget(tilePosition, TYPE_OF_CHARACTER.Enemy, TYPE_OF_CHARACTER.Soldier, nrOfSteps, out targetFound, out pathList);
                }

                float2 currWorldPos = GridManager.GetTile(tilePosition).GetWorldPos();
                if (currWorldPos.x == position.x && currWorldPos.y == position.y)
                {
                    isIdle = true;
                    posReached = true;
                }
                else if (!targetFound)
                {
                    isWalking = true;
                }

                if (pathList.Count > 0)
                {
                    path.one = pathList[0];
                    path.two = pathList[1];
                    path.three = pathList[2];
                    path.four = pathList[3];
                    path.five = pathList[4];
                    path.six = pathList[5];
                    path.seven = pathList[6];
                    path.eight = pathList[7];
                    path.nine = pathList[8];
                    path.ten = pathList[9];
                }
                else
                {
                    path.one = position;
                    path.two = new Vector2(float.MaxValue, float.MaxValue);
                }
            }
        }
    }
}
