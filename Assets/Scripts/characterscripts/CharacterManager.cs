using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

public static class HumansCounter
{
    public static int counter = 0;
    public static int nrOfHumans = 0;
    public static int max = 0; // depends on number of houses
}

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
    public static int nrToSpawn = 0;
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
    Text humansDataText, soldiersDataText, enemiesDataText, killsDataText;

    GameObject characterObjects, enemyObjects, soldierObjects;

    float enemySpawnDelay, soldierSpawnDelay, playerSpawnDelay;
    double enemySpawnTimer, soldierSpawnTimer, playerSpawnTimer;

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

        enemySpawnDelay = 5.0f; // decreases with time
        soldierSpawnDelay = 0.6f;
        playerSpawnDelay = 5.0f;

        enemySpawnTimer = 0;
        soldierSpawnTimer = 0;
        playerSpawnTimer = 0;

        DisplayCharacterData();
    }

    public void Update()
    {
        UpdateEnemies();
        UpdateSoldiers();
        UpdatePlayer();
        UpdateCharacterTextData();
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
            List<T> listToUpdate = GetListToUpdate(characters, 60);

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

                if (enemySpawnDelay > 4) enemySpawnDelay -= 0.05f;
                if (enemySpawnDelay > 3) enemySpawnDelay -= 0.01f;
                if (enemySpawnDelay > 2) enemySpawnDelay -= 0.005f;
                if (enemySpawnDelay > 1) enemySpawnDelay -= 0.001f;
                if (enemySpawnDelay > 0.5f) enemySpawnDelay -= 0.0005f;
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
        if (SoldierCounter.nrToSpawn > 0)
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

            // Respawn the player character after a certain amount of time
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
        enemies.Add(new Enemy(enemyObjects, coinMan, 15));
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
        SoldierCounter.nrToSpawn--;
    }

    void RemoveSoldier(Soldier soldier)
    {
        soldier.Destroy();
        soldiers.Remove(soldier);
        SoldierCounter.nrOfSoldiers--;
    }

    void DisplayCharacterData()
    {
        GameObject go = new GameObject() { name = "characterData" };
        go.transform.SetParent(characterObjects.transform);

        // Canvas
        GameObject canvasObject = new GameObject { name = "canvas" };
        canvasObject.transform.parent = go.transform;

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = CameraManager.GetCamera();
        canvas.sortingLayerName = "UI";

        CanvasScaler cs = canvasObject.AddComponent<CanvasScaler>();
        cs.referenceResolution = new Vector2(1920, 1080);

        // Bar
        GameObject barObject = new GameObject { name = "barImage" };
        barObject.transform.SetParent(canvasObject.transform);
        barObject.transform.localScale = new Vector2(0.3f, 0.3f);

        Image barImage = barObject.AddComponent<Image>();
        barImage.color = new Color(0, 0, 0, 0.9f);

        // Reset anchor
        barImage.rectTransform.anchorMin = Vector2.zero;
        barImage.rectTransform.anchorMax = Vector2.zero;

        barImage.rectTransform.anchoredPosition = new Vector3(canvas.pixelRect.width / 2, canvas.pixelRect.height - 30, 0);
        barImage.rectTransform.sizeDelta = new Vector2(1800, 120);

        // Humans

        // Image object
        GameObject humansDataImageObject = new GameObject { name = "humansDataImage" };
        humansDataImageObject.transform.SetParent(canvasObject.transform);
        humansDataImageObject.transform.localScale = new Vector2(0.3f, 0.3f);

        Image humansDataImage = humansDataImageObject.AddComponent<Image>();
        humansDataImage.sprite = Resources.Load<Sprite>("Sprites/HouseIcon");

        // Reset anchor
        humansDataImage.rectTransform.anchorMin = Vector2.zero;
        humansDataImage.rectTransform.anchorMax = Vector2.zero;

        // Anchor the image
        humansDataImage.rectTransform.anchoredPosition = new Vector3((canvas.pixelRect.width / 2) - 200, canvas.pixelRect.height - 30, 0);

        // Text object
        GameObject humansDataTextObject = new GameObject { name = "humansDataText" };
        humansDataTextObject.transform.SetParent(canvasObject.transform);
        humansDataTextObject.transform.localScale = new Vector2(0.4f, 0.4f);

        humansDataText = humansDataTextObject.AddComponent<Text>();
        humansDataText.text = HumansCounter.nrOfHumans.ToString() + " / " + HumansCounter.max;
        humansDataText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        humansDataText.fontSize = 40;
        humansDataText.alignment = TextAnchor.MiddleLeft;

        // Reset anchor
        humansDataText.rectTransform.anchorMin = Vector2.zero;
        humansDataText.rectTransform.anchorMax = Vector2.zero;
        humansDataText.rectTransform.sizeDelta = new Vector2(184, 100);

        humansDataText.rectTransform.anchoredPosition = new Vector3((canvas.pixelRect.width / 2) - 140, canvas.pixelRect.height - 30, 0);

        // Soldiers

        // Image object
        GameObject soldiersDataImageObject = new GameObject { name = "soldiersDataImage" };
        soldiersDataImageObject.transform.SetParent(canvasObject.transform);
        soldiersDataImageObject.transform.localScale = new Vector2(0.3f, 0.3f);

        Image soldiersDataImage = soldiersDataImageObject.AddComponent<Image>();
        soldiersDataImage.sprite = Resources.Load<Sprite>("Sprites/SwordIcon");

        // Reset anchor
        soldiersDataImage.rectTransform.anchorMin = Vector2.zero;
        soldiersDataImage.rectTransform.anchorMax = Vector2.zero;

        // Anchor the image
        soldiersDataImage.rectTransform.anchoredPosition = new Vector3((canvas.pixelRect.width / 2) - 80, canvas.pixelRect.height - 30, 0);

        // Text object
        GameObject soldiersDataTextObject = new GameObject { name = "soldiersDataText" };
        soldiersDataTextObject.transform.SetParent(canvasObject.transform);
        soldiersDataTextObject.transform.localScale = new Vector2(0.4f, 0.4f);

        soldiersDataText = soldiersDataTextObject.AddComponent<Text>();
        soldiersDataText.text = SoldierCounter.nrOfSoldiers.ToString();
        soldiersDataText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        soldiersDataText.fontSize = 40;
        soldiersDataText.alignment = TextAnchor.MiddleLeft;

        // Reset anchor
        soldiersDataText.rectTransform.anchorMin = Vector2.zero;
        soldiersDataText.rectTransform.anchorMax = Vector2.zero;
        soldiersDataText.rectTransform.sizeDelta = new Vector2(184, 100);

        soldiersDataText.rectTransform.anchoredPosition = new Vector3((canvas.pixelRect.width / 2) - 20, canvas.pixelRect.height - 30, 0);

        // Enemies

        // Image object
        GameObject enemiesDataImageObject = new GameObject { name = "enemiesDataImage" };
        enemiesDataImageObject.transform.SetParent(canvasObject.transform);
        enemiesDataImageObject.transform.localScale = new Vector2(0.3f, 0.3f);

        Image enemiesDataImage = enemiesDataImageObject.AddComponent<Image>();
        enemiesDataImage.sprite = Resources.Load<Sprite>("Sprites/MonsterIcon");

        // Reset anchor
        enemiesDataImage.rectTransform.anchorMin = Vector2.zero;
        enemiesDataImage.rectTransform.anchorMax = Vector2.zero;

        // Anchor the image
        enemiesDataImage.rectTransform.anchoredPosition = new Vector3((canvas.pixelRect.width / 2) + 40, canvas.pixelRect.height - 30, 0);

        // Text object
        GameObject enemiesDataTextObject = new GameObject { name = "enemiesDataText" };
        enemiesDataTextObject.transform.SetParent(canvasObject.transform);
        enemiesDataTextObject.transform.localScale = new Vector2(0.4f, 0.4f);

        enemiesDataText = enemiesDataTextObject.AddComponent<Text>();
        enemiesDataText.text = EnemyCounter.nrOfEnemies.ToString();
        enemiesDataText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        enemiesDataText.fontSize = 40;
        enemiesDataText.alignment = TextAnchor.MiddleLeft;

        // Reset anchor
        enemiesDataText.rectTransform.anchorMin = Vector2.zero;
        enemiesDataText.rectTransform.anchorMax = Vector2.zero;
        enemiesDataText.rectTransform.sizeDelta = new Vector2(184, 100);

        enemiesDataText.rectTransform.anchoredPosition = new Vector3((canvas.pixelRect.width / 2) + 100, canvas.pixelRect.height - 30, 0);

        // Kills
        // Image object
        GameObject killsDataImageObject = new GameObject { name = "killsDataImage" };
        killsDataImageObject.transform.SetParent(canvasObject.transform);
        killsDataImageObject.transform.localScale = new Vector2(0.3f, 0.3f);

        Image killsDataImage = killsDataImageObject.AddComponent<Image>();
        killsDataImage.sprite = Resources.Load<Sprite>("Sprites/SkullIcon");

        // Reset anchor
        killsDataImage.rectTransform.anchorMin = Vector2.zero;
        killsDataImage.rectTransform.anchorMax = Vector2.zero;

        // Anchor the image
        killsDataImage.rectTransform.anchoredPosition = new Vector3((canvas.pixelRect.width / 2) + 160, canvas.pixelRect.height - 30, 0);

        // Text object
        GameObject killsDataTextObject = new GameObject { name = "killsDataText" };
        killsDataTextObject.transform.SetParent(canvasObject.transform);
        killsDataTextObject.transform.localScale = new Vector2(0.4f, 0.4f);

        killsDataText = killsDataTextObject.AddComponent<Text>();
        int killed = EnemyCounter.counter - EnemyCounter.nrOfEnemies;
        killsDataText.text = killed.ToString();
        killsDataText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        killsDataText.fontSize = 40;
        killsDataText.alignment = TextAnchor.MiddleLeft;

        // Reset anchor
        killsDataText.rectTransform.anchorMin = Vector2.zero;
        killsDataText.rectTransform.anchorMax = Vector2.zero;
        killsDataText.rectTransform.sizeDelta = new Vector2(184, 100);

        killsDataText.rectTransform.anchoredPosition = new Vector3((canvas.pixelRect.width / 2) + 220, canvas.pixelRect.height - 30, 0);
    }

    void UpdateCharacterTextData()
    {
        humansDataText.text = HumansCounter.nrOfHumans.ToString() + " / " + HumansCounter.max;
        soldiersDataText.text = SoldierCounter.nrOfSoldiers.ToString();
        enemiesDataText.text = EnemyCounter.nrOfEnemies.ToString();

        int killed = EnemyCounter.counter - EnemyCounter.nrOfEnemies;
        killsDataText.text = killed.ToString();

    }
}
