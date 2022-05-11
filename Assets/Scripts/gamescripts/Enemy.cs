using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public Enemy(Graphics inGfx, GameObject inGo, GridManager inGm)
    {
        gfx = inGfx;
        gm = inGm;

        go = new GameObject { name = "enemy" + EnemyCounter.counter };
        go.transform.parent = inGo.transform;
        go.layer = LayerMask.NameToLayer("Enemies");

        cm = go.AddComponent<CollisionManager>();

        sm = go.AddComponent<SpriteManager>();
        sm.Init(go, "Sprites/StickFigureMonster", "GameObjects");
        sm.FlipX();

        go.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

        float randomY = Random.Range(gfx.GetWorldLimits().z + sm.GetCurrentSize(go.transform.localScale).y / 2, gfx.GetWorldLimits().w - sm.GetCurrentSize(go.transform.localScale).y / 2);
        go.transform.position = new Vector3(gfx.GetWorldLimits().y, randomY, 0);

        bc = go.AddComponent<BoxCollider2D>();
        bc.size = new Vector2(bc.size.x / 2, bc.size.y);

        rb = go.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.useFullKinematicContacts = true;

        health = go.AddComponent<Health>();
        health.Init(go, "Sprites/EnemyHealth", 100);

        speed = 1.2f;
        damage = 10;
        direction = -1;

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

                Vector2 newPos = pf.GetNextTile(currTile, typeof(Soldier), typeof(Enemy), out targetFound, false);

                if (!targetFound)
                {
                    newPos = pf.GetNextTile(currTile, typeof(Player), typeof(Enemy), out targetFound, false);
                }

                go.transform.position = new Vector3(Mathf.MoveTowards(go.transform.position.x, newPos.x, speed * Time.deltaTime),
                        Mathf.MoveTowards(go.transform.position.y, newPos.y, speed * Time.deltaTime), 0);
            }
            else if (sm.IsAttacking())
            {
                if (sm.Attack())
                {
                    ContactFilter2D filter = new ContactFilter2D();
                    filter.layerMask = LayerMask.GetMask("Soldiers") | LayerMask.GetMask("Player");
                    filter.useLayerMask = true;
                    List<Collider2D> results = new List<Collider2D>();
                    bc.OverlapCollider(filter, results);

                    foreach (Collider2D col in results)
                    {
                        if (col.gameObject.GetComponent<Health>() != null)
                        {
                            col.gameObject.GetComponent<Health>().Damage(damage);

                            // Turn towards the target
                            if (go.transform.position.x < col.transform.position.x && direction == -1)
                            {
                                direction = 1;
                                sm.FlipX();
                            }
                            else if (go.transform.position.x > col.transform.position.x && direction == 1)
                            {
                                direction = -1;
                                sm.FlipX();
                            }
                        }
                        else if (col.gameObject.GetComponent<PlayerHealth>() != null)
                        {
                            col.gameObject.GetComponent<PlayerHealth>().Damage(damage);

                            // Turn towards the target
                            if (go.transform.position.x < col.transform.position.x && direction == -1)
                            {
                                direction = 1;
                                sm.FlipX();
                            }
                            else if (go.transform.position.x > col.transform.position.x && direction == 1)
                            {
                                direction = -1;
                                sm.FlipX();
                            }
                        }
                    }

                    // Are there any soldiers left around the character?
                    if (results.Count == 0)
                    {
                        // Else, resume walking
                        sm.StartWalking();

                        // Turn if necessary
                        if (direction == 1)
                        {
                            direction = -1;
                            sm.FlipX();
                        }
                    }
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
