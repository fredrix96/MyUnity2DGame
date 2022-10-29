using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs.LowLevel.Unsafe;

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
    Text humansDataText, soldiersDataText, enemiesDataText, killsDataText;

    GameObject characterObjects, enemyObjects, soldierObjects;

    float enemySpawnDelay, soldierSpawnDelay;
    double enemySpawnTimer, soldierSpawnTimer;
    int randomStart; // The higher value this is, the more likely it is that stronger enemies will spawn

    public CharacterManager(Player inPlayer, CoinManager inCoinMan)
    {
        player = inPlayer;
        coinMan = inCoinMan;

        characterObjects = new GameObject { name = "characters" };
        characterObjects.transform.SetParent(GameManager.GameManagerObject.transform);
        message = characterObjects.AddComponent<PopUpMessage>();
        message.Init(characterObjects);

        enemyObjects = new GameObject { name = "enemies" };
        enemyObjects.transform.SetParent(characterObjects.transform);
        soldierObjects = new GameObject { name = "soldiers" };
        soldierObjects.transform.SetParent(characterObjects.transform);

        enemies = new List<Enemy>();
        soldiers = new List<Soldier>();

        enemySpawnDelay = 4.0f; // decreases with time
        soldierSpawnDelay = 0.6f;

        enemySpawnTimer = 0;
        soldierSpawnTimer = 0;

        randomStart = 0;

        DisplayCharacterData();
    }

    public void Update()
    {
        UpdateEnemies();
        UpdateSoldiers();
        UpdatePlayer();
        UpdateCharacterTextData();

#if DEBUG
        if (Input.GetKey(KeyCode.C))
        {
            SpawnEnemy();
        }
        if (Input.GetKey(KeyCode.X))
        {
            SpawnSoldier(CharacterInformation.TYPE_OF_SOLDIER.Spearman);
            SpawnSoldier(CharacterInformation.TYPE_OF_SOLDIER.Maceman);
            SpawnSoldier(CharacterInformation.TYPE_OF_SOLDIER.HeavySwordman);
        }
# endif
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
            int framesToWait = 10; // = n:th frame
            List<T> listToUpdate = GetListToUpdate(characters, framesToWait);

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


                int nrOfBatches = listToUpdate.Count / JobsUtility.JobWorkerCount;
                if (nrOfBatches < 1)
                {
                    nrOfBatches = 1;
                }

                // Child threads ----------------------------------------

                var jobHandle = job.Schedule(listToUpdate.Count, nrOfBatches);
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

        UpdateCharacterPosition(enemies, GameManager.toggles.GetMultithreadingOn());

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
        if (SoldierCounter_Spearmen.nrToSpawn > 0)
        {
            if (SoldierCounter_Spearmen.nrOfSoldiers < SoldierCounter_Spearmen.max)
            {
                soldierSpawnTimer += Time.deltaTime;
                if (soldierSpawnTimer > soldierSpawnDelay)
                {
                    SpawnSoldier(CharacterInformation.TYPE_OF_SOLDIER.Spearman);
                    soldierSpawnTimer = 0;
                }
            }
        }

        if (SoldierCounter_Macemen.nrToSpawn > 0)
        {
            if (SoldierCounter_Macemen.nrOfSoldiers < SoldierCounter_Macemen.max)
            {
                soldierSpawnTimer += Time.deltaTime;
                if (soldierSpawnTimer > soldierSpawnDelay)
                {
                    SpawnSoldier(CharacterInformation.TYPE_OF_SOLDIER.Maceman);
                    soldierSpawnTimer = 0;
                }
            }
        }

        if (SoldierCounter_HeavySwordmen.nrToSpawn > 0)
        {
            if (SoldierCounter_HeavySwordmen.nrOfSoldiers < SoldierCounter_HeavySwordmen.max)
            {
                soldierSpawnTimer += Time.deltaTime;
                if (soldierSpawnTimer > soldierSpawnDelay)
                {
                    SpawnSoldier(CharacterInformation.TYPE_OF_SOLDIER.HeavySwordman);
                    soldierSpawnTimer = 0;
                }
            }
        }

        UpdateCharacterPosition(soldiers, GameManager.toggles.GetMultithreadingOn());

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
        if (player.Remove() && !GameManager.IsGameOver())
        {
            player.HasBeenRemoved();
            player.Destroy();
            message.SendPopUpMessage("The King is dead!", 2.5f);
        }
    }

    void SpawnEnemy()
    {
        int randomNumber = Random.Range(0, 10000);

        CharacterInformation.TYPE_OF_ENEMY eType;
        if (randomNumber > 9800 - randomStart)
        {
            eType = CharacterInformation.TYPE_OF_ENEMY.Skeleton;
        }
        else if (randomNumber > 8500 - randomStart)
        {
            eType = CharacterInformation.TYPE_OF_ENEMY.Goblin;
        }
        else if (randomNumber > 6500 - randomStart)
        {
            eType = CharacterInformation.TYPE_OF_ENEMY.Eye;
        }
        else
        {
            eType = CharacterInformation.TYPE_OF_ENEMY.Mushroom;
        }

        enemies.Add(new Enemy(enemyObjects, eType, coinMan));
        EnemyCounter.counter++;
        EnemyCounter.nrOfEnemies++;

        if (enemySpawnDelay > 3) enemySpawnDelay -= 0.1f;
        if (enemySpawnDelay > 2) enemySpawnDelay -= 0.05f;
        if (enemySpawnDelay > 1) enemySpawnDelay -= 0.01f;
        if (enemySpawnDelay > 0.5f) enemySpawnDelay -= 0.005f;
        if (enemySpawnDelay > 0.25f) enemySpawnDelay -= 0.001f;

        if (randomStart < 5000)
        {
            randomStart += 1;
        }
    }

    void RemoveEnemy(Enemy enemy)
    {
        enemy.Destroy();
        enemies.Remove(enemy);
        EnemyCounter.nrOfEnemies--;
    }

    void SpawnSoldier(CharacterInformation.TYPE_OF_SOLDIER type)
    {
        soldiers.Add(new Soldier(soldierObjects, type));

        if (type == CharacterInformation.TYPE_OF_SOLDIER.Spearman)
        {
            SoldierCounter_Spearmen.counter++;
            SoldierCounter_Spearmen.nrOfSoldiers++;
            SoldierCounter_Spearmen.nrToSpawn--;
        }
        else if (type == CharacterInformation.TYPE_OF_SOLDIER.Maceman)
        {
            SoldierCounter_Macemen.counter++;
            SoldierCounter_Macemen.nrOfSoldiers++;
            SoldierCounter_Macemen.nrToSpawn--;
        }
        else if (type == CharacterInformation.TYPE_OF_SOLDIER.HeavySwordman)
        {
            SoldierCounter_HeavySwordmen.counter++;
            SoldierCounter_HeavySwordmen.nrOfSoldiers++;
            SoldierCounter_HeavySwordmen.nrToSpawn--;
        }
    }

    void RemoveSoldier(Soldier soldier)
    {
        soldier.Destroy();
        soldiers.Remove(soldier);

        if (soldier.GetSoldierType() == CharacterInformation.TYPE_OF_SOLDIER.Spearman)
        {
            SoldierCounter_Spearmen.nrOfSoldiers--;
        }
        else if (soldier.GetSoldierType() == CharacterInformation.TYPE_OF_SOLDIER.Maceman)
        {
            SoldierCounter_Macemen.nrOfSoldiers--;
        }
        else if (soldier.GetSoldierType() == CharacterInformation.TYPE_OF_SOLDIER.HeavySwordman)
        {
            SoldierCounter_HeavySwordmen.nrOfSoldiers--;
        }
    }

    void DisplayCharacterData()
    {
        string text;

        // Bar
        UIManager.CreateImage(null, "barImage", Resources.Load<Sprite>("Sprites/WoodenBackground"), new Vector2(0, 500), new Vector2(500f, 40f));

        // Humans
        UIManager.CreateImage(null, "humansDataImage", Resources.Load<Sprite>("Sprites/HouseIcon"), new Vector2(-200, 500), new Vector2(30f, 30f));
        text = HumansCounter.nrOfHumans.ToString() + " / " + HumansCounter.max.ToString();
        humansDataText = UIManager.CreateText(null, "humansDataText", text, 22, new Vector2(-75, 500), new Vector2(100f, 100f));

        // Soldiers
        UIManager.CreateImage(null, "soldiersDataImage", Resources.Load<Sprite>("Sprites/SwordIcon"), new Vector2(-80, 500), new Vector2(30f, 30f));
        text = (SoldierCounter_Spearmen.nrOfSoldiers + SoldierCounter_Macemen.nrOfSoldiers + SoldierCounter_HeavySwordmen.nrOfSoldiers).ToString();
        soldiersDataText = UIManager.CreateText(null, "soldiersDataText", text, 22, new Vector2(40, 500), new Vector2(100f, 100f));

        // Enemies
        UIManager.CreateImage(null, "enemiesDataImage", Resources.Load<Sprite>("Sprites/MonsterIcon"), new Vector2(80, 500), new Vector2(30f, 30f));
        text = EnemyCounter.nrOfEnemies.ToString();
        enemiesDataText = UIManager.CreateText(null, "enemiesDataText", text, 22, new Vector2(200, 500), new Vector2(100f, 100f));

        // Kills
        UIManager.CreateImage(null, "killsDataImage", Resources.Load<Sprite>("Sprites/SkullIcon"), new Vector2(180, 500), new Vector2(30f, 30f));
        int killed = EnemyCounter.counter - EnemyCounter.nrOfEnemies;
        text = killed.ToString();
        killsDataText = UIManager.CreateText(null, "killsDataText", text, 22, new Vector2(300, 500), new Vector2(100f, 100f));
    }

    void UpdateCharacterTextData()
    {
        humansDataText.text = HumansCounter.nrOfHumans.ToString() + " / " + HumansCounter.max;
        soldiersDataText.text = (SoldierCounter_Spearmen.nrOfSoldiers + SoldierCounter_Macemen.nrOfSoldiers + SoldierCounter_HeavySwordmen.nrOfSoldiers).ToString();
        enemiesDataText.text = EnemyCounter.nrOfEnemies.ToString();

        int killed = EnemyCounter.counter - EnemyCounter.nrOfEnemies;
        killsDataText.text = killed.ToString();
    }
}
