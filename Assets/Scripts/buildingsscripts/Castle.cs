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
        // Toolbar
        toolBarObject = new GameObject { name = go.name + "_toolBar" };
        toolBarObject.transform.SetParent(go.transform);

        // Canvas
        canvasToolBar = toolBarObject.AddComponent<Canvas>();
        canvasToolBar.transform.localScale = new Vector3(0.05f * CameraManager.GetCamera().aspect, 0.05f, 1f);

        // Sprite
        srToolBar = toolBarObject.AddComponent<SpriteRenderer>();
        srToolBar.sortingLayerName = "UI";
        srToolBar.sortingOrder = 0;
        srToolBar.sprite = Resources.Load<Sprite>("Sprites/WoodenBackground");
        srToolBar.drawMode = SpriteDrawMode.Sliced;
        srToolBar.size = new Vector2(100f, 100f);

        float height = 0;
        if (go.transform.GetComponent<BoxCollider2D>() == null)
        {
            Debug.Log("Warning: " + go.name + " does not have a box collider! Could not apply the correct height for the tool bar...");
        }
        else
        {
            height = srToolBar.sprite.bounds.max.y * 0.4f;
        }

        toolBarObject.transform.position = new Vector3(toolBarObject.transform.parent.position.x, toolBarObject.transform.parent.position.y + height, toolBarObject.transform.parent.position.z);
        toolBarObject.SetActive(false);

        // Text
        textObject = new GameObject { name = go.name + "textObject" };
        textObject.transform.SetParent(go.transform);
        textObject.transform.position = toolBarObject.transform.position;

        canvasText = textObject.AddComponent<Canvas>();
        canvasText.transform.localScale = new Vector3(0.05f * CameraManager.GetCamera().aspect, 0.05f, 1f);
        canvasText.sortingLayerName = "UI";
        canvasText.sortingOrder = 1;

        text = textObject.AddComponent<Text>();
        text.text = "The king is alive";
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = (int)(1.5f * Graphics.resolution);
        text.color = Color.white;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;

        textObject.SetActive(false);
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
            text.text = "The king respawns in " + remainingTime.ToString("F2");

            if (remainingTime <= 0)
            {
                player.Respawn();
                message.SendPopUpMessage("A new King has arrived!" + System.Environment.NewLine + "All hail the new King!", 2.5f);
                remainingTime = timeToRespawn;
            }
        }
    }

}
