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
    CoinManager coinMan;
    PopUpMessage message;

    GameObject chracterObjects, enemyObjects, soldierObjects;

    float enemySpawnDelay, soldierSpawnDelay, playerSpawnDelay;
    double enemySpawnTimer, soldierSpawnTimer, playerSpawnTimer;

    public CharacterManager(Graphics inGfx, GridManager inGm, CameraManager inCam, Player inPlayer, CoinManager inCoinMan)
    {
        gfx = inGfx;
        gm = inGm;
        player = inPlayer;
        coinMan = inCoinMan;

        chracterObjects = new GameObject { name = "characters" };
        chracterObjects.transform.SetParent(GameManager.GameManagerObject.transform);
        message = chracterObjects.AddComponent<PopUpMessage>();
        message.Init(chracterObjects, inCam);

        enemyObjects = new GameObject { name = "enemies" };
        enemyObjects.transform.SetParent(chracterObjects.transform);
        soldierObjects = new GameObject { name = "soldiers" };
        soldierObjects.transform.SetParent(chracterObjects.transform);

        enemies = new List<Enemy>();
        soldiers = new List<Soldier>();

        enemySpawnDelay = 1.0f;
        soldierSpawnDelay = 3.0f;
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
            message.SendPopUpMessage("The King is dead!", 2.5f, Color.red);
        }
        else if (playerSpawnTimer > 0)
        {
            playerSpawnTimer += Time.deltaTime;

            // Respawn the player object after a certain amount of time
            if (playerSpawnTimer > playerSpawnDelay)
            {
                player.Respawn();
                playerSpawnTimer = 0;
                message.SendPopUpMessage("A new King has arrived!" + System.Environment.NewLine + "All hail the new King!", 2.5f, Color.white);
            }
        }
    }

    void SpawnEnemy()
    {
        enemies.Add(new Enemy(gfx, enemyObjects, gm, coinMan));
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
