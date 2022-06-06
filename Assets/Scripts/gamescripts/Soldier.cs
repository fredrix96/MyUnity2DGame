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
        go.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        go.layer = LayerMask.NameToLayer("Soldiers");

        cm = go.AddComponent<CollisionManager>();

        sm = go.AddComponent<SpriteManager>();
        sm.Init(go, "Sprites/StickFigure", "Character");

        float randomY = Random.Range(0, GridManager.GetRes().y - 1);
        Vector2 spawnTile = new Vector2(0, randomY);
        go.transform.position = GridManager.GetTile(spawnTile).GetWorldPos();

        // This is to make sure that feet of the character wont walk on another sprite
        pivotHeightDiff = Mathf.Abs(go.transform.position.y - sm.GetColliderPivotPoint(go).y);
        go.transform.position = sm.GetColliderPivotPoint(go);
        ph.position = go.transform.position;

        health = go.AddComponent<Health>();
        health.Init(go,"Sprites/SoldierHealth", 100);

        speed = 2.0f;
        damage = 20;
        direction = 1;

        currTile = GridManager.GetTile(spawnTile);
        currTile.IncreaseCharacters(this);

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
                UpdatePositionHandler();

                if (ph.targetFound)
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

    public bool Remove()
    {
        return shouldBeRemoved;
    }

    public override void Destroy()
    {
        Object.Destroy(go);
    }
}
