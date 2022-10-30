using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Castle : Building
{
    Player player;
    double remainingTime;
    float timeToRespawn;
    double timer;
    float spawnDelay;

    public Castle(GameObject parent, Tile inPos, CoinManager inCoinMan, List<Building> inBuildings, Player inPlayer)
    {
        type = BuildingInformation.TYPE_OF_BUILDING.Castle;

        centerTile = inPos;
        buildings = inBuildings;
        coinMan = inCoinMan;
        player = inPlayer;

        timer = 0;
        spawnDelay = 10.0f;

        Vector2 spawnLocation = new Vector2(centerTile.GetTilePosition().x, centerTile.GetTilePosition().y + BuildingInformation.GetBuildingSize(type).y / 2);

        Debug.Log("Knights spawn location set at: " + spawnLocation);
        CharacterInformation.SetSpawnLocation(CharacterInformation.TYPE_OF_SOLDIER.Knight, spawnLocation);

        timeToRespawn = 3.0f;
        remainingTime = timeToRespawn;

        go = new GameObject { name = "building_" + type.ToString() + BuildingInformation.GetCounter(type) };
        go.transform.SetParent(parent.transform);
        go.layer = LayerMask.NameToLayer("Buildings");

        message = go.AddComponent<PopUpMessage>();
        message.Init(go);

        go.AddComponent<PositionRendererSorter>();
        go.AddComponent<CollisionManager>();

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

        selector = go.AddComponent<Selector>();
        selector.Init(toolBarObject, sr, textObject);
        selector.SetOutlineColor(Color.blue);
        selector.SetWidth(5);

        shouldBeRemoved = false;

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

        //RespawnTimer();
    }

    void CreateToolBar()
    {
        CreateCanvas();

        CreateToolbarObject(new Vector2(3.5f, 1.5f), 2f);

        CreateInfoText("Knights", 25, TextAnchor.MiddleCenter, new Vector2(60, 70));
    }

    void SpawnSoldier()
    {
        // Are there any humans available?
        if (HumansCounter.nrOfHumans != 0)
        {
            HumansCounter.nrOfHumans--;
            RemoveHumanFromRandomHouse();

            SoldierCounter_Knights.nrToSpawn++;
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

    void RespawnTimer()
    {
        if (!player.IsDead())
        {
            text.text = "The king is alive";
        }
        else if (player.GetPlayerObject() == null)
        {
            remainingTime -= Time.deltaTime;
            text.text = "The king" + System.Environment.NewLine + "respawns in" + System.Environment.NewLine + remainingTime.ToString("F2") + " sec";

            if (remainingTime <= 0)
            {
                float offset = BuildingInformation.GetBuildingSize(type).y;
                Tile spawnTile = GridManager.GetTile(new Vector2(centerTile.GetTilePosition().x, centerTile.GetTilePosition().y + offset / 2));

                player.Respawn(spawnTile);
                message.SendPopUpMessage("A new King has arrived!" + System.Environment.NewLine + "All hail the new King!", 2.5f);
                remainingTime = timeToRespawn;
            }
        }
    }

}
