using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraManager
{
    static Camera mainCam;

    static List<GameObject> cameras;

    static float camOrtoSize;

    static int cameraSpeed;
    static int zoomSpeed;

    static Vector2 dirVector;

    static bool onPlayer;

    static public void Init()
    {
        mainCam = null;

        camOrtoSize = 5.0f;
        zoomSpeed = 100;
        cameraSpeed = 20;

        mainCam = Camera.main;
        mainCam.orthographicSize = camOrtoSize;

        cameras = new List<GameObject>();

        dirVector = Vector2.zero;
    }

    public static void Update(Player player)
    {
        // Movement with player
        if (player != null && onPlayer)
        {
            PlayerMove(player);
        }
        else
        {
            // Movement with mouse
            if (dirVector.x != 0 || dirVector.y != 0)
            {
                MouseMove();
            }
        }

        SetDirX();
        SetDirY();
    }

    public static void ResetMainCameraPosition()
    {
        mainCam.transform.position = new Vector3(0, 0, -10);
    }

    public static void ActivateOnPlayer(bool activate)
    {
        onPlayer = activate;
    }

    public static bool IsOnPlayerActivated()
    {
        return onPlayer;
    }

    public static void CreateCamera(GameObject inGo, bool orthographic = false, CameraClearFlags flag = CameraClearFlags.Skybox)
    {
        GameObject UICamObject = new GameObject { name = inGo.name + "_camera" };
        UICamObject.transform.parent = inGo.transform;

        Camera UICam = UICamObject.AddComponent<Camera>();
        UICam.orthographic = orthographic;
        UICam.clearFlags = flag;
        UICam.orthographicSize = 100;

        cameras.Add(UICamObject);
    }

    // Returns the main camera if no game object is chosen
    public static Camera GetCamera(GameObject inGo = null)
    {
        if (inGo == null)
        {
            return mainCam;
        }

        foreach (GameObject cam in cameras)
        {
            if (cam.name == inGo.name + "_camera")
            {
                return cam.GetComponent<Camera>();
            }
        }

        Debug.LogWarning("Could not find a camera called " + inGo.name + "_camera");

        return null;
    }

    // Offset must be above 0 or below 1. The function returns a vector4 with (Xmin, Xmax, Ymin, Ymax)
    public static Vector4 GetCamBorderInScreenPoint(float offsetX = 0, float offsetY = 0, GameObject inGo = null)
    {
        Vector4 camBorder = Vector4.zero;

        // The main camera is the default
        Camera outCam = mainCam;

        if (offsetX < 0 || offsetX > 1 || offsetY < 0 || offsetY > 1)
        {
            offsetX = 0;
            offsetY = 0;

            Debug.LogWarning("Warning: One or both offsets are below 0 or above 1... changing the offsets to 0");
        }

        if (inGo != null)
        {
            // Returning the borders of the camera belonging to the given gameobject
            outCam = GetCamera(inGo);
        }

        camBorder = new Vector4(outCam.ViewportToScreenPoint(new Vector3(0 + offsetX, 0, 0)).x, outCam.ViewportToScreenPoint(new Vector3(1 - offsetX, 0, 0)).x,
                outCam.ViewportToScreenPoint(new Vector3(0, 0 + offsetY, 0)).y, outCam.ViewportToScreenPoint(new Vector3(0, 1 - offsetY, 0)).y);

        return camBorder;
    }

    // Offset must be above 0 or below 1. The function returns a vector4 with (Xmin, Xmax, Ymin, Ymax)
    public static Vector4 GetCamBorderInWorldPoint(float offsetX = 0, float offsetY = 0, GameObject inGo = null)
    {
        Vector4 camBorder = Vector4.zero;

        // The main camera is the default
        Camera outCam = mainCam;

        if (offsetX < 0 || offsetX > 1 || offsetY < 0 || offsetY > 1)
        {
            offsetX = 0;
            offsetY = 0;

            Debug.LogWarning("Warning: One or both offsets are below 0 or above 1... changing the offsets to 0");
        }

        if (inGo != null)
        {
            // Returning the borders of the camera belonging to the given gameobject
            outCam = GetCamera(inGo);
        }

        camBorder = new Vector4(outCam.ViewportToWorldPoint(new Vector3(0 + offsetX, 0, 0)).x, outCam.ViewportToWorldPoint(new Vector3(1 - offsetX, 0, 0)).x,
                outCam.ViewportToWorldPoint(new Vector3(0, 0 + offsetY, 0)).y, outCam.ViewportToWorldPoint(new Vector3(0, 1 - offsetY, 0)).y);

        return camBorder;
    }

    static void MouseMove()
    {
        mainCam.transform.position = new Vector3(mainCam.transform.position.x + cameraSpeed * Time.deltaTime * dirVector.x,
            mainCam.transform.position.y + cameraSpeed * Time.deltaTime * dirVector.y, mainCam.transform.position.z);
    }

    static void PlayerMove(Player player)
    {
        Vector2 playerPos = player.GetPosition();
        mainCam.transform.position = new Vector3(Mathf.Lerp(mainCam.transform.position.x, playerPos.x, 3 * Time.deltaTime),
                Mathf.Lerp(mainCam.transform.position.y, playerPos.y, 3 * Time.deltaTime), mainCam.transform.position.z);

        // Stay inside of border
        if (Graphics.GetWorldLimits().x > GetPosX() - GetWorldSpaceWidth() / 2)
        {
            SetPosX(Graphics.GetWorldLimits().x + GetWorldSpaceWidth() / 2);
        }
        else if (Graphics.GetLevelLimits().y < GetPosX() + GetWorldSpaceWidth() / 2)
        {
            SetPosX(Graphics.GetWorldLimits().y - GetWorldSpaceWidth() / 2);
        }

        if (Graphics.GetWorldLimits().w < GetPosY() + GetWorldSpaceHeight() / 2)
        {
            SetPosY(Graphics.GetWorldLimits().w - GetWorldSpaceHeight() / 2);
        }
        else if (Graphics.GetWorldLimits().z > GetPosY() - GetWorldSpaceHeight() / 2)
        {
            SetPosY(Graphics.GetWorldLimits().z + GetWorldSpaceHeight() / 2);
        }
    }

    public static void SetDirX(int x = 0)
    {
        dirVector = new Vector2(x, dirVector.y);
    }

    public static void SetDirY(int y = 0)
    {
        dirVector = new Vector2(dirVector.x, y);
    }

    public static void SetPosX(float newXPos)
    {
        mainCam.transform.position = new Vector3(newXPos, mainCam.transform.position.y, mainCam.transform.position.z);
    }

    public static void SetPosY(float newYPos)
    {
        mainCam.transform.position = new Vector3(mainCam.transform.position.x, newYPos, mainCam.transform.position.z);
    }

    public static void ZoomIn()
    {
        mainCam.orthographicSize = Mathf.MoveTowards(mainCam.orthographicSize, camOrtoSize / 4, zoomSpeed * Time.deltaTime);
    }

    public static void ZoomOut()
    {
        mainCam.orthographicSize = Mathf.MoveTowards(mainCam.orthographicSize, camOrtoSize, zoomSpeed * Time.deltaTime);
    }

    public static float GetPosX()
    {
        return mainCam.transform.position.x;
    }

    public static float GetPosY()
    {
        return mainCam.transform.position.y;
    }

    public static float GetWorldSpaceWidth()
    {
        return mainCam.ViewportToWorldPoint(new Vector2(1, 0)).x - mainCam.ViewportToWorldPoint(new Vector2(0, 0)).x;
    }

    public static float GetWorldSpaceHeight()
    {
        return mainCam.ViewportToWorldPoint(new Vector2(0, 1)).y - mainCam.ViewportToWorldPoint(new Vector2(0, 0)).y;
    }
}
