using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    PlayerHealth playerHealth;

    int playerSpeed;
    int maxHealth;
    double regenerationTimer;
    float regenerationDelay;
    Vector2 dirVector;
    bool playerHasSpawned; // checks if the player character (the king) has spawned for the first time

    public Player()
    {
        maxHealth = 300;
        regenerationDelay = 0.3f;
        playerSpeed = 5;
        damage = 20;
        playerHasSpawned = false;
        isDead = true;

        Respawn(GridManager.GetTile(new Vector2(5, 5)));
    }

    public override void Update()
    {
        if (!isDead)
        {
            RegenerateHealth();

            // Is the player attacking?
            if (sm.IsAttacking())
            {
                if (sm.Attack())
                {
                    PlaySwordSound();
                    Damage();
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

            if (playerHealth.GetHealth() <= 0)
            {
                isDead = true;
                currTile.PlayerOnTile(false);
                GridManager.SetPlayerTile(null);
                AudioManager.PlayAudio3D("Player Death", 0.2f, go.transform.position);
            }
        }
        else if (playerHasSpawned)
        {
            if (sm.Die() > 3 && go != null)
            {
                shouldBeRemoved = true;

                // End the game if the player dies without any castle to respawn in
                //if (BuildingInformation.GetCounter(BuildingInformation.TYPE_OF_BUILDING.Castle) < 1)
                //{
                    GameManager.GameOver();
                //}
            }
        }
    }

    void Damage()
    {
        GameObject swordSwingBox = new GameObject { name = "SwordSwingBox" };

        float range = GridManager.GetTileWidth();
        if (sm.IsFlipped())
        {
            range *= -1;
        }

        swordSwingBox.transform.position = new Vector2(sm.GetBoxCollider2D().transform.position.x + range, sm.GetBoxCollider2D().transform.position.y + (GridManager.GetTileHeight() * boundingBoxOffset.y));
        swordSwingBox.transform.localScale = new Vector2(GridManager.GetTileWidth() * 3, GridManager.GetTileHeight() * 3);

        BoxCollider2D swordBc = swordSwingBox.AddComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(sm.GetBoxCollider2D(), swordBc);

        List<Collider2D> results = sm.GetListOfOverlapColliders(LayerMask.GetMask("Enemies"), swordBc);

        foreach (Collider2D col in results)
        {
            if (col.gameObject.GetComponent<Health>() != null)
            {
                col.gameObject.GetComponent<Health>().Damage(damage);
            }
        }

        Object.Destroy(swordSwingBox);
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
        // "-(GridManager.GetTileHeight() * 2.5f))" is necessary to get the correct tile placement because of the sprite size
        Tile newTile = GridManager.GetTileFromWorldPosition(new Vector2(go.transform.position.x, go.transform.position.y - (GridManager.GetTileHeight() * 2.5f)));

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
        if (playerHealth.GetHealth() < maxHealth && playerHealth.GetHealth() > 0)
        {
            regenerationTimer += Time.deltaTime;

            if (regenerationTimer > regenerationDelay)
            {
                playerHealth.IncreaseHealth(1);
                regenerationTimer = 0;
            }
        }
    }

    public bool Remove()
    {
        return shouldBeRemoved;
    }

    public void HasBeenRemoved()
    {
        shouldBeRemoved = false;
    }

    public override void Destroy()
    {
        Object.Destroy(go);
        playerHealth.Destroy();
    }

    public void Respawn(Tile spawnTile)
    {
        go = new GameObject { name = "player" };
        go.transform.parent = GameManager.GameManagerObject.transform;
        go.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        go.transform.position = spawnTile.GetWorldPos();
        go.layer = LayerMask.NameToLayer("Player");

        go.AddComponent<CollisionManager>();

        AnimationStartingPoints asp;
        asp.idle = 20;
        asp.idleEnd = 27;
        asp.walk = 30;
        asp.walkEnd = 37;
        asp.attack = 0;
        asp.attackEnd = 3;
        asp.die = 13;
        asp.dieEnd = 17;

        boundingBoxOffset = new Vector2(0.0f, -1.5f);

        sm = go.AddComponent<SpriteManager>();
        sm.Init(go, "Sprites/Medieval King Pack 2/Sprites", "Player", asp, boundingBoxOffset, true, false);

        playerHealth = go.AddComponent<PlayerHealth>();
        playerHealth.Init(maxHealth);

        dirVector = Vector2.zero;
        isDead = false;
        shouldBeRemoved = false;

        currTile = spawnTile;
        currTile.PlayerOnTile(true);

        playerHasSpawned = true;
    }

    public GameObject GetPlayerObject()
    {
        return go;
    }
}
