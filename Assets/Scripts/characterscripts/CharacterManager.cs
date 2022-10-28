using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Jobs.LowLevel.Unsafe;

public static class CharacterInformation
{
    public enum TYPE_OF_ENEMY
    {
        Eye, Mushroom, Goblin, Skeleton
    }
    public enum TYPE_OF_SOLDIER
    {
        Spearman, Maceman
    }

    static readonly Vector2[] soldierSpawnLocations = new Vector2[]
    {
        Vector2.zero, Vector2.zero
    };

    static readonly int[] enemyDamage = new int[]
    {
        20, 20, 60, 100
    };

    static readonly int[] enemyHealth = new int[]
    {
        50, 50, 100, 200
    };

    static readonly int[] enemyValue = new int[]
    {
        10, 10, 30, 50
    };

    static readonly float[] enemySpeed = new float[]
    {
        2.5f, 1.2f, 1.4f, 1.5f
    };

    static readonly int[] soldierDamage = new int[]
    {
        20, 40
    };

    static readonly int[] soldierHealth = new int[]
    {
        100, 200
    };

    static public void SetSpawnLocation(TYPE_OF_SOLDIER type, Vector2 spawn)
    {
        soldierSpawnLocations[(int)type] = spawn;
    }

    static public Vector2 GetSoldierSpawnLocation(TYPE_OF_SOLDIER type)
    {
        return soldierSpawnLocations[(int)type];
    }


    public static int GetEnemyDamage(TYPE_OF_ENEMY type)
    {
        return enemyDamage[(int)type];
    }

    public static int GetEnemyHealth(TYPE_OF_ENEMY type)
    {
        return enemyHealth[(int)type];
    }

    public static int GetEnemyValue(TYPE_OF_ENEMY type)
    {
        return enemyValue[(int)type];
    }

    public static float GetEnemySpeed(TYPE_OF_ENEMY type)
    {
        return enemySpeed[(int)type];
    }

    public static int GetSoldierDamage(TYPE_OF_SOLDIER type)
    {
        return soldierDamage[(int)type];
    }

    public static int GetSoldierHealth(TYPE_OF_SOLDIER type)
    {
        return soldierHealth[(int)type];
    }


    public static AnimationStartingPoints GetEnemyAnimationStartingPoints(TYPE_OF_ENEMY type)
    {
        AnimationStartingPoints asp;

        if (type == TYPE_OF_ENEMY.Mushroom || type == TYPE_OF_ENEMY.Goblin)
        {
            asp.idle = 20;
            asp.idleEnd = 23;
            asp.walk = 24;
            asp.walkEnd = 31;
            asp.attack = 0;
            asp.attackEnd = 7;
            asp.die = 17;
            asp.dieEnd = 20;
            asp.takeDamage = 32;
            asp.takeDamageEnd = 35;
        }
        else if (type == TYPE_OF_ENEMY.Eye)
        {
            asp.idle = 20;
            asp.idleEnd = 27;
            asp.walk = 20;
            asp.walkEnd = 27;
            asp.attack = 0;
            asp.attackEnd = 7;
            asp.die = 16;
            asp.dieEnd = 19;
            asp.takeDamage = 28;
            asp.takeDamageEnd = 31;
        }
        else if (type == TYPE_OF_ENEMY.Skeleton)
        {
            asp.idle = 20;
            asp.idleEnd = 23;
            asp.walk = 32;
            asp.walkEnd = 35;
            asp.attack = 0;
            asp.attackEnd = 7;
            asp.die = 16;
            asp.dieEnd = 19;
            asp.takeDamage = 28;
            asp.takeDamageEnd = 31;
        }
        else
        {
            asp.idle = 0;
            asp.idleEnd = 0;
            asp.walk = 0;
            asp.walkEnd = 0;
            asp.attack = 0;
            asp.attackEnd = 0;
            asp.die = 0;
            asp.dieEnd = 0;
            asp.takeDamage = 0;
            asp.takeDamageEnd = 0;

            Debug.LogWarning("No animations found for " + type.ToString());
        }

        return asp;
    }

    public static AnimationStartingPoints GetSoldierAnimationStartingPoints(TYPE_OF_SOLDIER type)
    {
        AnimationStartingPoints asp;

        if (type == TYPE_OF_SOLDIER.Spearman)
        {
            asp.idle = 24;
            asp.idleEnd = 31;
            asp.walk = 34;
            asp.walkEnd = 41;
            asp.attack = 8;
            asp.attackEnd = 11;
            asp.die = 16;
            asp.dieEnd = 21;
            asp.takeDamage = 42;
            asp.takeDamageEnd = 45;
        }
        else if (type == TYPE_OF_SOLDIER.Maceman)
        {
            asp.idle = 24;
            asp.idleEnd = 31;
            asp.walk = 34;
            asp.walkEnd = 41;
            asp.attack = 12;
            asp.attackEnd = 15;
            asp.die = 16;
            asp.dieEnd = 21;
            asp.takeDamage = 42;
            asp.takeDamageEnd = 45;
        }
        else
        {
            asp.idle = 0;
            asp.idleEnd = 0;
            asp.walk = 0;
            asp.walkEnd = 0;
            asp.attack = 0;
            asp.attackEnd = 0;
            asp.die = 0;
            asp.dieEnd = 0;
            asp.takeDamage = 0;
            asp.takeDamageEnd = 0;

            Debug.LogWarning("No animations found for " + type.ToString());
        }

        return asp;
    }

}

public static class HumansCounter
{
    public static int counter = 0;
    public static int nrOfHumans = 0;
    public static int max = 0; // depends on number of houses

    public static void Reset()
    {
        counter = 0;
        nrOfHumans = 0;
    }
}

public static class EnemyCounter
{
    public static int counter = 0;
    public static int nrOfEnemies = 0;
    public static int max = 5000;

    public static void Reset()
    {
        counter = 0;
        nrOfEnemies = 0;
    }
}

public static class SoldierCounter_Spearmen
{
    public static int counter = 0;
    public static int nrOfSoldiers = 0;
    public static int nrToSpawn = 0;
    public static int max = 5000;

    public static void Reset()
    {
        counter = 0;
        nrOfSoldiers = 0;
        nrToSpawn = 0;
    }
}

public static class SoldierCounter_Macemen
{
    public static int counter = 0;
    public static int nrOfSoldiers = 0;
    public static int nrToSpawn = 0;
    public static int max = 5000;

    public static void Reset()
    {
        counter = 0;
        nrOfSoldiers = 0;
        nrToSpawn = 0;
    }
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

                if (enemySpawnDelay > 3) enemySpawnDelay -= 0.1f;
                if (enemySpawnDelay > 2) enemySpawnDelay -= 0.05f;
                if (enemySpawnDelay > 1) enemySpawnDelay -= 0.01f;
                if (enemySpawnDelay > 0.5f) enemySpawnDelay -= 0.005f;
                if (enemySpawnDelay > 0.25f) enemySpawnDelay -= 0.001f;

                if (randomStart < 450)
                {
                    randomStart += 1;
                }
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
        int randomNumber = Random.Range(0, 1000);

        CharacterInformation.TYPE_OF_ENEMY eType;
        if (randomNumber > 980)
        {
            eType = CharacterInformation.TYPE_OF_ENEMY.Skeleton;
        }
        else if (randomNumber > 850)
        {
            eType = CharacterInformation.TYPE_OF_ENEMY.Goblin;
        }
        else if (randomNumber > 650)
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
        text = (SoldierCounter_Spearmen.nrOfSoldiers + SoldierCounter_Macemen.nrOfSoldiers).ToString();
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
        soldiersDataText.text = (SoldierCounter_Spearmen.nrOfSoldiers + SoldierCounter_Macemen.nrOfSoldiers).ToString();
        enemiesDataText.text = EnemyCounter.nrOfEnemies.ToString();

        int killed = EnemyCounter.counter - EnemyCounter.nrOfEnemies;
        killsDataText.text = killed.ToString();
    }
}
