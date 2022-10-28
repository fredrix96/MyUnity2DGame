﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class BuildingInformation
{
    private static int castleCounter = 0;
    private static int houseCounter = 0;
    private static int barrackSpearCounter = 0;
    private static int barrackMaceCounter = 0;
    private static int archeryTowerCounter = 0;

    private static int castleMax = 1;
    private static int houseMax = 10;
    private static int barrackSpearMax = 1;
    private static int barrackMaceMax = 1;
    private static int archeryTowerMax = 5;

    public static void Reset()
    {
        castleCounter = 0;
        houseCounter = 0;
        barrackSpearCounter = 0;
        barrackMaceCounter = 0;
        archeryTowerCounter = 0;
    }

    public enum TYPE_OF_BUILDING
    {
        Castle, House, Barrack_Spear, Barrack_Mace, ArcheryTower
    }

    static readonly int[] cost = new int[]
    {
        500, 250, 800, 800, 1000
    };

    static readonly int[] health = new int[]
    {
        5000, 500, 1500, 1500, 1000
    };

    // The scaling works better for now if the sizes are in odd numbers to make sure that there is always a tile in the center
    static readonly Vector2[] size = new Vector2[] 
    {
        new Vector2(7,11), new Vector2(5,7), new Vector2(7,9), new Vector2(7,9), new Vector2(5,13)
    };

    public static int GetBuildingHealth(TYPE_OF_BUILDING type)
    {
        return health[(int)type];
    }

    public static int GetBuildingCost(TYPE_OF_BUILDING type)
    {
        return cost[(int)type];
    }

    public static Vector2 GetBuildingSize(TYPE_OF_BUILDING type)
    {
        return size[(int)type];
    }

    public static bool MaxLimitReached(TYPE_OF_BUILDING type)
    {
        bool limitReached = false;

        if (type is TYPE_OF_BUILDING.Castle)
        {
            if (castleCounter == castleMax)
            {
                limitReached = true;
            }
        }
        else if (type is TYPE_OF_BUILDING.House)
        {
            if (houseCounter == houseMax)
            {
                limitReached = true;
            }
        }
        else if (type is TYPE_OF_BUILDING.Barrack_Spear)
        {
            if (barrackSpearCounter == barrackSpearMax)
            {
                limitReached = true;
            }
        }
        else if (type is TYPE_OF_BUILDING.Barrack_Mace)
        {
            if (barrackMaceCounter == barrackMaceMax)
            {
                limitReached = true;
            }
        }
        else if (type is TYPE_OF_BUILDING.ArcheryTower)
        {
            if (archeryTowerCounter == archeryTowerMax)
            {
                limitReached = true;
            }
        }

        return limitReached;
    }

    public static int GetCounter(TYPE_OF_BUILDING type)
    {
        int counter = -1;

        switch (type)
        {
            case TYPE_OF_BUILDING.Castle:
                counter = castleCounter;
                break;
            case TYPE_OF_BUILDING.House:
                counter = houseCounter;
                break;
            case TYPE_OF_BUILDING.Barrack_Spear:
                counter = barrackSpearCounter;
                break;
            case TYPE_OF_BUILDING.Barrack_Mace:
                counter = barrackMaceCounter;
                break;
            case TYPE_OF_BUILDING.ArcheryTower:
                counter = archeryTowerCounter;
                break;
            default:
                Debug.LogWarning("Warning! Could not return the " + type.ToString() + "Counter...");
                break;
        }

        return counter;
    }

    public static int GetMax(TYPE_OF_BUILDING type)
    {
        int max = -1;

        switch (type)
        {
            case TYPE_OF_BUILDING.Castle:
                max = castleMax;
                break;
            case TYPE_OF_BUILDING.House:
                max = houseMax;
                break;
            case TYPE_OF_BUILDING.Barrack_Spear:
                max = barrackSpearMax;
                break;
            case TYPE_OF_BUILDING.Barrack_Mace:
                max = barrackMaceMax;
                break;
            case TYPE_OF_BUILDING.ArcheryTower:
                max = archeryTowerMax;
                break;
            default:
                Debug.LogWarning("Warning! Could not return the " + type.ToString() + "Max...");
                break;
        }

        return max;
    }

    public static void IncreaseCounter(TYPE_OF_BUILDING type)
    {
        switch (type)
        {
            case TYPE_OF_BUILDING.Castle:
                castleCounter++;
                break;
            case TYPE_OF_BUILDING.House:
                houseCounter++;
                break;
            case TYPE_OF_BUILDING.Barrack_Spear:
                barrackSpearCounter++;
                break;
            case TYPE_OF_BUILDING.Barrack_Mace:
                barrackMaceCounter++;
                break;
            case TYPE_OF_BUILDING.ArcheryTower:
                archeryTowerCounter++;
                break;
            default:
                Debug.LogWarning("Warning! Could not increase the " + type.ToString() + "Counter...");
                break;
        }
    }

    public static void DecreaseCounter(TYPE_OF_BUILDING type)
    {
        switch (type)
        {
            case TYPE_OF_BUILDING.Castle:
                castleCounter--;
                break;
            case TYPE_OF_BUILDING.House:
                houseCounter--;
                break;
            case TYPE_OF_BUILDING.Barrack_Spear:
                barrackSpearCounter--;
                break;
            case TYPE_OF_BUILDING.Barrack_Mace:
                barrackMaceCounter--;
                break;
            case TYPE_OF_BUILDING.ArcheryTower:
                archeryTowerCounter--;
                break;
            default:
                Debug.LogWarning("Warning! Could not decrease the " + type.ToString() + "Counter...");
                break;
        }
    }
}

public class BuildingManager : MonoBehaviour
{
    GameObject go, canvasObject;
    Canvas canvas;
    CanvasScaler cs;
    List<Building> buildings;
    List<Sprite> sprites;
    CoinManager coinMan;
    Player player;

    void Start()
    {
    }

    public void Init(CoinManager inCoinMan, Player inPlayer)
    {
        coinMan = inCoinMan;
        player = inPlayer;

        go = new GameObject() { name = "buildings" };
        go.transform.SetParent(GameManager.GameManagerObject.transform);

        buildings = new List<Building>();
        sprites = new List<Sprite>();

        sprites.Add(Resources.Load<Sprite>("Sprites/Castle"));
        sprites.Add(Resources.Load<Sprite>("Sprites/House"));
        sprites.Add(Resources.Load<Sprite>("Sprites/Barrack_Spear"));
        sprites.Add(Resources.Load<Sprite>("Sprites/Barrack_Mace"));
        sprites.Add(Resources.Load<Sprite>("Sprites/ArcheryTower"));

        // Canvas
        canvasObject = new GameObject { name = "canvas" };
        canvasObject.transform.parent = go.transform;

        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = CameraManager.GetCamera();
        canvas.sortingLayerName = "Buildings";

        cs = canvasObject.AddComponent<CanvasScaler>();
        cs.referenceResolution = new Vector2(1920, 1080);
    }

    void Update()
    {
        if (!GameManager.IsGameOver())
        {
            for (int i = 0; i < buildings.Count; i++)
            {
                if (buildings[i].ShouldBeRemoved())
                {
                    RemoveBuilding(buildings[i]);
                }
                else
                {
                    buildings[i].Update();
                }
            }
        }
    }

    public void RemoveBuilding(Building building)
    {
        BuildingInformation.TYPE_OF_BUILDING type = building.GetBuildingType();

        AudioManager.PlayAudio3D("Destruction", 0.4f, building.GetPosition());

        building.MarkOrUnmarkTiles(type, building.GetCenterTile(), false);
        building.Destroy();

        buildings.Remove(building);

        BuildingInformation.DecreaseCounter(type);
    }

    public void CreateBuilding(BuildingInformation.TYPE_OF_BUILDING type, Tile inPos)
    {
        AudioManager.PlayAudio2D("Construct", 0.4f);

        if (type == BuildingInformation.TYPE_OF_BUILDING.Castle)
        {
            Castle castle = new Castle(go, inPos, coinMan, buildings, player);
            buildings.Add(castle);
        }
        else if (type == BuildingInformation.TYPE_OF_BUILDING.House)
        {
            House house = new House(go, inPos, coinMan, buildings);
            buildings.Add(house);
        }
        else if (type == BuildingInformation.TYPE_OF_BUILDING.Barrack_Spear)
        {
            Barrack barrack = new Barrack(go, inPos, coinMan, buildings, type);
            buildings.Add(barrack);
        }
        else if (type == BuildingInformation.TYPE_OF_BUILDING.Barrack_Mace)
        {
            Barrack barrack = new Barrack(go, inPos, coinMan, buildings, type);
            buildings.Add(barrack);
        }
        else if (type == BuildingInformation.TYPE_OF_BUILDING.ArcheryTower)
        {
            ArcheryTower archeryTower = new ArcheryTower(go, inPos, coinMan, buildings);
            buildings.Add(archeryTower);
        }
    }

    public List<Building> GetBuildings()
    {
        return buildings;
    }

    public Sprite GetSprite(BuildingInformation.TYPE_OF_BUILDING type)
    {
        return sprites[(int)type];
    }
}
