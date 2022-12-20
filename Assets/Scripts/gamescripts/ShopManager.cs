using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager
{
    GameObject go, canvasObject;
    List<GameObject> buyObjects, upgradeObjects;
    List<Text> buyTexts, upgradeTexts;
    Canvas canvas;
    CanvasScaler scaler;
    Image shop;
    CoinManager coinMan;
    BuildingManager buildings;
    PopUpMessage message;
    Text title;
    Button buildingsButton, upgradesButton;
    bool onBuildings, onUpgrades;

    public static bool active;

    public ShopManager(CoinManager inCoinMan, BuildingManager inBuildings)
    {
        coinMan = inCoinMan;
        buildings = inBuildings;

        onBuildings = true;
        onUpgrades = false;

        buyObjects = new List<GameObject>();
        buyTexts = new List<Text>();

        upgradeObjects = new List<GameObject>();
        upgradeTexts = new List<Text>();

        go = new GameObject() { name = "shopObject" };
        go.transform.SetParent(GameManager.GameManagerObject.transform);

        canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.worldCamera = CameraManager.GetCamera();

        scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        shop = UIManager.CreateImage(go, "shopImage", Resources.Load<Sprite>("Sprites/WoodenBackground"), new Vector2(-800, 0), new Vector2(300, 700)).GetComponent<Image>();
        title = UIManager.CreateText(go, "shopTitle", "Store", 50, new Vector2(-800, 310), new Vector2(130, 100), TextAnchor.MiddleCenter);
        buildingsButton = UIManager.CreateButton(go, "buildings", Resources.Load<Sprite>("Sprites/BuildingsButton"), new Vector2(100, 50), new Vector2(-870, 250), ChangeToBuildings);
        upgradesButton = UIManager.CreateButton(go, "upgrades", Resources.Load<Sprite>("Sprites/UpgradesButton"), new Vector2(100, 50), new Vector2(-730, 250), ChangeToUpgrades);

        // Creating building images
        CreateNewBuilding(BuildingInformation.TYPE_OF_BUILDING.Castle, new Vector2(-880, 180));
        CreateNewBuilding(BuildingInformation.TYPE_OF_BUILDING.House, new Vector2(-800, 180));
        CreateNewBuilding(BuildingInformation.TYPE_OF_BUILDING.Goldmine, new Vector2(-720, 180));
        CreateNewBuilding(BuildingInformation.TYPE_OF_BUILDING.Barrack_Spear, new Vector2(-880, 70));
        CreateNewBuilding(BuildingInformation.TYPE_OF_BUILDING.Barrack_Mace, new Vector2(-800, 70));
        CreateNewBuilding(BuildingInformation.TYPE_OF_BUILDING.Barrack_HeavySword, new Vector2(-720, 70));
        CreateNewBuilding(BuildingInformation.TYPE_OF_BUILDING.ArcheryTower, new Vector2(-880, -40));

        CreateNewUpgrade(UpgradeManager.TYPE_OF_UPGRADE.Building_Health, new Vector2(-880, 180));
        //CreateNewUpgrade(UpgradeManager.TYPE_OF_UPGRADE.House, new Vector2(-800, 180));
        //CreateNewUpgrade(UpgradeManager.TYPE_OF_UPGRADE.Goldmine, new Vector2(-720, 180));
        //CreateNewUpgrade(UpgradeManager.TYPE_OF_UPGRADE.Barrack_Spear, new Vector2(-880, 70));
        //CreateNewUpgrade(UpgradeManager.TYPE_OF_UPGRADE.Barrack_Mace, new Vector2(-800, 70));
        //CreateNewUpgrade(UpgradeManager.TYPE_OF_UPGRADE.Barrack_HeavySword, new Vector2(-720, 70));
        //CreateNewUpgrade(UpgradeManager.TYPE_OF_UPGRADE.ArcheryTower, new Vector2(-880, -40));

        // Just a trick to make sure that the upgrade images don't spawn at the buildings page
        ChangeToUpgrades();
        ChangeToBuildings();

        canvas.sortingLayerName = "UI";
        canvas.sortingOrder = 100;

        // Disable at start
        active = false;
        go.SetActive(active);
    }

    public void Update()
    {
        if (onBuildings)
        {
            // TODO: Find a more efficient way of doing this
            foreach (BuildingInformation.TYPE_OF_BUILDING type in System.Enum.GetValues(typeof(BuildingInformation.TYPE_OF_BUILDING)))
            {
                GameObject imageObject = GameObject.Find(type.ToString() + "_buy");
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

                    foreach (Text text in buyTexts)
                    {
                        if (text.name == type.ToString() + "_info_building")
                        {
                            UpdateBuildingTextInformation(text, type);
                        }
                    }
                }
            }
        }
        else if (onUpgrades)
        {
            // TODO: Find a more efficient way of doing this
            foreach (UpgradeManager.TYPE_OF_UPGRADE type in System.Enum.GetValues(typeof(UpgradeManager.TYPE_OF_UPGRADE)))
            {
                GameObject imageObject = GameObject.Find(type.ToString() + "_upgrade");
                if (imageObject != null)
                {
                    Image img = imageObject.GetComponent<Image>();

                    if (UpgradeManager.MaxUpgradeLevelReached(type))
                    {
                        img.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
                    }
                    else if (img.color == new Color(0.3f, 0.3f, 0.3f, 0.5f))
                    {
                        img.color = Color.white;
                    }

                    foreach (Text text in upgradeTexts)
                    {
                        if (text.name == type.ToString() + "_info_upgrade")
                        {
                            UpdateUpgradeTextInformation(text, type);
                        }
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

    void ChangeToUpgrades()
    {
        if (onBuildings)
        {
            HideBuildings();
            ShowUpgrades();
            onBuildings = false;
            onUpgrades = true;
        }
    }

    void ChangeToBuildings()
    {
        if (onUpgrades)
        {
            HideUpgrades();
            ShowBuildings();
            onUpgrades = false;
            onBuildings = true;
        }
    }

    void CreateNewBuilding(BuildingInformation.TYPE_OF_BUILDING type, Vector2 inPos)
    {
        // If the type has not yet been created, create it
        if (GameObject.Find(type.ToString() + "_buy") == null)
        {
            GameObject buyObject = UIManager.CreateImage(go, type.ToString() + "_buy", buildings.GetSprite(type), inPos, new Vector2(70, 70));

            BoxCollider2D col = buyObject.AddComponent<BoxCollider2D>();
            col.size = new Vector2(100, 100);

            message = buyObject.AddComponent<PopUpMessage>();
            message.Init(go);

            Vendor vendor = buyObject.AddComponent<Vendor>();
            vendor.InitBuilding(buyObject, coinMan, type);

            buyObjects.Add(buyObject);

            Text text = UIManager.CreateText(go, type.ToString() + "_info_building", "", 15, new Vector2(inPos.x, inPos.y - 55), new Vector2(100, 100), TextAnchor.MiddleCenter);
            buyTexts.Add(text);
        }
        else
        {
            Debug.LogWarning("Warning! Could not create image of " + type.ToString() + "! It already exists...");
        }
    }

    void CreateNewUpgrade(UpgradeManager.TYPE_OF_UPGRADE type, Vector2 inPos)
    {
        // If the type has not yet been created, create it
        if (GameObject.Find(type.ToString() + "_upgrade") == null)
        {
            GameObject upgradeObject = UIManager.CreateImage(go, type.ToString() + "_upgrade", UpgradeManager.GetSprite(type), inPos, new Vector2(70, 70));

            BoxCollider2D col = upgradeObject.AddComponent<BoxCollider2D>();
            col.size = new Vector2(100, 100);
            
            message = upgradeObject.AddComponent<PopUpMessage>();
            message.Init(go);
            
            Vendor vendor = upgradeObject.AddComponent<Vendor>();
            vendor.InitUpgrade(upgradeObject, coinMan, type);
            
            upgradeObjects.Add(upgradeObject);
            
            Text text = UIManager.CreateText(go, type.ToString() + "_info_upgrade", "", 15, new Vector2(inPos.x, inPos.y - 55), new Vector2(100, 100), TextAnchor.MiddleCenter);
            upgradeTexts.Add(text);
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

    void UpdateUpgradeTextInformation(Text text, UpgradeManager.TYPE_OF_UPGRADE type)
    {
        text.text = "Cost: " + UpgradeManager.GetUpgradeCost(type) + System.Environment.NewLine +
                UpgradeManager.GetLevel(type).ToString() + " / " + UpgradeManager.GetMaxLevel(type).ToString();
    }

    void HideBuildings()
    {
        foreach (GameObject buyObject in buyObjects)
        {
            if (buyObject != null)
            {
                buyObject.SetActive(false);
            }
        }

        foreach (Text text in buyTexts)
        {
            if (text != null)
            {
                text.enabled = false;
            }
        }
    }

    void ShowBuildings()
    {
        foreach (GameObject imageObject in buyObjects)
        {
            if (imageObject != null)
            {
                imageObject.SetActive(true);
            }
        }

        foreach (Text text in buyTexts)
        {
            if (text != null)
            {
                text.enabled = true;
            }
        }
    }

    void HideUpgrades()
    {
        foreach (GameObject upgradeObject in upgradeObjects)
        {
            if (upgradeObject != null)
            {
                upgradeObject.SetActive(false);
            }
        }

        foreach (Text text in upgradeTexts)
        {
            if (text != null)
            {
                text.enabled = false;
            }
        }
    }

    void ShowUpgrades()
    {
        foreach (GameObject upgradeObject in upgradeObjects)
        {
            if (upgradeObject != null)
            {
                upgradeObject.SetActive(true);
            }
        }

        foreach (Text text in upgradeTexts)
        {
            if (text != null)
            {
                text.enabled = true;
            }
        }
    }
}
