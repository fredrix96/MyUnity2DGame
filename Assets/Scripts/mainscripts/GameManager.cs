using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public static class GameManager
{
    public static GameObject GameManagerObject;
    static GameObject eventSystemObject;
    static PopUpMessage message;
    static bool gameOver;

    public static void Init()
    {
        GameManagerObject = new GameObject { name = "GameManager" };
        GameManagerObject.transform.position = Vector3.zero;
        GameManagerObject.AddComponent<PhysicsUpdater>();

        message = GameManagerObject.AddComponent<PopUpMessage>();
        message.Init(GameManagerObject);

        eventSystemObject = new GameObject { name = "EventSystem" };
        eventSystemObject.transform.SetParent(GameManagerObject.transform);
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<StandaloneInputModule>();

        gameOver = false;
    }

    public static void GameOver()
    {
        message.SendPopUpMessage("GAME OVER" + System.Environment.NewLine + "Press ESC to quit", -1, 60);
        gameOver = true;
    }

    public static bool IsGameOver()
    {
        return gameOver;
    }

    public static void Quit()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Application.Quit();
        gameOver = false;
    }
}
