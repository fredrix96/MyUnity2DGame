using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    CoinManager coinMan;
    int value;
    float groanDelay;
    double time;
    
    public Enemy(GameObject inGo, CoinManager inCoinMan, int inValue = 1)
    {
        type = TYPE_OF_CHARACTER.Enemy;

        coinMan = inCoinMan;
        value = inValue;

        groanDelay = 5.0f;
        time = 0;

        go = new GameObject { name = "enemy" + EnemyCounter.counter };
        go.transform.parent = inGo.transform;
        go.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        go.layer = LayerMask.NameToLayer("Enemies");

        cm = go.AddComponent<CollisionManager>();

        AnimationStartingPoints asp;
        asp.idle = 20;
        asp.idleEnd = 23;
        asp.walk = 24;
        asp.walkEnd = 31;
        asp.attack = 0;
        asp.attackEnd = 8;
        asp.die = 17;
        asp.dieEnd = 19;

        boundingBoxOffset = new Vector2(0.0f, -0.5f);

        sm = go.AddComponent<SpriteManager>();
        sm.Init(go, "Sprites/Monsters Creatures Fantasy/Sprites/Mushroom", "Character", asp, boundingBoxOffset);
        sm.FlipX();

        float randomY = Random.Range(0, GridManager.GetRes().y - 1);
        Vector2 spawnTile = new Vector2(GridManager.GetRes().x - 1, randomY);
        go.transform.position = GridManager.GetTile(spawnTile).GetWorldPos();

        // This is to make sure that feet of the character wont walk on another sprite
        pivotHeightDiff = Mathf.Abs(go.transform.position.y - sm.GetColliderPivotPoint(go).y);
        go.transform.position = sm.GetColliderPivotPoint(go);
        ph.position = new Vector2(go.transform.position.x, go.transform.position.y);
        lastXPos = ph.position.x;

        health = go.AddComponent<Health>();
        health.Init(go, "Sprites/EnemyHealth", 100, new Vector2(0.2f, 0.15f));

        speed = 1.2f;
        damage = 50;
        direction = -1;

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
            time += Time.deltaTime;

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
                // Damage every time the attack animation has completed
                if (sm.Attack())
                {
                    Damage();
                }
            }

            // Groan
            if (time > groanDelay)
            {
                Groan();
                time = 0;
            }

            if (health.GetHealth() <= 0)
            {
                isDead = true;
                currTile.DecreaseCharacters(this);
                GridManager.GetCharacterTiles(type).Remove(currTile);
                coinMan.CreateCoin(go.transform.position, new Vector2(0.01f, 0.01f), Vector3.up, 1, 1.5f, true);
                coinMan.AddCoins(value);
                AudioManager.PlayAudio3D("Enemy Death", 0.2f, go.transform.position);
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
        GameObject attackBox = new GameObject { name = "AttackBox" };

        float range = GridManager.GetTileWidth();
        if (sm.IsFlipped())
        {
            range *= -1;
        }

        attackBox.transform.position = new Vector2(sm.GetBoxCollider2D().transform.position.x + range, sm.GetBoxCollider2D().transform.position.y + (GridManager.GetTileHeight() * boundingBoxOffset.y));
        attackBox.transform.localScale = new Vector2(GridManager.GetTileWidth() * 1.5f, GridManager.GetTileHeight() * 1.5f);

        BoxCollider2D attackBc = attackBox.AddComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(sm.GetBoxCollider2D(), attackBc);

        List<Collider2D> results = sm.GetListOfOverlapColliders(
                        LayerMask.GetMask("Soldiers") | LayerMask.GetMask("Player") | LayerMask.GetMask("Buildings"),
                        attackBc);

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

                // Only hit one character per attack
                //break;
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

                // Only hit one character per attack
                //break;
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

        Object.Destroy(attackBox);
    }

    void Groan()
    {
        // 10% chance to groan
        int index = Random.Range(0, 49);

        switch (index)
        {
            case 0:
                AudioManager.PlayAudio3D("Groan", 0.1f, go.transform.position);
                break;
            case 1:
                AudioManager.PlayAudio3D("Groan2", 0.1f, go.transform.position);
                break;
            case 2:
                AudioManager.PlayAudio3D("Groan3", 0.1f, go.transform.position);
                break;
            case 3:
                AudioManager.PlayAudio3D("Groan4", 0.1f, go.transform.position);
                break;
            case 4:
                AudioManager.PlayAudio3D("Groan5", 0.1f, go.transform.position);
                break;
            default:
                // Be quiet
                break;
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
