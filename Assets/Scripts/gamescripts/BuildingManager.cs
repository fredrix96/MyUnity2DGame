using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class BuildingInformation
{
    private static int castleCounter = 0;
    private static int houseCounter = 0;

    private static int castleMax = 1;
    private static int houseMax = 15;

    public enum TYPE_OF_BUILDING
    {
        Castle, House
    }

    static readonly int[] cost = new int[]
    {
        400, 50
    };

    static readonly int[] health = new int[]
    {
        10000, 1000
    };

    // The scaling works better for now if the sizes are in odd numbers to make sure that there is always a tile in the center
    static readonly Vector2[] size = new Vector2[] 
    {
        new Vector2(7,11), new Vector2(3,5),
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
    AudioManager audioMan;
    CoinManager coinMan;

    void Start()
    {
    }

    public void Init(CameraManager inCam, AudioManager inAudioMan, CoinManager inCoinMan)
    {
        audioMan = inAudioMan;
        coinMan = inCoinMan;

        go = new GameObject() { name = "buildings" };
        go.transform.SetParent(GameManager.GameManagerObject.transform);

        buildings = new List<Building>();
        sprites = new List<Sprite>();

        sprites.Add(Resources.Load<Sprite>("Sprites/Castle"));
        sprites.Add(Resources.Load<Sprite>("Sprites/House"));

        // Canvas
        canvasObject = new GameObject { name = "canvas" };
        canvasObject.transform.parent = go.transform;

        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = inCam.GetCamera();
        canvas.sortingLayerName = "Buildings";

        cs = canvasObject.AddComponent<CanvasScaler>();
        cs.referenceResolution = new Vector2(1920, 1080);
    }

    void Update()
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

    public void RemoveBuilding(Building building)
    {
        BuildingInformation.TYPE_OF_BUILDING type = building.GetBuildingType();

        audioMan.PlayAudio3D("Destruction", 0.4f, building.GetPosition());

        building.MarkOrUnmarkTiles(type, building.GetCenterTile(), false);
        building.Destroy();

        buildings.Remove(building);

        BuildingInformation.DecreaseCounter(type);
    }

    public void CreateBuilding(BuildingInformation.TYPE_OF_BUILDING type, Tile inPos, GridManager inGridMan)
    {
        audioMan.PlayAudio3D("Construct", 0.4f, inPos.GetWorldPos());

        if (type == BuildingInformation.TYPE_OF_BUILDING.Castle)
        {
            Castle castle = new Castle(go, inPos, inGridMan, coinMan);
            buildings.Add(castle);
        }
        else if (type == BuildingInformation.TYPE_OF_BUILDING.House)
        {
            House house = new House(go, inPos, inGridMan, coinMan);
            buildings.Add(house);
        }
    }

    public Sprite GetSprite(BuildingInformation.TYPE_OF_BUILDING type)
    {
        return sprites[(int)type];
    }
}
