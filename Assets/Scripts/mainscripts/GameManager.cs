using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManager
{
    public static GameObject GameManagerObject;

    public static void Init()
    {
        GameManagerObject = new GameObject { name = "GameManager" };
        GameManagerObject.transform.position = Vector3.zero;
        GameManagerObject.AddComponent<PhysicsUpdater>();
    }
}
