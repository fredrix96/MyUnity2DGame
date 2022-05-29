using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager
{
    GameObject go, canvasObject, shopImage;
    List<GameObject> imageObjects, textObjects;
    Canvas canvas;
    CanvasScaler cs;
    Image shop;
    CoinManager coinMan;
    BuildingManager buildings;
    PopUpMessage message;

    bool active;

    public ShopManager(CameraManager inCam, CoinManager inCoinMan, BuildingManager inBuildings, GridManager inGridMan)
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
        canvas.worldCamera = inCam.GetCamera();
        canvas.sortingLayerName = "UI";

        cs = canvasObject.AddComponent<CanvasScaler>();
        cs.referenceResolution = new Vector2(1920, 1080);

        // Image object
        shopImage = new GameObject { name = "shop" };
        shopImage.transform.SetParent(canvasObject.transform);
        shopImage.transform.localScale = new Vector2(3.0f, 7.0f);

        // Shop background
        shop = shopImage.AddComponent<Image>();
        Vector3 color = new Vector3(205, 133, 63);
        shop.color = new Color(color.x / 255f, color.y / 255f, color.z / 255f);

        shop.rectTransform.anchorMin = Vector2.zero;
        shop.rectTransform.anchorMax = Vector2.zero;
        shop.rectTransform.pivot = new Vector2(0, 0.5f);
        shop.rectTransform.anchoredPosition = new Vector3(0, canvas.pixelRect.height / 2, 0);

        CreateNewBuildingImage(BuildingInformation.TYPE_OF_BUILDING.Castle, new Vector3(canvas.pixelRect.width * 0.03f, canvas.pixelRect.height * 0.8f, 0), inCam, inGridMan);
        CreateNewBuildingImage(BuildingInformation.TYPE_OF_BUILDING.House, new Vector3(canvas.pixelRect.width * 0.08f, canvas.pixelRect.height * 0.8f, 0), inCam, inGridMan);

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

    void CreateNewBuildingImage(BuildingInformation.TYPE_OF_BUILDING type, Vector3 position, CameraManager inCam, GridManager inGridMan)
    {
        if (GameObject.Find(type.ToString()) == null)
        {
            GameObject imageObject = new GameObject() { name = type.ToString() };
            imageObject.transform.SetParent(canvasObject.transform);
            imageObject.AddComponent<BoxCollider2D>();
            message = imageObject.AddComponent<PopUpMessage>();
            message.Init(go, inCam);

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
            vendor.Init(imageObject, textObject, coinMan, inCam, inGridMan, type);

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
