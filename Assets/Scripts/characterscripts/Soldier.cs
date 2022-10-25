using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Character
{
    public Soldier(GameObject inGo)
    {
        type = TYPE_OF_CHARACTER.Soldier;

        go = new GameObject { name = "soldier" + SoldierCounter.counter };
        go.transform.SetParent(inGo.transform);
        go.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        go.layer = LayerMask.NameToLayer("Soldiers");

        cm = go.AddComponent<CollisionManager>();

        AnimationStartingPoints asp;
        asp.idle = 24;
        asp.idleEnd = 31;
        asp.walk = 34;
        asp.walkEnd = 41;
        asp.attack = 8;
        asp.attackEnd = 11;
        asp.die = 16;
        asp.dieEnd = 21;
        asp.takeDamage = 42;
        asp.takeDamageEnd = 45;

        boundingBoxOffset = new Vector2(0.0f, -0.25f);

        sm = go.AddComponent<SpriteManager>();
        sm.Init(go, "Sprites/Medieval Warrior Pack 2/Sprites", asp, boundingBoxOffset, 0.2f);

        float randomY = Random.Range(0, GridManager.GetRes().y - 1);
        spawnTile = new Vector2(0, randomY);
        go.transform.position = GridManager.GetTile(spawnTile).GetWorldPos();

        // This is to make sure that feet of the character wont walk on another sprite
        pivotHeightDiff = Mathf.Abs(go.transform.position.y - sm.GetColliderPivotPoint(go).y);
        go.transform.position = sm.GetColliderPivotPoint(go);
        ph.position = new Vector2(go.transform.position.x, go.transform.position.y);
        lastXPos = ph.position.x;

        health = go.AddComponent<Health>();
        health.Init(go,"Sprites/SoldierHealth", 100, new Vector2(0.2f, 0.15f));

        speed = 2.0f;
        damage = 10;
        direction = 1;

        currTile = GridManager.GetTile(spawnTile);
        currTile.IncreaseCharacters(this);
        GridManager.GetCharacterTiles(type).Add(currTile);

        ph.type = type;
        ph.isIdle = false;

        UpdatePositionHandler();
    }

    public override void Update()
    {
        if (!isDead)
        {
            if (sm.IsWalking())
            {
                sm.Walk();

                UpdatePositionHandler();

                WalkToNewPosition();

                MarkTile(type);

                CheckDirection(type);

            }
            else if (sm.IsAttacking())
            {
                if (sm.Attack())
                {
                    PlaySwordSound();
                    Damage();
                }
            }
            else if (sm.IsIdle())
            {
                UpdatePositionHandler();

                sm.Idle();
            }
            else if (sm.IsTakingDamage())
            {
                if (sm.TakeDamage())
                {
                    // SOUND
                }
            }

            if (health.GetHealth() <= 0)
            {
                isDead = true;
                currTile.DecreaseCharacters(this);
                GridManager.GetCharacterTiles(type).Remove(currTile);
            }
        }
        else
        {
            if (sm.Die() > 3)
            {
                shouldBeRemoved = true;
            }
        }
    }

    void Damage()
    {
        GameObject spearBox = new GameObject { name = "SpearBox" };

        float range = GridManager.GetTileWidth();
        if (sm.IsFlipped())
        {
            range *= -1;
        }

        spearBox.transform.position = new Vector2(sm.GetBoxCollider2D().transform.position.x + range, sm.GetBoxCollider2D().transform.position.y + (GridManager.GetTileHeight() * boundingBoxOffset.y));
        spearBox.transform.localScale = new Vector2(GridManager.GetTileWidth() * 4.0f, GridManager.GetTileHeight() * 1.0f);

        BoxCollider2D spearBc = spearBox.AddComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(sm.GetBoxCollider2D(), spearBc);

        List<Collider2D> results = sm.GetListOfOverlapColliders(LayerMask.GetMask("Enemies"), spearBc);

        foreach (Collider2D col in results)
        {
            if (col.gameObject.GetComponent<Health>() != null)
            {
                col.gameObject.GetComponent<Health>().Damage(damage, false, false);

                // Turn towards the target
                if (go.transform.position.x > col.transform.position.x && direction == 1)
                {
                    direction = -1;
                    sm.FlipX();
                }
                else if (go.transform.position.x < col.transform.position.x && direction == -1)
                {
                    direction = 1;
                    sm.FlipX();
                }

                // Only hit one character per attack
                //break;
            }
        }

        // Are there any enemies left around the character?
        if (results.Count == 0)
        {
            // Else, resume walking
            sm.StartWalking();

            // Turn if necessary
            if (direction == -1)
            {
                direction = 1;
                sm.FlipX();
            }
        }

        Object.Destroy(spearBox);
    }

    public bool Remove()
    {
        return shouldBeRemoved;
    }

    public override void Destroy()
    {
        Object.Destroy(go);
    }
}
