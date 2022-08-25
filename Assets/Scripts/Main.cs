using UnityEngine;

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
        Tools.DebugMode = false;

        GameManager.Init();
        Graphics.Init();
        AudioManager.Init();
        UIManager.Init();

        if (!AudioManager.PlayBackgroundMusic("Game Background Music", 0.5f, true))
        {
            Debug.LogWarning("Warning: Could not play background music!");
        }

        float width = 200;
        Vector2 res = new Vector2(width, (int)(width / CameraManager.GetCamera().orthographicSize));

        GridManager.Init(res);

        player = new Player();
        int moneyToStartWith = 1500;
        coinMan = new CoinManager(moneyToStartWith);
        charMan = new CharacterManager(player, coinMan);
        
        buildMan = GameManager.GameManagerObject.AddComponent<BuildingManager>();
        buildMan.Init(coinMan, player);

        shopMan = new ShopManager(coinMan, buildMan);
        ctrl = new Controller(player, shopMan);

        // Start at the left side of the world
        CameraManager.SetPosX(Graphics.GetLevelLimits().x + CameraManager.GetWorldSpaceWidth() / 2);
    }

    void Update()
    {
        ctrl.Update();

        if (!GameManager.IsGameOver())
        {
            CameraManager.Update();

            charMan.Update();
            coinMan.Update();
            shopMan.Update();
        }
    }
}