using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager
{
    GameObject go, canvasObject, imageObject;
    List<GameObject> buildingImages;
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

        buildingImages = new List<GameObject>();

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
        imageObject = new GameObject { name = "shop" };
        imageObject.transform.SetParent(canvasObject.transform);
        imageObject.transform.localScale = new Vector2(3.0f, 7.0f);

        // Shop background
        shop = imageObject.AddComponent<Image>();
        Vector3 color = new Vector3(205, 133, 63);
        shop.color = new Color(color.x / 255f, color.y / 255f, color.z / 255f);

        shop.rectTransform.anchorMin = Vector2.zero;
        shop.rectTransform.anchorMax = Vector2.zero;
        shop.rectTransform.pivot = new Vector2(0, 0.5f);
        shop.rectTransform.anchoredPosition = new Vector3(0, canvas.pixelRect.height / 2, 0);

        // Castle
        GameObject castleObject = new GameObject() { name = "castle" };
        castleObject.transform.SetParent(canvasObject.transform);
        castleObject.AddComponent<BoxCollider2D>();
        message = castleObject.AddComponent<PopUpMessage>();
        message.Init(go, inCam);

        Image castle = castleObject.AddComponent<Image>();
        castle.sprite = buildings.GetSprite(BuildingInformation.TYPE_OF_BUILDING.CASTLE);

        castle.rectTransform.sizeDelta = new Vector2(0.5f, 0.5f);
        castle.rectTransform.anchorMin = Vector2.zero;
        castle.rectTransform.anchorMax = Vector2.zero;
        castle.rectTransform.anchoredPosition = new Vector3(20, canvas.pixelRect.height * 0.8f, 0);

        Vendor selector = castleObject.AddComponent<Vendor>();
        selector.Init(castleObject, coinMan, inCam, inGridMan, BuildingInformation.TYPE_OF_BUILDING.CASTLE);

        buildingImages.Add(castleObject);

        // Disable at start
        active = false;
        go.SetActive(active);
    }

    public void Update()
    {
        
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
}
