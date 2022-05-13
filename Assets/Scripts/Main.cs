using UnityEngine;

public class Main : MonoBehaviour 
{
    Graphics gfx;
    CameraManager camMan;
    AudioManager am;
    Player player;
    CharacterManager charMan;
    Controller ctrl;
    GridManager gm;
    CoinManager coinMan;

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

        Vector2 res = new Vector2((int)(Screen.width / 10), (int)(Screen.height / camMan.GetCamera().aspect / 10));
        gm = new GridManager(gfx, res);

        player = new Player(gm, camMan);
        ctrl = new Controller(gfx, camMan, player);
        coinMan = new CoinManager(camMan, am);
        charMan = new CharacterManager(gfx, gm, player, coinMan);

        // Start at the left side of the world
        camMan.SetPosX(gfx.GetLevelLimits().x + camMan.GetWorldSpaceWidth() / 2);
    }

    void Update()
    {
        camMan.Update();
        ctrl.Update();
        charMan.Update();
        coinMan.Update();

        if (Tools.DebugMode)
        {
            Tools.LogFPS();
        }
    }
}