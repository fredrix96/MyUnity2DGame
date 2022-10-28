using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public static class GameManager
{
    public static GameObject GameManagerObject;
    public static Toggles toggles;

    static GameObject eventSystemObject;
    static PopUpMessage message;
    static bool gameOver;
    static Text gameTimer;
    static float startTime;

    public static void Init()
    {
        GameManagerObject = new GameObject { name = "GameManager" };
        GameManagerObject.transform.position = Vector3.zero;
        GameManagerObject.AddComponent<PhysicsUpdater>();
        toggles = GameManagerObject.AddComponent<Toggles>();

        message = GameManagerObject.AddComponent<PopUpMessage>();
        message.Init(GameManagerObject);

        eventSystemObject = new GameObject { name = "EventSystem" };
        eventSystemObject.transform.SetParent(GameManagerObject.transform);
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<StandaloneInputModule>();

        startTime = 0;

        gameOver = false;
    }

    public static void GameStart(float time)
    {
        startTime = time;
        gameTimer = UIManager.CreateText(null, "gameTimer", "Survive for: " + startTime.ToString("0.00") + " seconds", 17, new Vector2(0, 465), new Vector2(110, 110), TextAnchor.MiddleRight);
    }

    public static bool UpdateTimer(float currentTime)
    {
        startTime -= currentTime;

        if (startTime < 0)
        {
            startTime = 0;
            gameTimer.text = "Survive for: " + startTime.ToString("0.00") + " seconds";
            return true;
        }
        else
        {
            gameTimer.text = "Survive for: " + startTime.ToString("0.00") + " seconds";
            return false;
        }
    }

    public static void GameOver(bool win = false)
    {
        if (win)
        {
            message.SendPopUpMessage("Congratulations! You made it!" + System.Environment.NewLine + "Press ESC", -1);
        }
        else
        {
            message.SendPopUpMessage("GAME OVER" + System.Environment.NewLine + "Press ESC", -1);
        }
        gameOver = true;
    }

    public static bool IsGameOver()
    {
        return gameOver;
    }

    public static void Quit()
    {
        Application.Quit();
        gameOver = false;
    }
}
