using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class BuildingInformation
{
    public static int CastleCounter = 0;

    public enum TYPE_OF_BUILDING
    {
        CASTLE
    }

    static readonly int[] cost = new int[]
    {
        50
    };

    // The scaling works better for now if the sizes are in odd numbers to make sure that there is always a tile in the center
    static readonly Vector2[] size = new Vector2[] 
    {
        new Vector2(5,9)
    };

    public static int GetBuildingCost(TYPE_OF_BUILDING type)
    {
        return cost[(int)type];
    }

    public static Vector2 GetBuildingSize(TYPE_OF_BUILDING type)
    {
        return size[(int)type];
    }
}

public class BuildingManager : MonoBehaviour
{
    GameObject go, canvasObject;
    Canvas canvas;
    CanvasScaler cs;
    List<Building> buildings;
    List<Sprite> sprites;

    void Start()
    {
    }

    public void Init(CameraManager inCam)
    {
        go = new GameObject() { name = "buildings" };
        go.transform.SetParent(GameManager.GameManagerObject.transform);

        buildings = new List<Building>();
        sprites = new List<Sprite>();

        sprites.Add(Resources.Load<Sprite>("Sprites/Castle"));

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

    public void CreateBuilding(BuildingInformation.TYPE_OF_BUILDING type, Tile inPos, GridManager inGridMan)
    {
        if (type == BuildingInformation.TYPE_OF_BUILDING.CASTLE)
        {
            Castle castle = new Castle(go, inPos, inGridMan);
            buildings.Add(castle);
        }
    }

    public Sprite GetSprite(BuildingInformation.TYPE_OF_BUILDING type)
    {
        return sprites[(int)type];
    }
}
