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
        Vector3 correctPivotToTilePos = new Vector3(go.transform.position.x, go.transform.position.y - pivotHeightDiff, go.transform.position.z);
        Tile newTile = gm.GetTileFromWorldPosition(correctPivotToTilePos);

        if (newTile != currTile)
        {
            currTile.DecreaseCharacters(this);

            if (newTile != null)
            {
                newTile.IncreaseCharacters(this);

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

    public void UpdatePositionHandler()
    {
        // In
        ph.tilePosition = currTile.GetTilePosition();
        ph.speed = speed;
        ph.isDead = IsDead();
        ph.isAttacking = sm.IsAttacking();
        ph.levelLimits = gfx.GetLevelLimits();

        // Out
        targetFound = ph.targetFound;
        if (ph.isIdle)
        {
            sm.StartIdle();
        }
    }

    protected void WalkToNewPosition()
    {
        // Notice how we adjust based on the pivot difference
        go.transform.position = new float3(Mathf.MoveTowards(go.transform.position.x, ph.position.x, speed * Time.deltaTime),
            Mathf.MoveTowards(go.transform.position.y, ph.position.y + pivotHeightDiff, speed * Time.deltaTime), 0);
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
        public bool isDead;
        public bool isAttacking;
        public bool isIdle;
        public float speed;

        public void UpdatePosition()
        {
            // Only search for targets if the character is alive
            if (!isDead && !isAttacking)
            {
                isIdle = false;
                float2 newPos;

                if (type is TYPE_OF_CHARACTER.Enemy)
                {
                    //newPos = PathFinding.SearchForTarget(tilePosition, typeof(Soldier), typeof(Enemy), out targetFound, false);
                    newPos = PathFinding.SearchForTargetAStar(tilePosition, typeof(Soldier), typeof(Enemy), out targetFound, false);

                    //if (!targetFound)
                    //{
                    //    newPos = PathFinding.SearchForTarget(tilePosition, typeof(Player), typeof(Enemy), out targetFound, false);
                    //}
                    //
                    //if (!targetFound)
                    //{
                    //    newPos = PathFinding.SearchForBuidling(tilePosition, typeof(Enemy), out targetFound, false);
                    //}


                    position = new float3(newPos, position.z);
                }
                else if (type is TYPE_OF_CHARACTER.Soldier)
                {
                    //newPos = PathFinding.SearchForTarget(tilePosition, typeof(Enemy), typeof(Soldier), out targetFound, true);
                    newPos = PathFinding.SearchForTargetAStar(tilePosition, typeof(Enemy), typeof(Soldier), out targetFound, true);

                    // If the soldier has reached half of the field, then stop if there are no enemies nearby
                    if (!targetFound && position.x > levelLimits.y / 2)
                    {
                        isIdle = true;
                    }
                    else
                    {
                        position = new float3(newPos, position.z);
                    }
                }
            }
        }
    }
}
