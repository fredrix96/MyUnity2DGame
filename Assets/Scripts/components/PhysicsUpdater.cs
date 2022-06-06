using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsUpdater : MonoBehaviour
{
    static PhysicsUpdater instance = null;
    public static System.Action OnFixedUpdate;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this);
        }
        else if (this != instance) Destroy(this);
    }

    void FixedUpdate()
    {
        if (OnFixedUpdate != null) OnFixedUpdate();
    }

    // To add a function: "PhysicsUpdater.OnFixedUpdate += Function;"
}
