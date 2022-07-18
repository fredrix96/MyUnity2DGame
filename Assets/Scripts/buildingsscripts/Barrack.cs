using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Barrack : Building
{
    int soldierCost;

    public Barrack(GameObject parent, Tile inPos, CoinManager inCoinMan, List<Building> inBuildings)
    {
        type = BuildingInformation.TYPE_OF_BUILDING.Barrack;

        centerTile = inPos;
        coinMan = inCoinMan;
        buildings = inBuildings;

        soldierCost = 50;

        go = new GameObject { name = "building_" + type.ToString() + BuildingInformation.GetCounter(type).ToString() };
        go.transform.SetParent(parent.transform);
        go.layer = LayerMask.NameToLayer("Buildings");

        go.AddComponent<CollisionManager>();

        sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("Sprites/" + type.ToString());
        sr.sortingLayerName = "Buildings";

        go.transform.position = inPos.GetWorldPos();

        MarkOrUnmarkTiles(type, inPos, true);

        collider = go.AddComponent<BoxCollider2D>();
        rb = go.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.useFullKinematicContacts = true;

        CreateHealthBar(type);
        CreateToolBar();

        selector = go.AddComponent<Selector>();
        selector.Init(toolBarObject, sr, textObject, buttonObject);
        selector.SetOutlineColor(Color.blue);
        selector.SetWidth(5);

        BuildingInformation.IncreaseCounter(type);
    }

    public override void Update()
    {
        CheckIfDestroyed();

        LookIfIgnored();

        // Update text
        text.text = "Available Humans: " + HumansCounter.nrOfHumans;
    }

    void CreateToolBar()
    {
        CreateToolbarObject(new Vector2(10f, 12f), 0.4f);

        CreateInfoText("Available Humans: " + HumansCounter.nrOfHumans, TextAnchor.UpperCenter, new Vector2(30, 15));

        // Button
        buttonObject = new GameObject { name = go.name + "_trainSoldierButton" };
        buttonObject.transform.SetParent(go.transform);
        buttonObject.transform.position = toolBarObject.transform.position;
        buttonObject.AddComponent<GraphicRaycaster>();
        buttonObject.AddComponent<ButtonEvents>();

        canvasButton = buttonObject.GetComponent<Canvas>();
        canvasButton.transform.SetParent(buttonObject.transform);
        canvasButton.transform.position = new Vector2(buttonObject.transform.position.x, buttonObject.transform.position.y - 0.3f);
        canvasButton.transform.localScale = new Vector3(0.02f * CameraManager.GetCamera().aspect, 0.01f, 1f);
        canvasButton.sortingLayerName = "UI";
        canvasButton.sortingOrder = 2;

        button = buttonObject.AddComponent<Button>();
        button.transform.position = canvasButton.transform.position;
        button.transform.SetParent(canvasButton.transform);

        button.image = buttonObject.AddComponent<Image>();
        button.image.sprite = Resources.Load<Sprite>("Sprites/Coin");
        button.targetGraphic = button.image;

        button.onClick.AddListener(SpawnSoldier);

        buttonObject.SetActive(false);
    }

    void SpawnSoldier()
    {
        // Are there any humans available?
        if (HumansCounter.nrOfHumans != 0)
        {
            if (coinMan.SpendMoney(soldierCost))
            {
                HumansCounter.nrOfHumans--;
                SoldierCounter.nrToSpawn++;
                RemoveHumanFromRandomHouse();
            }
        }
    }

    void RemoveHumanFromRandomHouse()
    {
        List<House> houses = new List<House>();
        foreach (Building building in buildings)
        {
            if (building is House)
            {
                House house = building as House;
                if (house.GetNrOfHumans() > 0)
                {
                    houses.Add(house);
                }
            }
        }

        int index = Random.Range(0, houses.Count - 1);
        houses[index].RemoveHuman();
    }

    public void CheckIfDestroyed()
    {
        if (health.GetHealth() <= 0)
        {
            shouldBeRemoved = true;
        }
    }
}
