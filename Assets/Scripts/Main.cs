using UnityEngine;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour 
{
    Player player;
    CharacterManager charMan;
    Controller ctrl;
    CoinManager coinMan;
    ShopManager shopMan;
    BuildingManager buildMan;

    void Start() 
    {
        CameraManager.Init();

        // Make sure that Unity get all the available cores except for one (to not overload the CPU due to some issues in the Job system)
        JobsUtility.JobWorkerCount = SystemInfo.processorCount - 1;
        Debug.Log("Available cores: " + JobsUtility.JobWorkerCount);

        Tools.DebugMode = false;

        GameManager.Init();
        Graphics.Init();
        AudioManager.Init();
        UIManager.Init();

        float width = 200;
        Vector2 res = new Vector2(width, (int)(width / CameraManager.GetCamera().orthographicSize));
        GridManager.Init(res);

        EventManager.Init();
        EventManager.AddListenerToLoadMenu(LoadMenu);
        EventManager.AddListenerToUnloadMenu(UnloadMenu);
        EventManager.AddListenerToLoadLevel(LoadLevel);
        EventManager.AddListenerToUnloadLevel(UnloadLevel);

        EventManager.InvokeLoadMenu();
    }

    void Update()
    {
        AudioManager.Update();

        if (EventManager.levelLoaded)
        {
            ctrl.Update();

            if (!GameManager.IsGameOver())
            {
                if (GameManager.UpdateTimer(Time.deltaTime))
                {
                    GameManager.GameOver(true);
                }

                CameraManager.Update(player);
                ExperienceManager.Update();

                charMan.Update();
                coinMan.Update();
                shopMan.Update();
            }
        }
    }

    void LoadMenu()
    {
        if (!AudioManager.PlayBackgroundMusic("Menu Music 1", 0.2f, true))
        {
            Debug.LogWarning("Warning: Could not play menu background music!");
        }

        MainMenu.Init(LoadLevel, QuitGame);

        EventManager.menuLoaded = true;
    }

    void UnloadMenu()
    {
        AudioManager.StopAllBackgroundMusic();

        MainMenu.Remove();

        EventManager.menuLoaded = false;
    }

    void LoadLevel()
    {
        GameManager.GameStart(1000);

        EventManager.InvokeUnloadMenu();

        if (!AudioManager.PlayBackgroundMusic("Game Background Music", 0.5f, true))
        {
            Debug.LogWarning("Warning: Could not play background music!");
        }

        player = new Player();

        ExperienceManager.Init(player);

        int moneyToStartWith = 5000;
        coinMan = new CoinManager(moneyToStartWith);

        charMan = new CharacterManager(player, coinMan);

        buildMan = GameManager.GameManagerObject.AddComponent<BuildingManager>();
        buildMan.Init(coinMan, player);

        shopMan = new ShopManager(coinMan, buildMan);
        ctrl = new Controller(player, shopMan);

        // Start at the player
        CameraManager.ActivateOnPlayer(true);

        EventManager.levelLoaded = true;
    }

    void UnloadLevel()
    {
        AudioManager.StopAllBackgroundMusic();

        HumansCounter.Reset();
        EnemyCounter.Reset();
        SoldierCounter_Spearmen.Reset();
        SoldierCounter_Macemen.Reset();
        BuildingInformation.Reset();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void QuitGame()
    {
        GameManager.Quit();
    }
}