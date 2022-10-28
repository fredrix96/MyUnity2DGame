using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager
{
    GameObject go, canvasObject;
    List<GameObject> imageObjects;
    List<Text> texts;
    Canvas canvas;
    CanvasScaler scaler;
    Image shop;
    CoinManager coinMan;
    BuildingManager buildings;
    PopUpMessage message;
    Text title;

    public static bool active;

    public ShopManager(CoinManager inCoinMan, BuildingManager inBuildings)
    {
        coinMan = inCoinMan;
        buildings = inBuildings;

        imageObjects = new List<GameObject>();
        texts = new List<Text>();

        go = new GameObject() { name = "shopObject" };
        go.transform.SetParent(GameManager.GameManagerObject.transform);

        canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.worldCamera = CameraManager.GetCamera();
        canvas.sortingLayerName = "UI";
        canvas.sortingOrder = 100;

        scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        shop = UIManager.CreateImage(go, "shopImage", Resources.Load<Sprite>("Sprites/WoodenBackground"), new Vector2(-800, 0), new Vector2(300, 700)).GetComponent<Image>();
        title = UIManager.CreateText(go, "shopTitle", "Shop", 80, new Vector2(-800, 280), new Vector2(100, 100), TextAnchor.MiddleCenter);    

        // Creating building images
        CreateNewBuildingImage(BuildingInformation.TYPE_OF_BUILDING.Castle, new Vector2(-880, 190));
        CreateNewBuildingImage(BuildingInformation.TYPE_OF_BUILDING.House, new Vector2(-800, 190));
        CreateNewBuildingImage(BuildingInformation.TYPE_OF_BUILDING.Barrack_Spear, new Vector2(-720, 190));
        CreateNewBuildingImage(BuildingInformation.TYPE_OF_BUILDING.ArcheryTower, new Vector2(-880, 80));
        CreateNewBuildingImage(BuildingInformation.TYPE_OF_BUILDING.Barrack_Mace, new Vector2(-800, 80));

        // Disable at start
        active = false;
        go.SetActive(active);
    }

    public void Update()
    {
        // TODO: Find a more efficient way of doing this
        foreach (BuildingInformation.TYPE_OF_BUILDING type in System.Enum.GetValues(typeof(BuildingInformation.TYPE_OF_BUILDING)))
        {
            GameObject imageObject = GameObject.Find(type.ToString() + "_image");
            if (imageObject != null)
            {
                Image img = imageObject.GetComponent<Image>();

                if (BuildingInformation.MaxLimitReached(type))
                {
                    img.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
                }
                else if (img.color == new Color(0.3f, 0.3f, 0.3f, 0.5f))
                {
                    img.color = Color.white;
                }

                foreach (Text text in texts)
                {
                    if (text.name == type.ToString() + "_info")
                    {
                        UpdateBuildingTextInformation(text, type);
                    }
                }
            }
        }
    }

    public void ChangeActive()
    {
        if (active)
        {
            active = false;
        }
        else
        {
            active = true;
        }

        go.SetActive(active);
    }

    void CreateNewBuildingImage(BuildingInformation.TYPE_OF_BUILDING type, Vector2 inPos)
    {
        // If the type has not yet been created, create it
        if (GameObject.Find(type.ToString()) == null)
        {
            GameObject imageObject = UIManager.CreateImage(go, type.ToString() + "_image", buildings.GetSprite(type), inPos, new Vector2(70, 70));

            BoxCollider2D col = imageObject.AddComponent<BoxCollider2D>();
            col.size = new Vector2(100, 100);

            message = imageObject.AddComponent<PopUpMessage>();
            message.Init(go);

            Vendor vendor = imageObject.AddComponent<Vendor>();
            vendor.Init(imageObject, coinMan, type);

            imageObjects.Add(imageObject);

            Text text = UIManager.CreateText(go, type.ToString() + "_info", "", 15, new Vector2(inPos.x, inPos.y - 55), new Vector2(100, 100), TextAnchor.MiddleCenter);
            texts.Add(text);
        }
        else
        {
            Debug.LogWarning("Warning! Could not create image of " + type.ToString() + "! It already exists...");
        }
    }

    void UpdateBuildingTextInformation(Text text, BuildingInformation.TYPE_OF_BUILDING type)
    {
        text.text = "Cost: " + BuildingInformation.GetBuildingCost(type) + System.Environment.NewLine +
                BuildingInformation.GetCounter(type).ToString() + " / " + BuildingInformation.GetMax(type).ToString();
    }
}
