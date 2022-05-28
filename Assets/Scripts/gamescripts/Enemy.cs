using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    CoinManager coinMan;
    int value;
    
    public Enemy(Graphics inGfx, GameObject inGo, GridManager inGm, CoinManager inCoinMan, int inValue = 1)
    {
        type = TYPE_OF_CHARACTER.Enemy;

        gfx = inGfx;
        gm = inGm;
        coinMan = inCoinMan;
        value = inValue;

        go = new GameObject { name = "enemy" + EnemyCounter.counter };
        go.transform.parent = inGo.transform;
        go.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        go.layer = LayerMask.NameToLayer("Enemies");

        cm = go.AddComponent<CollisionManager>();

        sm = go.AddComponent<SpriteManager>();
        sm.Init(go, gm, "Sprites/StickFigureMonster", "Character");
        sm.FlipX();

        float randomY = Random.Range(0, gm.GetRes().y - 1);
        Vector2 spawnTile = new Vector2(gm.GetRes().x - 1, randomY);
        go.transform.position = gm.GetTile(spawnTile).GetWorldPos();

        // This is to make sure that feet of the character wont walk on another sprite
        pivotHeightDiff = Mathf.Abs(go.transform.position.y - sm.GetColliderPivotPoint(go).y);
        go.transform.position = sm.GetColliderPivotPoint(go);
        ph.position = go.transform.position;

        health = go.AddComponent<Health>();
        health.Init(go, "Sprites/EnemyHealth", 100);

        speed = 1.2f;
        damage = 10;
        direction = -1;

        currTile = gm.GetTile(spawnTile);
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

                MarkTile();

                SetGameObjectPosition(position);

                UpdatePositionHandler();
            }
            else if (sm.IsAttacking())
            {
                if (sm.Attack())
                {
                    List<Collider2D> results = sm.GetListOfOverlapColliders(LayerMask.GetMask("Soldiers") | LayerMask.GetMask("Player"));

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
                currTile.DecreaseCharacters(this);
                coinMan.CreateCoin(go.transform.position, new Vector2(0.2f, 0.2f), Vector3.up, 1, 1.5f, true);
                coinMan.AddCoins(value);
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
