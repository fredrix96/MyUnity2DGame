using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    GameObject go;
    SpriteManager sm;
    PlayerHealth health;
    BoxCollider2D bc;
    Rigidbody2D rb;
    GridManager gm;
    Tile currTile;
    CollisionManager cm;
    CameraManager cam;

    bool isDead;
    bool shouldBeRemoved;
    int playerSpeed;
    int damage;
    int maxHealth;
    double regenerationTimer;
    float regenerationDelay;
    Vector2 dirVector;

    public Player(GridManager inGm, CameraManager inCam)
    {
        gm = inGm;
        cam = inCam;

        maxHealth = 300;
        regenerationDelay = 0.3f;
        playerSpeed = 14;
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
                    ContactFilter2D filter = new ContactFilter2D();
                    filter.SetLayerMask(LayerMask.GetMask("Enemies"));
                    List<Collider2D> results = new List<Collider2D>();
                    bc.OverlapCollider(filter, results);

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
                SetDirX();
                SetDirY();
            }
            else if (sm.IsIdle())
            {
                sm.Idle();
            }

            if (health.GetHealth() <= 0)
            {
                isDead = true;
                bc.isTrigger = true;
                rb.useFullKinematicContacts = false;
                currTile.PlayerOnTile(false);
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
        if (!isDead)
        {
            sm.StartWalking();
            dirVector = new Vector2(x, dirVector.y);
        }
    }

    public void SetDirY(int y = 0)
    {
        if (!isDead)
        {
            sm.StartWalking();
            dirVector = new Vector2(dirVector.x, y);
        }
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

        go.transform.position = new Vector2(go.transform.position.x + playerSpeed * Time.deltaTime * dirVector.x, go.transform.position.y + playerSpeed * Time.deltaTime * dirVector.y);

        sm.Walk();

        MarkTile();
    }

    void MarkTile()
    {
        Tile newTile = gm.GetTileFromWorldPosition(go.transform.position);

        if (newTile != currTile)
        {
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
        go = new GameObject { name = "player" };
        go.transform.parent = GameManager.GameManagerObject.transform;
        go.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
        go.transform.position = new Vector3(-30f, 0, 0);
        go.layer = LayerMask.NameToLayer("Player");

        cm = go.AddComponent<CollisionManager>();

        sm = go.AddComponent<SpriteManager>();
        sm.Init(go, "Sprites/StickFigureKing", "Player");

        bc = go.AddComponent<BoxCollider2D>();
        bc.size = new Vector2(bc.size.x / 2, bc.size.y);

        rb = go.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.useFullKinematicContacts = true;

        health = go.AddComponent<PlayerHealth>();
        health.Init(go, maxHealth, cam);

        dirVector = Vector2.zero;
        isDead = false;
        shouldBeRemoved = false;

        currTile = gm.GetTileFromWorldPosition(go.transform.position);
        currTile.PlayerOnTile(true);
    }

    public Vector3 GetPosition()
    {
        return go.transform.position;
    }

    public Vector2 GetSize()
    {
        return bc.bounds.size;
    }
}
