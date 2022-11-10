﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{   
    struct STATS
    {
        public float critChance; // calculate in percent (0.1 = 10%)
        public int maxHealth;
        public float playerSpeed;
        public float gatheringArea;
        public int level;
        public int damage;
    }

    PlayerHealth playerHealth;
    STATS playerStats;

    double regenerationTimer;
    float regenerationDelay;
    Vector2 dirVector;

    float experience;
    float levelExp; 

    public Player()
    {
        isDead = false;

        Tile spawnTile = GridManager.GetTile(new Vector2(5, 5));
        go = new GameObject { name = "player" };
        go.transform.parent = GameManager.GameManagerObject.transform;
        go.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        go.transform.position = spawnTile.GetWorldPos();
        go.layer = LayerMask.NameToLayer("Player");

        go.AddComponent<CollisionManager>();

        playerStats.maxHealth = 500;
        playerStats.playerSpeed = 5;
        playerStats.critChance = 0.1f;
        playerStats.level = 1;
        playerStats.damage = 20;
        playerStats.gatheringArea = 5.0f;

        regenerationDelay = 0.3f;
        attackSpeed = 0.08f;
        experience = 0;
        levelExp = 100;

        AnimationStartingPoints asp;
        asp.idle = 20;
        asp.idleEnd = 27;
        asp.walk = 30;
        asp.walkEnd = 37;
        asp.attack = 0;
        asp.attackEnd = 3;
        asp.die = 13;
        asp.dieEnd = 17;
        asp.takeDamage = 38;
        asp.takeDamageEnd = 41;

        boundingBoxOffset = new Vector2(0.0f, -1.5f);

        sm = go.AddComponent<SpriteManager>();
        sm.Init(go, "Sprites/Medieval King Pack 2/Sprites", asp, boundingBoxOffset, attackSpeed, true, false);

        playerHealth = go.AddComponent<PlayerHealth>();
        playerHealth.Init(go, playerStats.maxHealth);

        dirVector = Vector2.zero;
        isDead = false;
        shouldBeRemoved = false;

        currTile = spawnTile;
        currTile.PlayerOnTile(true);
        GridManager.SetPlayerTile(currTile);
    }

    public override void Update()
    {
        if (!isDead)
        {
            RegenerateHealth();
            GatherExperienceOrbs();

            // Is the player attacking?
            if (sm.IsAttacking())
            {
                if (sm.Attack())
                {
                    PlayWeaponSound();
                    Damage();
                }
            }
            // Is the player walking?
            else if (sm.IsWalking())
            {
                Walk();
            }
            else if (sm.IsTakingDamage())
            {
                if (sm.TakeDamage())
                {
                    // SOUND
                }
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
        else if (sm.Die() > 3 && go != null)
        {
            shouldBeRemoved = true;

            // End the game if the player dies without any castle to respawn in
            //if (BuildingInformation.GetCounter(BuildingInformation.TYPE_OF_BUILDING.Castle) < 1)
            //{
                GameManager.GameOver();
            //}
        }
    }

    public float GetLevelXp()
    {
        return levelExp;
    }

    public float GetCurrentXp()
    {
        return experience;
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

        int damageDone = playerStats.damage;
        bool crit = false;

        // Calculate if the player crits
        if (Tools.CalculateChance(playerStats.critChance))
        {
            float critDamage = Random.Range(playerStats.damage * 0.1f, playerStats.damage * 1);
            damageDone += (int)critDamage;
            crit = true;
        }

        foreach (Collider2D col in results)
        {
            if (col.gameObject.GetComponent<Health>() != null)
            {
                col.gameObject.GetComponent<Health>().Damage(damageDone, crit, true);
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

        go.transform.position = new Vector2(go.transform.position.x + playerStats.playerSpeed * Time.deltaTime * dirVector.x, go.transform.position.y + playerStats.playerSpeed * Time.deltaTime * dirVector.y);

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
        if (playerHealth.GetHealth() < playerStats.maxHealth && playerHealth.GetHealth() > 0)
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

    public GameObject GetPlayerObject()
    {
        return go;
    }

    public void GatherExperienceOrbs()
    {
        GameObject gatherArea = new GameObject { name = "GatherArea" };

        gatherArea.transform.position = new Vector2(sm.GetBoxCollider2D().transform.position.x, sm.GetBoxCollider2D().transform.position.y + (GridManager.GetTileHeight() * boundingBoxOffset.y));
        gatherArea.transform.localScale = new Vector2(GridManager.GetTileWidth() * playerStats.gatheringArea, GridManager.GetTileHeight() * playerStats.gatheringArea);

        BoxCollider2D gatherBc = gatherArea.AddComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(sm.GetBoxCollider2D(), gatherBc);

        List<Collider2D> results = sm.GetListOfOverlapColliders(LayerMask.GetMask("Experience"), gatherBc);
        
        foreach (Collider2D col in results)
        {
            ExperienceOrb orb = ExperienceManager.GetOrb(col.gameObject.name);

            if (orb != null)
            {
                orb.SetCollected();
                experience += orb.GetExpPoints();
                ExperienceManager.UpdateXpBar();
            }
        }

        Object.Destroy(gatherArea);
    }

    public void LevelUp()
    {
        experience = experience - levelExp;
        levelExp += levelExp * 0.1f; // Increase with 10%
        ExperienceManager.UpdateXpBar();

        playerStats.critChance += 0.002f;
        playerStats.maxHealth += 5;
        playerStats.damage += 1;
        playerStats.level++;

        Debug.Log("LEVEL UP: " + playerStats.level.ToString());
    }

    void PlayWeaponSound()
    {
        int index = Random.Range(0, 4);

        switch (index)
        {
            case 0:
                AudioManager.PlayWeaponsAudio3D("Sword Swing", 0.1f, go.transform.position);
                break;
            case 1:
                AudioManager.PlayWeaponsAudio3D("Sword Swing2", 0.1f, go.transform.position);
                break;
            case 2:
                AudioManager.PlayWeaponsAudio3D("Sword Swing3", 0.1f, go.transform.position);
                break;
            case 3:
                AudioManager.PlayWeaponsAudio3D("Sword Swing4", 0.1f, go.transform.position);
                break;
            default:
                AudioManager.PlayWeaponsAudio3D("Sword Swing", 0.1f, go.transform.position);
                break;
        }
    }
}
