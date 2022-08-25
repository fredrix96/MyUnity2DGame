using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vendor : MonoBehaviour
{
    GameObject go, tmpObject;
    Image img;
    CoinManager coinMan;

    BuildingInformation.TYPE_OF_BUILDING type;

    int cost;
    bool draging;

    void Start()
    {
        img = GetComponent<Image>();
    }

    public void Init(GameObject inGo, CoinManager inCoinMan, BuildingInformation.TYPE_OF_BUILDING inType)
    {
        go = inGo;
        coinMan = inCoinMan;

        img = go.GetComponent<Image>();
        img.color = Color.white;
        type = inType;
        cost = BuildingInformation.GetBuildingCost(type);
    }

    void OnMouseEnter()
    {
        if (!BuildingInformation.MaxLimitReached(type))
        {
            img.color = new Color(0.3f, 0.3f, 0.6f, 1.0f);
        }
    }

    void OnMouseExit()
    {
        if (!BuildingInformation.MaxLimitReached(type))
        {
            img.color = Color.white;
        }
    }

    void OnMouseDown()
    {
        if (!BuildingInformation.MaxLimitReached(type))
        {
            if (coinMan.SpendMoney(cost))
            {
                CreateBuildingImage();

                Vector3 screenPoint = Input.mousePosition;
                screenPoint.z = CameraManager.GetCamera().nearClipPlane;
                tmpObject.transform.position = CameraManager.GetCamera().ScreenToWorldPoint(screenPoint);

                draging = true;

                // Hide shop UI
                go.GetComponentInParent<Canvas>().enabled = false;
                go.transform.parent.GetComponentInChildren<Image>().enabled = false;
            }
        }
        else
        {
            go.GetComponent<PopUpMessage>().SendPopUpMessage("You have reached the max limit!", 1.5f);
        }
    }

    void OnMouseDrag()
    {
        if (draging)
        {
            GridManager.ActivateAreaImage(true);

            Tile tile = MousePosToTilePos();

            // Follow the mouse
            tmpObject.transform.position = tile.GetWorldPos();

            // If there is obstacles on the tiles, mark the building red
            if (!AvoidObstacles(tile) || !CheckIfInsidePlacementArea(tile))
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
            GridManager.ActivateAreaImage(false);

            Tile tile = MousePosToTilePos();

            // If inside the placement area
            if (CheckIfInsidePlacementArea(tile))
            {
                // If there is no obstacle, create the building
                if (AvoidObstacles(tile))
                {
                    // Use Gameobject.find?
                    GameManager.GameManagerObject.GetComponent<BuildingManager>().CreateBuilding(type, tile);
                }
                else
                {
                    go.GetComponent<PopUpMessage>().SendPopUpMessage("You can not place the building" + System.Environment.NewLine + "at an obstacle!");
                    coinMan.AddCoins(cost);
                }
            }
            else
            {
                go.GetComponent<PopUpMessage>().SendPopUpMessage("You can not place the building" + System.Environment.NewLine + "outside the placement area!");
                coinMan.AddCoins(cost);
            }

            Destroy(tmpObject);

            draging = false;

            // Show shop UI
            go.GetComponentInParent<Canvas>().enabled = true;
            go.transform.parent.GetComponentInChildren<Image>().enabled = true;
        }
    }

    void CreateBuildingImage()
    {
        tmpObject = new GameObject(go.name + "_tmp");
        tmpObject.transform.SetParent(GameManager.GameManagerObject.GetComponent<BuildingManager>().GetComponentInChildren<Canvas>().transform);

        Image newImg = tmpObject.AddComponent<Image>();
        newImg.sprite = img.sprite;
        newImg.color = Color.white;
        
        Tile tmpTile = GridManager.GetTile(new Vector2(0, 0));
        RectTransform rect = tmpObject.GetComponent<RectTransform>();
        Vector2 size = BuildingInformation.GetBuildingSize(type) * 12 * Graphics.resolution;
        tmpObject.transform.localScale = new Vector3(tmpTile.GetSize().x * size.x / rect.sizeDelta.x, tmpTile.GetSize().y * size.y / rect.sizeDelta.y, 1);
    }

    bool CheckIfInsidePlacementArea(Tile tile)
    {
        bool inside = false;

        Vector4 area = GridManager.GetPlacementAreaBorders();
        Vector2 tilePos = tile.GetWorldPos();

        // Note that the object will be moved to the nearest tile if it is above the play area
        if (tilePos.x > area.x && tilePos.x < area.y && tilePos.y > area.z && tilePos.y < area.w * 2)
        {
            inside = true;
        }

        return inside;
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
            Vector2 size = BuildingInformation.GetBuildingSize(type);
            int startX = -Mathf.FloorToInt(size.x / 2f);
            int endX = Mathf.CeilToInt(size.x / 2f);
            int startY = -Mathf.FloorToInt(size.y / 2f);
            int endY = Mathf.CeilToInt(size.y / 2f);

            Tile bottomRightTile = GridManager.GetTile(new Vector2(tile.GetTilePosition().x + endX - 1, tile.GetTilePosition().y + endY - 1));
            Tile bottomLeftTile = GridManager.GetTile(new Vector2(tile.GetTilePosition().x + startX, tile.GetTilePosition().y + endY - 1));
            Tile tileBelowBuilding = GridManager.GetTile(new Vector2(tile.GetTilePosition().x + startX, tile.GetTilePosition().y + endY));

            // The "door" to the building can not be placed at the very bottom of the grid, however, it is allowed to be placed at the very top
            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    Tile currTile = GridManager.GetTile(new Vector2(tile.GetTilePosition().x + x, tile.GetTilePosition().y + y));

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
        screenPoint.z = CameraManager.GetCamera().nearClipPlane;

        Vector3 worldPoint = CameraManager.GetCamera().ScreenToWorldPoint(screenPoint);
        Tile tile = GridManager.GetTileFromWorldPosition(worldPoint);

        // If the mouse pointer is not on a tile, find the closest tile
        if (tile == null)
        {
            tile = GridManager.FindClosestTile(worldPoint);
        }

        return tile;
    }
}
