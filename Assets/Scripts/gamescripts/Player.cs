using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    GameObject go;
    SpriteManager sm;
    PlayerHealth health;
    Tile currTile;
    CollisionManager cm;

    bool isDead;
    bool shouldBeRemoved;
    int playerSpeed;
    int damage;
    int maxHealth;
    double regenerationTimer;
    float regenerationDelay;
    Vector2 dirVector;

    public Player()
    {
        maxHealth = 300;
        regenerationDelay = 0.3f;
        playerSpeed = 5;
        damage = 20;

        Respawn();
    }

    public void Update()
    {
        if (!isDead)
        {
            RegenerateHealth();

            // Is the player attacking?
            if (sm.IsAttacking())
            {
                if (sm.Attack())
                {
                    List<Collider2D> results = sm.GetListOfOverlapColliders(LayerMask.GetMask("Enemies"));

                    foreach (Collider2D col in results)
                    {
                        if (col.gameObject.GetComponent<Health>() != null)
                        {
                            col.gameObject.GetComponent<Health>().Damage(damage);
                        }
                    }
                }
            }
            // Is the player walking?
            else if (sm.IsWalking())
            {
                Walk();
            }
            else if (sm.IsIdle())
            {
                sm.Idle();
            }

            if (health.GetHealth() <= 0)
            {
                isDead = true;
                currTile.PlayerOnTile(false);
                GridManager.SetPlayerTile(null);
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

    public void SetDirX(int x = 0)
    {
        if (!isDead) sm.StartWalking();
        dirVector.x = x;
    }

    public void SetDirY(int y = 0)
    {
        if (!isDead) sm.StartWalking();
        dirVector.y = y;
    }

    void ResetDir()
    {
        dirVector = Vector2.zero;
    }

    public void Attack()
    {
        if (!isDead)
        {
            sm.StartAttacking();
        }
    }

    public void Idle()
    {
        if (!isDead)
        {
            sm.StartIdle();
        }
    }

    void Walk()
    {
        // Change direction if needed
        if (dirVector.x < 0 && !sm.IsFlipped() || dirVector.x > 0 && sm.IsFlipped())
        {
            sm.FlipX();
        }

        // To avoid faster speeds at the diagonal
        dirVector = dirVector.normalized;

        go.transform.position = new Vector2(go.transform.position.x + playerSpeed * Time.deltaTime * dirVector.x, go.transform.position.y + playerSpeed * Time.deltaTime * dirVector.y);

        ResetDir();

        sm.Walk();

        MarkTile();
    }

    void MarkTile()
    {
        Tile newTile = GridManager.GetTileFromWorldPosition(go.transform.position);

        if (newTile != currTile && newTile != null)
        {
            GridManager.SetPlayerTile(newTile);
            newTile.PlayerOnTile(true);
            currTile.PlayerOnTile(false);
            currTile = newTile;
        }
    }

    void RegenerateHealth()
    {
        if (health.GetHealth() < maxHealth && health.GetHealth() > 0)
        {
            regenerationTimer += Time.deltaTime;

            if (regenerationTimer > regenerationDelay)
            {
                health.IncreaseHealth(1);
                regenerationTimer = 0;
            }
        }
    }

    public bool Remove()
    {
        return shouldBeRemoved;
    }

    public void Destroy()
    {
        Object.Destroy(go);
        health.Destroy();
    }

    public void Respawn()
    {
        Vector2 spawnTile = new Vector2(0, 3);

        go = new GameObject { name = "player" };
        go.transform.parent = GameManager.GameManagerObject.transform;
        go.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
        go.transform.position = GridManager.GetTile(spawnTile).GetWorldPos();
        go.layer = LayerMask.NameToLayer("Player");

        cm = go.AddComponent<CollisionManager>();

        sm = go.AddComponent<SpriteManager>();
        sm.Init(go, "Sprites/StickFigureKing", "Player", false);

        health = go.AddComponent<PlayerHealth>();
        health.Init(go, maxHealth);

        dirVector = Vector2.zero;
        isDead = false;
        shouldBeRemoved = false;

        currTile = GridManager.GetTile(spawnTile);
        currTile.PlayerOnTile(true);
    }

    public Vector3 GetPosition()
    {
        return go.transform.position;
    }
}
