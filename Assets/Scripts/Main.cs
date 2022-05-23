using UnityEngine;

public class Main : MonoBehaviour 
{
    Graphics gfx;
    CameraManager camMan;
    AudioManager am;
    Player player;
    CharacterManager charMan;
    Controller ctrl;
    GridManager gridMan;
    CoinManager coinMan;
    ShopManager shopMan;
    BuildingManager buildMan;

    void Start() 
    {
        Tools.DebugMode = false;

        GameManager.Init();

        gfx = new Graphics();
        camMan = new CameraManager();
        am = new AudioManager();

        if (!am.PlayBackgroundMusic("Game Background Music", 0.5f, true))
        {
            Debug.LogWarning("Warning: Could not play background music!");
        }

        float width = 200;
        Vector2 res = new Vector2(width, (int)(width / camMan.GetCamera().orthographicSize));
        gridMan = new GridManager(gfx, res);

        player = new Player(gridMan, camMan);
        int moneyToStartWith = 500000;
        coinMan = new CoinManager(camMan, am, moneyToStartWith);
        charMan = new CharacterManager(gfx, gridMan, camMan, player, coinMan);
        
        buildMan = GameManager.GameManagerObject.AddComponent<BuildingManager>();
        buildMan.Init(camMan, am, coinMan);

        shopMan = new ShopManager(camMan, coinMan, buildMan, gridMan);
        ctrl = new Controller(gfx, camMan, player, shopMan, gridMan);

        // Start at the left side of the world
        camMan.SetPosX(gfx.GetLevelLimits().x + camMan.GetWorldSpaceWidth() / 2);
    }

    void Update()
    {
        camMan.Update();
        ctrl.Update();
        charMan.Update();
        coinMan.Update();
        shopMan.Update();

        if (Tools.DebugMode)
        {
            Tools.LogFPS();
        }
    }
}