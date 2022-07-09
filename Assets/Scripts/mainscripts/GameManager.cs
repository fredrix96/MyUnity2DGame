using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class GameManager
{
    public static GameObject GameManagerObject;
    static GameObject eventSystemObject;

    public static void Init()
    {
        GameManagerObject = new GameObject { name = "GameManager" };
        GameManagerObject.transform.position = Vector3.zero;
        GameManagerObject.AddComponent<PhysicsUpdater>();

        eventSystemObject = new GameObject { name = "EventSystem" };
        eventSystemObject.transform.SetParent(GameManagerObject.transform);
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<StandaloneInputModule>();
    }
}
