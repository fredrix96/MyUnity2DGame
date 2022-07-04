using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

public static class EnemyCounter
{
    public static int counter = 0;
    public static int nrOfEnemies = 0;
    public static int max = 1000;
}

public static class SoldierCounter
{
    public static int counter = 0;
    public static int nrOfSoldiers = 0;
    public static int max = 1000;
}

//[BurstCompile] // Burst compiler is making the code more streamlined to SIMD (more optimized)
public struct CharacterUpdatePositionJob : IJobParallelFor
{
    public NativeArray<Character.PositionHandler> characterDataArray;

    public void Execute(int index)
    {
        var data = characterDataArray[index];
        data.UpdatePosition();
        characterDataArray[index] = data;
    }
}

public class CharacterManager
{
    List<Enemy> enemies;
    List<Soldier> soldiers;
    Player player;
    CoinManager coinMan;
    PopUpMessage message;

    GameObject chracterObjects, enemyObjects, soldierObjects;

    float enemySpawnDelay, soldierSpawnDelay, playerSpawnDelay;
    double enemySpawnTimer, soldierSpawnTimer, playerSpawnTimer;

    public CharacterManager(Player inPlayer, CoinManager inCoinMan)
    {
        player = inPlayer;
        coinMan = inCoinMan;

        chracterObjects = new GameObject { name = "characters" };
        chracterObjects.transform.SetParent(GameManager.GameManagerObject.transform);
        message = chracterObjects.AddComponent<PopUpMessage>();
        message.Init(chracterObjects);

        enemyObjects = new GameObject { name = "enemies" };
        enemyObjects.transform.SetParent(chracterObjects.transform);
        soldierObjects = new GameObject { name = "soldiers" };
        soldierObjects.transform.SetParent(chracterObjects.transform);

        enemies = new List<Enemy>();
        soldiers = new List<Soldier>();

        enemySpawnDelay = 0.3f;
        soldierSpawnDelay = 0.6f;
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

    List<T> GetListToUpdate<T>(List<T> characters, float targetFrame)
    {
        float chunkSize;

        if (characters.Count < targetFrame)
        {
            chunkSize = characters.Count;
        }
        else
        {
            chunkSize = (float)characters.Count / targetFrame;
        }

        List<List<T>> chunks = Tools.PartitionList(characters, (int)Mathf.Ceil(chunkSize));
        List<T> listToUpdate = new List<T>();

        if (Time.frameCount % targetFrame == 0)
        {
            listToUpdate = chunks[0];
        }
        else
        {
            for (int i = 1; i < targetFrame; i++)
            {
                if (Time.frameCount % targetFrame == i && chunks.Count >= i + 1)
                {
                    listToUpdate = chunks[i];
                }
            }
        }

        return listToUpdate;
    }

    /// <summary> This is done with multithreading OR singlethreading. The new position is based on a search algorithm </summary>
    void UpdateCharacterPosition<T>(List<T> characters, bool multithreading) where T : Character
    {
        // Only search for a new path every n:th frame.
        // This is done because it allows us to have a "clever" AI while also having good FPS
        if (characters.Count > 0)
        {
            List<T> listToUpdate = GetListToUpdate(characters, 10);

            if (multithreading)
            {
                // Main thread -----------------------------------------

                var characterDataArray = new NativeArray<Character.PositionHandler>(listToUpdate.Count, Allocator.TempJob);

                for (int i = 0; i < listToUpdate.Count; i++)
                {
                    characterDataArray[i] = listToUpdate[i].ph;
                }

                var job = new CharacterUpdatePositionJob()
                {
                    characterDataArray = characterDataArray,
                };

                // Child threads ----------------------------------------

                var jobHandle = job.Schedule(listToUpdate.Count, 1);
                jobHandle.Complete();

                // Main thread ------------------------------------------

                for (int i = 0; i < listToUpdate.Count; i++)
                {
                    listToUpdate[i].SetPositionHandler(characterDataArray[i]);
                }

                characterDataArray.Dispose();
            }
            else
            {
                for (int i = 0; i < listToUpdate.Count; i++)
                {
                    listToUpdate[i].ph.UpdatePosition();
                }
            }
        }
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

        UpdateCharacterPosition(enemies, true);

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

        UpdateCharacterPosition(soldiers, true);

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
            message.SendPopUpMessage("The King is dead!", 2.5f);
        }
        else if (playerSpawnTimer > 0)
        {
            playerSpawnTimer += Time.deltaTime;

            // Respawn the player object after a certain amount of time
            if (playerSpawnTimer > playerSpawnDelay)
            {
                player.Respawn();
                playerSpawnTimer = 0;
                message.SendPopUpMessage("A new King has arrived!" + System.Environment.NewLine + "All hail the new King!", 2.5f);
            }
        }
    }

    void SpawnEnemy()
    {
        enemies.Add(new Enemy(enemyObjects, coinMan));
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
        soldiers.Add(new Soldier(soldierObjects));
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
