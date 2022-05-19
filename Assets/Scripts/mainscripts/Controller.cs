using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller
{
    Graphics gfx;
    CameraManager cm;
    Player player;
    ShopManager shopMan;
    GridManager gridMan;

    public Controller(Graphics inGfx, CameraManager inCm, Player inPlayer, ShopManager inShopMan, GridManager inGridMan)
    {
        gfx = inGfx;
        cm = inCm;
        player = inPlayer;
        shopMan = inShopMan;
        gridMan = inGridMan;
    }

    public void Update()
    {
        UpdateMouse();
        UpdateKeys();
    }

    void UpdateMouse()
    {
        if (Input.mousePosition.x < cm.GetCamBorderInScreenPoint(0.05f, 0.05f).x && Input.GetMouseButton(1))
        {
            if (gfx.GetWorldLimits().x < cm.GetPosX() - cm.GetWorldSpaceWidth() / 2)
            {
                cm.SetDirX(-1);
            }
        }
        else if (Input.mousePosition.x > cm.GetCamBorderInScreenPoint(0.05f, 0.05f).y && Input.GetMouseButton(1))
        {
            if (gfx.GetWorldLimits().y > cm.GetPosX() + cm.GetWorldSpaceWidth() / 2)
            {
                cm.SetDirX(1);
            }
        }

        if (Input.mousePosition.y > cm.GetCamBorderInScreenPoint(0.05f, 0.05f).w && Input.GetMouseButton(1))
        {
            if (gfx.GetWorldLimits().w > cm.GetPosY() + cm.GetWorldSpaceHeight() / 2)
            {
                cm.SetDirY(1);
            }
        }
        else if (Input.mousePosition.y < cm.GetCamBorderInScreenPoint(0.05f, 0.05f).z && Input.GetMouseButton(1))
        {
            if (gfx.GetWorldLimits().z < cm.GetPosY() - cm.GetWorldSpaceHeight() / 2)
            {
                cm.SetDirY(-1);
            }
        }

        // Scroll to "zoom"
        if (Input.mouseScrollDelta.y > 0)
        {
            cm.ZoomIn();
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            cm.ZoomOut();

            // Make sure the camera stays inside the game area
            if (gfx.GetWorldLimits().x > cm.GetPosX() - cm.GetWorldSpaceWidth() / 2)
            {
                cm.SetPosX(gfx.GetWorldLimits().x + cm.GetWorldSpaceWidth() / 2);
            }
            else if (gfx.GetLevelLimits().y < cm.GetPosX() + cm.GetWorldSpaceWidth() / 2)
            {
                cm.SetPosX(gfx.GetWorldLimits().y - cm.GetWorldSpaceWidth() / 2);
            }

            if (gfx.GetWorldLimits().w < cm.GetPosY() + cm.GetWorldSpaceHeight() / 2)
            {
                cm.SetPosY(gfx.GetWorldLimits().w - cm.GetWorldSpaceHeight() / 2);
            }
            else if (gfx.GetWorldLimits().z > cm.GetPosY() - cm.GetWorldSpaceHeight() / 2)
            {
                cm.SetPosY(gfx.GetWorldLimits().z + cm.GetWorldSpaceHeight() / 2);
            }
        }
    }

    void UpdateKeys()
    {
        // Player related keys
        if (!player.Remove())
        {
            player.Idle();

            if (Input.GetKey(KeyCode.Space))
            {
                player.Attack();
            }

            Vector2 playerPos = player.GetPosition();
            if (Input.GetKey(KeyCode.W) && playerPos.y <= gridMan.GetTile(new Vector2(0,0)).GetPos().y)
            {
                player.SetDirY(1);
            }
            else if (Input.GetKey(KeyCode.S) && playerPos.y >= gridMan.GetTile(new Vector2(0, gridMan.GetRes().y - 1)).GetPos().y)
            {
                player.SetDirY(-1);
            }

            if (Input.GetKey(KeyCode.A) && playerPos.x >= gridMan.GetTile(new Vector2(0,0)).GetPos().x)
            {
                player.SetDirX(-1);
            }
            else if (Input.GetKey(KeyCode.D) && playerPos.x <= gridMan.GetTile(new Vector2(gridMan.GetRes().x - 1, 0)).GetPos().x)
            {
                player.SetDirX(1);
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            shopMan.ChangeActive();
        }
    }
}
