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
        go.layer = LayerMask.NameToLayer("Soldiers");

        cm = go.AddComponent<CollisionManager>();

        sm = go.AddComponent<SpriteManager>();
        sm.Init(go, "Sprites/StickFigure", "GameObjects");

        go.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

        float randomY = Random.Range(gfx.GetWorldLimits().z + sm.GetCurrentSize(go.transform.localScale).y / 2, gfx.GetWorldLimits().w - sm.GetCurrentSize(go.transform.localScale).y / 2);
        go.transform.position = new Vector3(gfx.GetWorldLimits().x, randomY, 0);

        bc = go.AddComponent<BoxCollider2D>();
        bc.size = new Vector2(bc.size.x / 2, bc.size.y);

        rb = go.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.useFullKinematicContacts = true;

        health = go.AddComponent<Health>();
        health.Init(go,"Sprites/SoldierHealth", 100);

        speed = 2.0f;
        damage = 38;
        direction = 1;

        currTile = gm.GetTileFromWorldPosition(go.transform.position);
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
                if (!targetFound && go.transform.position.x > gfx.GetWorldLimits().y / 2)
                {
                    sm.StartIdle();
                }
                else
                {
                    go.transform.position = new Vector3(Mathf.MoveTowards(go.transform.position.x, newPos.x, speed * Time.deltaTime),
                        Mathf.MoveTowards(go.transform.position.y, newPos.y, speed * Time.deltaTime), 0);
                }
            }
            else if (sm.IsAttacking())
            {
                if (sm.Attack())
                {
                    ContactFilter2D filter = new ContactFilter2D();
                    filter.SetLayerMask(LayerMask.GetMask("Enemies"));
                    List<Collider2D> results = new List<Collider2D>();
                    bc.OverlapCollider(filter, results);

                    foreach (Collider2D col in results)
                    {
                        if (col.gameObject.GetComponent<Health>() != null)
                        {
                            col.gameObject.GetComponent<Health>().Damage(damage);

                            if (col.gameObject.name[0] == 's')
                            {
                                Debug.Log("ATTACKING FIERND");
                            }

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
                bc.isTrigger = true;
                rb.useFullKinematicContacts = false;
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
        Tile newTile = gm.GetTileFromWorldPosition(go.transform.position);

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
