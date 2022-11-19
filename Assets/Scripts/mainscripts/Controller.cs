using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller
{
    Player player;
    ShopManager shopMan;

    public Controller(Player inPlayer, ShopManager inShopMan)
    {
        player = inPlayer;
        shopMan = inShopMan;
    }

    public void Update()
    {
        UpdateMouse();
        UpdateKeys();
    }

    void UpdateMouse()
    {
        if (Input.mousePosition.x < CameraManager.GetCamBorderInScreenPoint(0.05f, 0.05f).x && Input.GetMouseButton(1))
        {
            if (Graphics.GetWorldLimits().x < CameraManager.GetPosX() - CameraManager.GetWorldSpaceWidth() / 2)
            {
                CameraManager.SetDirX(-1);
            }
        }
        else if (Input.mousePosition.x > CameraManager.GetCamBorderInScreenPoint(0.05f, 0.05f).y && Input.GetMouseButton(1))
        {
            if (Graphics.GetWorldLimits().y > CameraManager.GetPosX() + CameraManager.GetWorldSpaceWidth() / 2)
            {
                CameraManager.SetDirX(1);
            }
        }

        if (Input.mousePosition.y > CameraManager.GetCamBorderInScreenPoint(0.05f, 0.05f).w && Input.GetMouseButton(1))
        {
            if (Graphics.GetWorldLimits().w > CameraManager.GetPosY() + CameraManager.GetWorldSpaceHeight() / 2)
            {
                CameraManager.SetDirY(1);
            }
        }
        else if (Input.mousePosition.y < CameraManager.GetCamBorderInScreenPoint(0.05f, 0.05f).z && Input.GetMouseButton(1))
        {
            if (Graphics.GetWorldLimits().z < CameraManager.GetPosY() - CameraManager.GetWorldSpaceHeight() / 2)
            {
                CameraManager.SetDirY(-1);
            }
        }

        // Scroll to "zoom"
        if (Input.mouseScrollDelta.y > 0)
        {
            CameraManager.ZoomIn();
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            CameraManager.ZoomOut();

            // Make sure the camera stays inside the game area
            if (Graphics.GetWorldLimits().x > CameraManager.GetPosX() - CameraManager.GetWorldSpaceWidth() / 2)
            {
                CameraManager.SetPosX(Graphics.GetWorldLimits().x + CameraManager.GetWorldSpaceWidth() / 2);
            }
            else if (Graphics.GetLevelLimits().y < CameraManager.GetPosX() + CameraManager.GetWorldSpaceWidth() / 2)
            {
                CameraManager.SetPosX(Graphics.GetWorldLimits().y - CameraManager.GetWorldSpaceWidth() / 2);
            }

            if (Graphics.GetWorldLimits().w < CameraManager.GetPosY() + CameraManager.GetWorldSpaceHeight() / 2)
            {
                CameraManager.SetPosY(Graphics.GetWorldLimits().w - CameraManager.GetWorldSpaceHeight() / 2);
            }
            else if (Graphics.GetWorldLimits().z > CameraManager.GetPosY() - CameraManager.GetWorldSpaceHeight() / 2)
            {
                CameraManager.SetPosY(Graphics.GetWorldLimits().z + CameraManager.GetWorldSpaceHeight() / 2);
            }
        }
    }

    void UpdateKeys()
    {
        // Player related keys
        if (player.GetPlayerObject() != null)
        {
            player.Idle();

            if (Input.GetKey(KeyCode.Space))
            {
                player.Attack();
            }

            Vector2 playerPos = player.GetPosition();
            if (Input.GetKey(KeyCode.W) && playerPos.y <= GridManager.GetTile(new Vector2(0,0)).GetWorldPos().y + GridManager.GetTileHeight() * 1.5f)
            {
                player.SetDirY(1);
            }
            else if (Input.GetKey(KeyCode.S) && playerPos.y >= GridManager.GetTile(new Vector2(0, GridManager.GetRes().y - 2)).GetWorldPos().y)
            {
                player.SetDirY(-1);
            }

            if (Input.GetKey(KeyCode.A) && playerPos.x >= GridManager.GetTile(new Vector2(0,0)).GetWorldPos().x)
            {
                player.SetDirX(-1);
            }
            else if (Input.GetKey(KeyCode.D) && playerPos.x <= GridManager.GetTile(new Vector2(GridManager.GetRes().x - 1, 0)).GetWorldPos().x)
            {
                player.SetDirX(1);
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            shopMan.ChangeActive();
        }

        // Control if the camera will follow the player or the mouse
        int scrollWheelButton = 2;
        if (Input.GetMouseButtonDown(scrollWheelButton))
        {
            if (CameraManager.IsOnPlayerActivated()) CameraManager.ActivateOnPlayer(false);
            else CameraManager.ActivateOnPlayer(true);
        }

        // If game over, go back to the menu
        if (Input.GetKeyDown(KeyCode.Escape) && GameManager.IsGameOver())
        {
            EventManager.InvokeUnloadLevel();
            EventManager.InvokeLoadMenu();
        }
    }
}
