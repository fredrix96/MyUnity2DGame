using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public static class EventManager
{
    static UnityEvent loadLevel, unloadLevel;
    static UnityEvent loadMenu, unloadMenu;

    static public bool levelLoaded, menuLoaded;

    static public void Init()
    {
        loadLevel = new UnityEvent();
        unloadLevel = new UnityEvent();

        loadMenu = new UnityEvent();
        unloadMenu = new UnityEvent();

        levelLoaded = false;
        menuLoaded = false;
    }

    static public void Update()
    {
        //if (Input.anyKeyDown && loadLevel != null)
        //{
        //    loadLevel.Invoke();
        //}
    }

    static public void AddListenerToLoadLevel(UnityAction func)
    {
        loadLevel.AddListener(func);
    }

    static public void AddListenerToUnloadLevel(UnityAction func)
    {
        unloadLevel.AddListener(func);
    }

    static public void InvokeLoadLevel()
    {
        loadLevel.Invoke();
    }

    static public void InvokeUnloadLevel()
    {
        unloadLevel.Invoke();
    }

    static public void AddListenerToLoadMenu(UnityAction func)
    {
        loadMenu.AddListener(func);
    }

    static public void AddListenerToUnloadMenu(UnityAction func)
    {
        unloadMenu.AddListener(func);
    }

    static public void InvokeLoadMenu()
    {
        loadMenu.Invoke();
    }

    static public void InvokeUnloadMenu()
    {
        unloadMenu.Invoke();
    }
}
