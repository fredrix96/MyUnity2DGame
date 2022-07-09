using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Barrack : Building
{
    int soldierCost;

    public Barrack(GameObject parent, Tile inPos, CoinManager inCoinMan)
    {
        type = BuildingInformation.TYPE_OF_BUILDING.Barrack;

        centerTile = inPos;
        coinMan = inCoinMan;

        soldierCost = 10;

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
        // Toolbar
        toolBarObject = new GameObject { name = go.name + "_toolBar" };
        toolBarObject.transform.SetParent(go.transform);
        toolBarObject.AddComponent<BoxCollider2D>();

        // Canvas
        canvasToolBar = toolBarObject.AddComponent<Canvas>();
        canvasToolBar.transform.localScale = new Vector3(0.05f * CameraManager.GetCamera().aspect, 0.05f, 1f);

        // Sprite
        srToolBar = toolBarObject.AddComponent<SpriteRenderer>();
        srToolBar.sortingLayerName = "UI";
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
        textObject = new GameObject { name = go.name + "_textObject" };
        textObject.transform.SetParent(go.transform);
        textObject.transform.position = toolBarObject.transform.position;

        canvasText = textObject.AddComponent<Canvas>();
        canvasText.transform.localScale = new Vector3(0.05f * CameraManager.GetCamera().aspect, 0.05f, 1f);
        canvasText.sortingLayerName = "UI";
        canvasText.sortingOrder = 1;

        text = textObject.AddComponent<Text>();
        text.text = "Available Humans: " + HumansCounter.nrOfHumans;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = (int)(1.5f * Graphics.resolution);
        text.color = Color.white;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.UpperCenter;

        // Button
        buttonObject = new GameObject { name = go.name + "_trainSoldierButton" };
        buttonObject.transform.SetParent(go.transform);
        buttonObject.transform.position = toolBarObject.transform.position;
        buttonObject.AddComponent<GraphicRaycaster>();

        canvasButton = buttonObject.GetComponent<Canvas>();
        canvasButton.transform.SetParent(buttonObject.transform);
        canvasButton.transform.position = buttonObject.transform.position;
        canvasButton.transform.localScale = new Vector3(0.02f * CameraManager.GetCamera().aspect, 0.02f, 1f);
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
            }
        }
    }
}
