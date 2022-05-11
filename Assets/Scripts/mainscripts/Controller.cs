using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller
{
    Graphics gfx;
    CameraManager cm;
    Player player;

    public Controller(Graphics inGfx, CameraManager inCm, Player inPlayer)
    {
        gfx = inGfx;
        cm = inCm;
        player = inPlayer;
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
            else if (gfx.GetWorldLimits().y < cm.GetPosX() + cm.GetWorldSpaceWidth() / 2)
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
        if (!player.Remove())
        {
            player.Idle();

            if (Input.GetKey(KeyCode.Space))
            {
                player.Attack();
            }

            if (Input.GetKey(KeyCode.W) && player.GetPosition().y < gfx.GetWorldLimits().w)
            {
                player.SetDirY(1);
            }
            else if (Input.GetKey(KeyCode.S) && player.GetPosition().y > gfx.GetWorldLimits().z + player.GetSize().y / 2f)
            {
                player.SetDirY(-1);
            }

            if (Input.GetKey(KeyCode.A) && player.GetPosition().x > gfx.GetWorldLimits().x)
            {
                player.SetDirX(-1);
            }
            else if (Input.GetKey(KeyCode.D) && player.GetPosition().x < gfx.GetWorldLimits().y)
            {
                player.SetDirX(1);
            }
        }
    }
}
