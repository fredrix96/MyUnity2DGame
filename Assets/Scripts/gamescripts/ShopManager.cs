using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager
{
    GameObject go, canvasObject, shopImageObject, titleObject;
    List<GameObject> imageObjects, textObjects;
    Canvas canvas;
    CanvasScaler cs;
    SpriteRenderer shop;
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
        textObjects = new List<GameObject>();

        go = new GameObject() { name = "shopObject" };
        go.transform.SetParent(GameManager.GameManagerObject.transform);

        // Canvas
        canvasObject = new GameObject { name = "canvas" };
        canvasObject.transform.parent = go.transform;

        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = CameraManager.GetCamera();
        canvas.sortingLayerName = "UI";
        canvas.sortingOrder = 1;

        cs = canvasObject.AddComponent<CanvasScaler>();
        cs.referenceResolution = new Vector2(1920, 1080);

        // Image object
        shopImageObject = new GameObject { name = "shop" };
        shopImageObject.transform.SetParent(canvasObject.transform);

        // Shop background
        shop = shopImageObject.AddComponent<SpriteRenderer>();
        shop.sprite = Resources.Load<Sprite>("Sprites/WoodenBackground");
        shop.drawMode = SpriteDrawMode.Sliced;
        shop.size = new Vector2(4f, 8f);
        shop.sortingLayerName = "UI";

        RectTransform rect = shopImageObject.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(canvas.pixelRect.width * 0.1f, canvas.pixelRect.height / 2);
        rect.sizeDelta = shop.size;

        // Shop title
        titleObject = new GameObject { name = "shopTitle" };
        titleObject.transform.SetParent(canvasObject.transform);
        titleObject.transform.localScale = new Vector3(2, 2, 0);

        RectTransform rectTitle = titleObject.AddComponent<RectTransform>();
        rectTitle.anchorMin = Vector2.zero;
        rectTitle.anchorMax = Vector2.zero;
        rectTitle.pivot = new Vector2(0.5f, 0.5f);
        rectTitle.anchoredPosition = new Vector2(canvas.pixelRect.width * 0.1f, canvas.pixelRect.height * 0.7f);
        rectTitle.sizeDelta = new Vector2(0.05f * canvas.pixelRect.width, 0.13f * canvas.pixelRect.height);

        title = titleObject.AddComponent<Text>();
        title.text = "Shop";
        title.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        title.fontSize = (int)(4 * Graphics.resolution);
        title.color = Color.white;
        title.fontStyle = FontStyle.Bold;
        title.alignment = TextAnchor.UpperCenter;

        CreateNewBuildingImage(BuildingInformation.TYPE_OF_BUILDING.Castle, new Vector3(canvas.pixelRect.width * 0.05f, canvas.pixelRect.height * 0.7f, 0));
        CreateNewBuildingImage(BuildingInformation.TYPE_OF_BUILDING.House, new Vector3(canvas.pixelRect.width * 0.1f, canvas.pixelRect.height * 0.7f, 0));
        CreateNewBuildingImage(BuildingInformation.TYPE_OF_BUILDING.Barrack, new Vector3(canvas.pixelRect.width * 0.15f, canvas.pixelRect.height * 0.7f, 0));

        // Disable at start
        active = false;
        go.SetActive(active);
    }

    public void Update()
    {
        // TODO: Find a more efficient way of doing this
        foreach (BuildingInformation.TYPE_OF_BUILDING type in System.Enum.GetValues(typeof(BuildingInformation.TYPE_OF_BUILDING)))
        {
            GameObject imageObject = GameObject.Find(type.ToString());
            if (imageObject != null)
            {
                Image img = imageObject.GetComponent<Image>();

                if (BuildingInformation.MaxLimitReached(type))
                {
                    img.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
                }
                else
                {
                    img.color = Color.white;
                }

                GameObject textObject = GameObject.Find(type.ToString() + "_info");
                if (textObject != null)
                {
                    UpdateBuildingTextInformation(textObject.GetComponent<Text>(), type);
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

    void CreateNewBuildingImage(BuildingInformation.TYPE_OF_BUILDING type, Vector3 position)
    {
        if (GameObject.Find(type.ToString()) == null)
        {
            GameObject imageObject = new GameObject() { name = type.ToString() };
            imageObject.transform.SetParent(canvasObject.transform);
            imageObject.AddComponent<BoxCollider2D>();
            message = imageObject.AddComponent<PopUpMessage>();
            message.Init(go);

            Image image = imageObject.AddComponent<Image>();
            image.sprite = buildings.GetSprite(type);

            image.rectTransform.sizeDelta = new Vector2(0.8f, 0.8f);
            image.rectTransform.anchorMin = Vector2.zero;
            image.rectTransform.anchorMax = Vector2.zero;
            image.rectTransform.anchoredPosition = position;

            GameObject textObject = new GameObject() { name = type.ToString() + "_info" };
            textObject.transform.SetParent(canvasObject.transform);

            Text text = textObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.transform.localScale = new Vector3(1, 1, 1);
            text.color = Color.black;
            text.alignment = TextAnchor.MiddleCenter;

            text.rectTransform.anchorMin = Vector2.zero;
            text.rectTransform.anchorMax = Vector2.zero;
            text.rectTransform.anchoredPosition = new Vector3(position.x, position.y - canvasObject.GetComponent<Canvas>().pixelRect.height * 0.07f, position.z);

            textObjects.Add(textObject);

            Vendor vendor = imageObject.AddComponent<Vendor>();
            vendor.Init(imageObject, textObject, coinMan, type);

            imageObjects.Add(imageObject);
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
