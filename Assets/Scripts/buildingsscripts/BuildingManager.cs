using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        sprites.Add(Resources.Load<Sprite>("Sprites/Barrack_HeavySword"));
        sprites.Add(Resources.Load<Sprite>("Sprites/ArcheryTower"));
        sprites.Add(Resources.Load<Sprite>("Sprites/Goldmine"));

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
            Castle castle = new Castle(go, inPos, buildings, player);
            buildings.Add(castle);
        }
        else if (type == BuildingInformation.TYPE_OF_BUILDING.House)
        {
            House house = new House(go, inPos, buildings);
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
        else if (type == BuildingInformation.TYPE_OF_BUILDING.Barrack_HeavySword)
        {
            Barrack barrack = new Barrack(go, inPos, coinMan, buildings, type);
            buildings.Add(barrack);
        }
        else if (type == BuildingInformation.TYPE_OF_BUILDING.ArcheryTower)
        {
            ArcheryTower archeryTower = new ArcheryTower(go, inPos, buildings);
            buildings.Add(archeryTower);
        }
        else if (type == BuildingInformation.TYPE_OF_BUILDING.Goldmine)
        {
            Goldmine goldmine = new Goldmine(go, inPos, coinMan, buildings);
            buildings.Add(goldmine);
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
