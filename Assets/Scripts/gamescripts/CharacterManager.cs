using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyCounter
{
    public static int counter = 0;
    public static int nrOfEnemies = 0;
    public static int max = 50;
}

public static class SoldierCounter
{
    public static int counter = 0;
    public static int nrOfSoldiers = 0;
    public static int max = 1000;
}

public class CharacterManager
{
    Graphics gfx;
    GridManager gm;
    List<Enemy> enemies;
    List<Soldier> soldiers;
    Player player;

    GameObject enemyObjects, soldierObjects;

    float enemySpawnDelay, soldierSpawnDelay, playerSpawnDelay;
    double enemySpawnTimer, soldierSpawnTimer, playerSpawnTimer;

    public CharacterManager(Graphics inGfx, GridManager inGm, Player inPlayer)
    {
        gfx = inGfx;
        gm = inGm;
        player = inPlayer;

        enemyObjects = new GameObject { name = "enemies" };
        enemyObjects.transform.parent = GameManager.GameManagerObject.transform;
        soldierObjects = new GameObject { name = "soldiers" };
        soldierObjects.transform.parent = GameManager.GameManagerObject.transform;

        enemies = new List<Enemy>();
        soldiers = new List<Soldier>();

        enemySpawnDelay = 1.0f;
        soldierSpawnDelay = 2.0f;
        playerSpawnDelay = 5.0f;

        enemySpawnTimer = 0;
        soldierSpawnTimer = 0;
        playerSpawnTimer = 0;
    }

    public void Update()
    {
        UpdateEnemies();
        UpdateSoldiers();
        UpdatePlayer();
    }

    void UpdateEnemies()
    {
        if (EnemyCounter.nrOfEnemies < EnemyCounter.max)
        {
            enemySpawnTimer += Time.deltaTime;
            if (enemySpawnTimer > enemySpawnDelay)
            {
                SpawnEnemy();
                enemySpawnTimer = 0;
            }
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].Remove())
            {
                RemoveEnemy(enemies[i]);
            }
            else
            {
                enemies[i].Update();
            }
        }
    }

    void UpdateSoldiers()
    {
        if (SoldierCounter.nrOfSoldiers < SoldierCounter.max)
        {
            soldierSpawnTimer += Time.deltaTime;
            if (soldierSpawnTimer > soldierSpawnDelay)
            {
                SpawnSoldier();
                soldierSpawnTimer = 0;
            }
        }

        for (int i = 0; i < soldiers.Count; i++)
        {
            if (soldiers[i].Remove())
            {
                RemoveSoldier(soldiers[i]);
            }
            else
            {
                soldiers[i].Update();
            }
        }
    }

    void UpdatePlayer()
    {
        player.Update();

        // Remove the player object if it is dead
        if (player.Remove() && playerSpawnTimer == 0)
        {
            player.Destroy();
            playerSpawnTimer += Time.deltaTime;
        }
        else if (playerSpawnTimer > 0)
        {
            playerSpawnTimer += Time.deltaTime;

            // Respawn the player object after a certain amount of time
            if (playerSpawnTimer > playerSpawnDelay)
            {
                player.Respawn();
                playerSpawnTimer = 0;
            }
        }
    }

    void SpawnEnemy()
    {
        enemies.Add(new Enemy(gfx, enemyObjects, gm));
        EnemyCounter.counter++;
        EnemyCounter.nrOfEnemies++;
    }

    void RemoveEnemy(Enemy enemy)
    {
        enemy.Destroy();
        enemies.Remove(enemy);
        EnemyCounter.nrOfEnemies--;
    }

    void SpawnSoldier()
    {
        soldiers.Add(new Soldier(gfx, soldierObjects, gm));
        SoldierCounter.counter++;
        SoldierCounter.nrOfSoldiers++;
    }

    void RemoveSoldier(Soldier soldier)
    {
        soldier.Destroy();
        soldiers.Remove(soldier);
        SoldierCounter.nrOfSoldiers--;
    }
}
