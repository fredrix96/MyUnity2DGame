using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Barrack : Building
{
    float spawnDelay;
    double timer;

    public Barrack(GameObject parent, Tile inPos, CoinManager inCoinMan, List<Building> inBuildings, BuildingInformation.TYPE_OF_BUILDING inType)
    {
        type = inType;

        centerTile = inPos;
        coinMan = inCoinMan;
        buildings = inBuildings;

        Vector2 spawnLocation = new Vector2(centerTile.GetTilePosition().x, centerTile.GetTilePosition().y + BuildingInformation.GetBuildingSize(type).y / 2);

        if (type == BuildingInformation.TYPE_OF_BUILDING.Barrack_Spear)
        {
            Debug.Log("Spearmen spawn location set at: " + spawnLocation);
            CharacterInformation.SetSpawnLocation(CharacterInformation.TYPE_OF_SOLDIER.Spearman, spawnLocation);
        }
        else if (type == BuildingInformation.TYPE_OF_BUILDING.Barrack_Mace)
        {
            Debug.Log("Macemen spawn location set at: " + spawnLocation);
            CharacterInformation.SetSpawnLocation(CharacterInformation.TYPE_OF_SOLDIER.Maceman, spawnLocation);
        }
        else if (type == BuildingInformation.TYPE_OF_BUILDING.Barrack_HeavySword)
        {
            Debug.Log("HeavySwordmen spawn location set at: " + spawnLocation);
            CharacterInformation.SetSpawnLocation(CharacterInformation.TYPE_OF_SOLDIER.HeavySwordman, spawnLocation);
        }

        spawnDelay = 2;

        go = new GameObject { name = "building_" + type.ToString() + BuildingInformation.GetCounter(type).ToString() };
        go.transform.SetParent(parent.transform);
        go.layer = LayerMask.NameToLayer("Buildings");

        PositionRendererSorter prs = go.AddComponent<PositionRendererSorter>();
        prs.SetIsOnlyRunOnce();
        go.AddComponent<CollisionManager>();

        message = go.AddComponent<PopUpMessage>();
        message.Init(go);

        sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("Sprites/" + type.ToString());

        go.transform.position = inPos.GetWorldPos();

        MarkOrUnmarkTiles(type, inPos, true);

        collider = go.AddComponent<BoxCollider2D>();
        rb = go.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.useFullKinematicContacts = true;

        CreateHealthBar(type);
        CreateToolBar();

        collider.size = new Vector2(sr.size.x, sr.size.y / 2);
        collider.offset = new Vector2(0, -(sr.size.y / 4));

        AddSelector();

        BuildingInformation.IncreaseCounter(type);
    }

    public override void Update()
    {
        CheckIfDestroyed();

        LookIfIgnored();

        timer += Time.deltaTime;
        if (timer > spawnDelay)
        {
            SpawnSoldier();
            timer = 0;
        }
    }

    void CreateToolBar()
    {
        CreateCanvas();

        CreateToolbarObject(new Vector2(3f, 1f), 2f);

        if (type == BuildingInformation.TYPE_OF_BUILDING.Barrack_Spear)
        {
            CreateInfoText("Spearmen", 25, TextAnchor.MiddleCenter, new Vector2(65, 100));
        }
        else if (type == BuildingInformation.TYPE_OF_BUILDING.Barrack_Mace)
        {
            CreateInfoText("Macemen", 25, TextAnchor.MiddleCenter, new Vector2(65, 100));
        }
        else if (type == BuildingInformation.TYPE_OF_BUILDING.Barrack_HeavySword)
        {
            CreateInfoText("HeavySwordmen", 20, TextAnchor.MiddleCenter, new Vector2(65, 100));
        }
    }

    void SpawnSoldier()
    {
        // Are there any humans available?
        if (HumansCounter.nrOfHumans != 0)
        {
            HumansCounter.nrOfHumans--;
            RemoveHumanFromRandomHouse();

            if (type == BuildingInformation.TYPE_OF_BUILDING.Barrack_Spear)
            {
                SoldierCounter_Spearmen.nrToSpawn++;
            }
            else if (type == BuildingInformation.TYPE_OF_BUILDING.Barrack_Mace)
            {
                SoldierCounter_Macemen.nrToSpawn++;
            }
            else if (type == BuildingInformation.TYPE_OF_BUILDING.Barrack_HeavySword)
            {
                SoldierCounter_HeavySwordmen.nrToSpawn++;
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

        int index = UnityEngine.Random.Range(0, houses.Count - 1);
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
