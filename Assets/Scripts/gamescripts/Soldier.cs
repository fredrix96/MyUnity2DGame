using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Character
{
    public Soldier(Graphics inGfx, GameObject inGo, GridManager inGm)
    {
        gfx = inGfx;
        gm = inGm;

        go = new GameObject { name = "soldier" + SoldierCounter.counter };
        go.transform.SetParent(inGo.transform);
        go.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        go.layer = LayerMask.NameToLayer("Soldiers");

        cm = go.AddComponent<CollisionManager>();

        sm = go.AddComponent<SpriteManager>();
        sm.Init(go, gm, "Sprites/StickFigure", "Character");

        float randomY = Random.Range(0, gm.GetRes().y - 1);
        Vector2 spawnTile = new Vector2(0, randomY);
        go.transform.position = gm.GetTile(spawnTile).GetPos();

        // This is to make sure that feet of the character wont walk on another sprite
        pivotHeightDiff = Mathf.Abs(go.transform.position.y - sm.GetColliderPivotPoint(go).y);
        go.transform.position = sm.GetColliderPivotPoint(go);

        health = go.AddComponent<Health>();
        health.Init(go,"Sprites/SoldierHealth", 100);

        speed = 2.0f;
        damage = 20;
        direction = 1;

        currTile = gm.GetTile(spawnTile);
        currTile.IncreaseCharacters(this);
        pf = new PathFinding(gm);
    }

    public override void Update()
    {
        if (!isDead)
        {
            if (sm.IsWalking())
            {
                sm.Walk();

                MarkTile();

                Vector2 newPos = pf.GetNextTile(currTile, typeof(Enemy), typeof(Soldier), out targetFound, true);

                // If the soldier has reached half of the field, then stop if there are no enemies nearby
                if (!targetFound && go.transform.position.x > gfx.GetLevelLimits().y / 2)
                {
                    sm.StartIdle();
                }
                else
                {
                    // Notice how we adjust based on the pivot difference
                    go.transform.position = new Vector3(Mathf.MoveTowards(go.transform.position.x, newPos.x, speed * Time.deltaTime),
                        Mathf.MoveTowards(go.transform.position.y, newPos.y + pivotHeightDiff, speed * Time.deltaTime), 0);
                }
            }
            else if (sm.IsAttacking())
            {
                if (sm.Attack())
                {
                    List<Collider2D> results = sm.GetListOfOverlapColliders(LayerMask.GetMask("Enemies"));

                    foreach (Collider2D col in results)
                    {
                        if (col.gameObject.GetComponent<Health>() != null)
                        {
                            col.gameObject.GetComponent<Health>().Damage(damage);

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
                }
            }
            else if (sm.IsIdle())
            {
                // Always search for target
                pf.GetNextTile(currTile, typeof(Enemy), typeof(Soldier), out targetFound, true);

                if (targetFound)
                {
                    sm.StartWalking();
                }
                else
                {
                    sm.Idle();
                }
            }

            if (health.GetHealth() <= 0)
            {
                isDead = true;
                currTile.DecreaseCharacters(this);
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

    void MarkTile()
    {
        Vector3 correctPivotToTilePos = new Vector3(go.transform.position.x, go.transform.position.y - pivotHeightDiff, go.transform.position.z);
        Tile newTile = gm.GetTileFromWorldPosition(correctPivotToTilePos);

        if (newTile != currTile)
        {
            newTile.IncreaseCharacters(this);
            currTile.DecreaseCharacters(this);
            currTile = newTile;
        }
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
