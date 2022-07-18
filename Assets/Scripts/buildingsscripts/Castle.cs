using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Castle : Building
{
    Player player;
    PopUpMessage message;
    double remainingTime;
    float timeToRespawn;

    public Castle(GameObject parent, Tile inPos, CoinManager inCoinMan, List<Building> inBuildings, Player inPlayer)
    {
        type = BuildingInformation.TYPE_OF_BUILDING.Castle;

        centerTile = inPos;
        coinMan = inCoinMan;
        buildings = inBuildings;
        player = inPlayer;

        timeToRespawn = 10.0f;
        remainingTime = timeToRespawn;

        go = new GameObject { name = "building_" + type.ToString() + BuildingInformation.GetCounter(type) };
        go.transform.SetParent(parent.transform);
        go.layer = LayerMask.NameToLayer("Buildings");

        message = go.AddComponent<PopUpMessage>();
        message.Init(go);

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

        RespawnTimer();
    }

    void CreateToolBar()
    {
        CreateToolbarObject(new Vector2(10f, 10f), 0.3f);

        CreateInfoText("The king is alive", TextAnchor.MiddleCenter, new Vector2(30, 25));
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
            text.text = "The king respawns" + System.Environment.NewLine + "in " + remainingTime.ToString("F2") + " seconds";

            if (remainingTime <= 0)
            {
                player.Respawn();
                message.SendPopUpMessage("A new King has arrived!" + System.Environment.NewLine + "All hail the new King!", 2.5f);
                remainingTime = timeToRespawn;
            }
        }
    }

}
