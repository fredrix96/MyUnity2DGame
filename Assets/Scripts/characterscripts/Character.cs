using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics; // more optimized math, good for multithreading

public class Character
{
    public enum TYPE_OF_CHARACTER
    {
        Enemy, Soldier, Player
    }
    public TYPE_OF_CHARACTER type;
    public PositionHandler ph;

    protected GameObject go;
    protected SpriteManager sm;
    protected CollisionManager cm;
    protected Tile currTile;
    protected Health health;

    protected float lastXPos;
    protected float pivotHeightDiff;
    protected int direction;
    protected int damage;
    protected float speed;
    protected bool isDead;
    protected bool shouldBeRemoved;

    public Character()
    {
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

    public void UpdatePositionHandler()
    {
        // In
        ph.tilePosition = currTile.GetTilePosition();
        ph.speed = speed;
        ph.isDead = IsDead();
        ph.isAttacking = sm.IsAttacking();

        // Out
        if (ph.isIdle)
        {
            sm.StartIdle();
        }
    }

    protected void WalkToNewPosition()
    {
        // Notice how we adjust the height based on the pivot difference
        go.transform.position = new float3(Mathf.MoveTowards(go.transform.position.x, ph.position.x, speed * Time.deltaTime),
            Mathf.MoveTowards(go.transform.position.y, ph.position.y + pivotHeightDiff, speed * Time.deltaTime), 0);
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

    public struct PositionHandler
    {
        public TYPE_OF_CHARACTER type;
        public float3 position;
        public float2 tilePosition;
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

                if (type == TYPE_OF_CHARACTER.Enemy)
                {
                    newPos = PathFinding.SearchForTarget(tilePosition, TYPE_OF_CHARACTER.Soldier, TYPE_OF_CHARACTER.Enemy, out targetFound, false);

                    position = new float3(newPos, position.z);
                }
                else if (type == TYPE_OF_CHARACTER.Soldier)
                {
                    newPos = PathFinding.SearchForTarget(tilePosition, TYPE_OF_CHARACTER.Enemy, TYPE_OF_CHARACTER.Soldier, out targetFound, true);

                    // If the soldier has reached half of the field, then stop if there are no enemies nearby
                    if (!targetFound && position.x > Graphics.GetLevelLimits().y / 2)
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

    protected void PlaySwordSound()
    {
        int index = UnityEngine.Random.Range(0, 4);

        switch (index)
        {
            case 0:
                AudioManager.PlayAudio3D("Sword Swing", 0.1f, go.transform.position);
                break;
            case 1:
                AudioManager.PlayAudio3D("Sword Swing2", 0.1f, go.transform.position);
                break;
            case 2:
                AudioManager.PlayAudio3D("Sword Swing3", 0.1f, go.transform.position);
                break;
            case 3:
                AudioManager.PlayAudio3D("Sword Swing4", 0.1f, go.transform.position);
                break;
            default:
                AudioManager.PlayAudio3D("Sword Swing", 0.1f, go.transform.position);
                break;
        }
    }
}
