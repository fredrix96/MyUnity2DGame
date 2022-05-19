using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Selector : MonoBehaviour
{
    GameObject go;
    GameObject tmpObject;
    Image img;
    CoinManager coinMan;
    Color color;
    CameraManager cam;
    GridManager gridMan;

    int type;
    int counter;
    int cost;
    bool draging;

    void Start()
    {
        counter = 0;
        img = GetComponent<Image>();
    }

    public void Init(GameObject inGo, CoinManager inCoinMan, CameraManager inCam, GridManager inGridMan, BuildingInformation.TYPE_OF_BUILDING inType)
    {
        go = inGo;
        coinMan = inCoinMan;
        cam = inCam;
        gridMan = inGridMan;

        img = go.GetComponent<Image>();
        img.color = Color.white;
        color = new Color();
        type = (int)inType;
        cost = BuildingInformation.GetBuildingCost((BuildingInformation.TYPE_OF_BUILDING)type);
    }

    void OnMouseEnter()
    {
        color = img.color;
        img.color = new Color(0.3f, 0.3f, 0.8f, 1.0f);
    }

    void OnMouseExit()
    {
        img.color = color;
    }

    void OnMouseDown()
    {
        if (coinMan.SpendMoney(cost))
        {
            CreateBuildingImage();

            Vector3 screenPoint = Input.mousePosition;
            screenPoint.z = cam.GetCamera().nearClipPlane;
            tmpObject.transform.position = cam.GetCamera().ScreenToWorldPoint(screenPoint);

            draging = true;
            counter++;

            // Hide shop UI
            go.GetComponentInParent<Canvas>().enabled = false;
        }
    }

    void OnMouseDrag()
    {
        if (draging)
        {
            Vector3 screenPoint = Input.mousePosition;

            // Distance of the plane from the camera
            screenPoint.z = cam.GetCamera().nearClipPlane;

            Vector3 worldPoint = cam.GetCamera().ScreenToWorldPoint(screenPoint);
            Tile tile = gridMan.GetTileFromWorldPosition(worldPoint);

            // If the mouse pointer is not on a tile, find the closest tile
            if (tile == null)
            {
                tile = gridMan.FindClosestTile(worldPoint);
            }

            // If there is no obstacle, follow the mouse
            if (AvoidObstacles(tile))
            {
                tmpObject.transform.position = tile.GetPos();
                tmpObject.GetComponent<Image>().color = Color.white;
            }
            else
            {
                // Indicate with red color that the building can not be placed here
                tmpObject.GetComponent<Image>().color = new Color(0.8f, 0.2f, 0.2f, 0.8f);
            }
        }
    }

    void OnMouseUp()
    {
        if (draging)
        {
            Vector3 screenPoint = Input.mousePosition;

            // Distance of the plane from the camera
            screenPoint.z = cam.GetCamera().nearClipPlane;

            Vector3 worldPoint = cam.GetCamera().ScreenToWorldPoint(screenPoint);
            Tile tile = gridMan.GetTileFromWorldPosition(worldPoint);

            // If there is no obstacle, create the building
            if (AvoidObstacles(tile))
            {
                // Use Gameobject.find?
                GameManager.GameManagerObject.GetComponent<BuildingManager>().CreateBuilding((BuildingInformation.TYPE_OF_BUILDING)type, tmpObject.transform.position, gridMan);
            }
            else
            {
                go.GetComponent<PopUpMessage>().SendPopUpMessage("You can not place the building at an obstacle!", 1.5f);
                coinMan.AddCoins(cost);
            }

            Destroy(tmpObject);

            draging = false;

            // Show shop UI
            go.GetComponentInParent<Canvas>().enabled = true;
        }
    }

    void CreateBuildingImage()
    {
        tmpObject = Instantiate(go);
        tmpObject.transform.SetParent(GameManager.GameManagerObject.GetComponent<BuildingManager>().GetComponentInChildren<Canvas>().transform);
        tmpObject.GetComponent<Image>().color = color;
        tmpObject.GetComponent<BoxCollider2D>().enabled = false; // this wont push the player character around

        Tile tmpTile = gridMan.GetTile(new Vector2(0, 0));
        RectTransform rect = tmpObject.GetComponent<RectTransform>();
        Vector2 size = BuildingInformation.GetBuildingSize((BuildingInformation.TYPE_OF_BUILDING)type) * 100;
        tmpObject.transform.localScale = new Vector3(tmpTile.GetSize().x * size.x / rect.sizeDelta.x, tmpTile.GetSize().y * size.y / rect.sizeDelta.y, 1);
    }

    bool AvoidObstacles(Tile tile)
    {
        bool placeable = true;
        if (tile == null)
        {
            placeable = false;
        }
        else
        {
            Vector2 size = BuildingInformation.GetBuildingSize((BuildingInformation.TYPE_OF_BUILDING)type);
            int startX = -Mathf.FloorToInt(size.x / 2f);
            int endX = Mathf.CeilToInt(size.x / 2f);
            int startY = -Mathf.FloorToInt(size.y / 2f);
            int endY = Mathf.CeilToInt(size.y / 2f);
            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    Tile currTile = gridMan.GetTile(new Vector2(tile.GetTilePosition().x + x, tile.GetTilePosition().y + y));
                    if (currTile != null)
                    {
                        if (!currTile.BuildPermission() || currTile.IsObjectPresent()
                            || currTile.IsCharacterPresent(typeof(Player)) || currTile.IsCharacterPresent(typeof(Enemy)) || currTile.IsCharacterPresent(typeof(Soldier)))
                        {
                            placeable = false;
                        }
                    }
                    else
                    {
                        placeable = false;
                    }
                }
            }
        }

        return placeable;
    }
}
