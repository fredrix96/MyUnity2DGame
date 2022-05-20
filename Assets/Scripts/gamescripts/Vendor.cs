using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vendor : MonoBehaviour
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
            Tile tile = MousePosToTilePos();

            // Follow the mouse
            tmpObject.transform.position = tile.GetPos();

            // If there is obstacles on the tiles, mark the building red
            if (!AvoidObstacles(tile))
            {
                // Indicate with red color that the building can not be placed here
                tmpObject.GetComponent<Image>().color = new Color(0.8f, 0.2f, 0.2f, 0.8f);
            }
            else
            {
                tmpObject.GetComponent<Image>().color = Color.white;
            }
        }
    }

    void OnMouseUp()
    {
        if (draging)
        {
            Tile tile = MousePosToTilePos();

            // If there is no obstacle, create the building
            if (AvoidObstacles(tile))
            {
                // Use Gameobject.find?
                GameManager.GameManagerObject.GetComponent<BuildingManager>().CreateBuilding((BuildingInformation.TYPE_OF_BUILDING)type, tile, gridMan);
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

            Tile bottomRightTile = gridMan.GetTile(new Vector2(tile.GetTilePosition().x + endX - 1, tile.GetTilePosition().y + endY - 1));
            Tile bottomLeftTile = gridMan.GetTile(new Vector2(tile.GetTilePosition().x + startX, tile.GetTilePosition().y + endY - 1));
            Tile tileBelowBuilding = gridMan.GetTile(new Vector2(tile.GetTilePosition().x + startX, tile.GetTilePosition().y + endY));

            // The "door" to the building can not be placed at the very bottom of the grid, however, it is allowed to be placed at the very top
            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    Tile currTile = gridMan.GetTile(new Vector2(tile.GetTilePosition().x + x, tile.GetTilePosition().y + y));

                    if (currTile != null)
                    {
                        if (!currTile.IsPlaceable() || tileBelowBuilding == null)
                        {
                            placeable = false;
                        }
                    }
                    else if (currTile == null && bottomLeftTile != null && bottomRightTile != null)
                    {
                        if (bottomLeftTile.IsPlaceable() && bottomRightTile.IsPlaceable())
                        {
                            if (tileBelowBuilding != null)
                            {
                                continue;
                            }
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

    Tile MousePosToTilePos()
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

        return tile;
    }
}
