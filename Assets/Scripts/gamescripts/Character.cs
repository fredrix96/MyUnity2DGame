using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics; // more optimized math, good for multithreading

public class Character
{
    public enum TYPE_OF_CHARACTER
    {
        Enemy, Soldier
    }
    public TYPE_OF_CHARACTER type;
    public PositionHandler ph;

    protected GameObject go;
    protected SpriteManager sm;
    protected Graphics gfx;
    protected CollisionManager cm;
    protected Tile currTile;
    protected GridManager gm;
    protected Health health;

    protected Vector3 position;

    protected float pivotHeightDiff;
    protected int direction;
    protected int damage;
    protected float speed;
    protected bool targetFound;
    protected bool isDead;
    protected bool shouldBeRemoved;

    public Character()
    {
        targetFound = false;
        isDead = false;
        shouldBeRemoved = false;
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

    public void MarkTile()
    {
        Vector3 correctPivotToTilePos = new Vector3(position.x, position.y - pivotHeightDiff, position.z);
        Tile newTile = gm.GetTileFromWorldPosition(correctPivotToTilePos);

        if (newTile != currTile)
        {
            newTile.IncreaseCharacters(this);
            currTile.DecreaseCharacters(this);

            currTile = newTile;
        }
    }

    public void SetGameObjectPosition(Vector3 newPos)
    {
        go.transform.position = newPos;
    }

    public void SetPosition(Vector3 newPos)
    {
        position = newPos;
    }

    public bool IsWalking()
    {
        return sm.IsWalking();
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void UpdatePositionHandler()
    {
        // In
        ph.tilePosition = currTile.GetTilePosition();
        ph.speed = speed;
        ph.deltaTime = Time.deltaTime;
        ph.pivotHeightDiff = pivotHeightDiff;
        ph.isWalking = IsWalking();
        ph.levelLimits = gfx.GetLevelLimits();

        // Out
        targetFound = ph.targetFound;
        position = ph.position;
        if (ph.isIdle)
        {
            sm.StartIdle();
        }
    }

    public void SetPositionHandler(PositionHandler inPh)
    {
        ph = inPh;
    }

    public struct PositionHandler
    {
        public TYPE_OF_CHARACTER type;
        public float3 position;
        public float2 tilePosition;
        public float4 levelLimits;
        public bool targetFound;
        public bool isWalking;
        public bool isIdle;
        public float pivotHeightDiff;
        public float speed;
        public float deltaTime;

        public void UpdatePosition()
        {
            if (isWalking)
            {
                isIdle = false;
                float2 newPos;

                if (type is TYPE_OF_CHARACTER.Enemy)
                {
                    newPos = PathFinding.GetNextTile(tilePosition, typeof(Soldier), typeof(Enemy), out targetFound, false);

                    if (!targetFound)
                    {
                        newPos = PathFinding.GetNextTile(tilePosition, typeof(Player), typeof(Enemy), out targetFound, false);
                    }

                    // Notice how we adjust based on the pivot difference
                    position = new float3(Mathf.MoveTowards(position.x, newPos.x, speed * deltaTime),
                        Mathf.MoveTowards(position.y, newPos.y + pivotHeightDiff, speed * deltaTime), 0);
                }
                else if (type is TYPE_OF_CHARACTER.Soldier)
                {
                    newPos = PathFinding.GetNextTile(tilePosition, typeof(Enemy), typeof(Soldier), out targetFound, true);

                    // If the soldier has reached half of the field, then stop if there are no enemies nearby
                    if (!targetFound && position.x > levelLimits.y / 2)
                    {
                        isIdle = true;
                    }
                    else
                    {
                        // Notice how we adjust based on the pivot difference
                        position = new float3(Mathf.MoveTowards(position.x, newPos.x, speed * deltaTime),
                            Mathf.MoveTowards(position.y, newPos.y + pivotHeightDiff, speed * deltaTime), 0);
                    }
                }
            }
        }
    }
}
