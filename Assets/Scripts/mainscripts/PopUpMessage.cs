﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpMessage : MonoBehaviour
{
    GameObject canvasObject, messageObject;
    Canvas canvas;
    CanvasScaler cs;
    Text message;
    CameraManager cam;

    double messageTime;
    float messageLifeTime;

    // Start is called before the first frame update
    public void Init(GameObject go, CameraManager inCam)
    {
        cam = inCam;

        // Canvas
        canvasObject = new GameObject { name = "canvas" };
        canvasObject.transform.parent = go.transform;
        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = cam.GetCamera();
        canvas.sortingLayerName = "Message";

        cs = canvasObject.AddComponent<CanvasScaler>();
        cs.referenceResolution = new Vector2(1920, 1080);

        CreatePopUpMessageText();

        messageLifeTime = 0;
        messageTime = 0;
    }

    void Update()
    {
        if (messageObject != null)
        {
            if (messageObject.activeSelf)
            {
                messageTime += Time.deltaTime;

                if (messageTime > messageLifeTime)
                {
                    messageObject.SetActive(false);
                    messageTime = 0;
                }
            }
        }
    }

    void CreatePopUpMessageText()
    {
        // Message object
        messageObject = new GameObject { name = "popUpMessage" };

        messageObject.transform.SetParent(canvas.transform);
        messageObject.transform.localScale = new Vector2(0.8f, 0.55f);

        Outline outline = messageObject.AddComponent<Outline>();
        outline.effectDistance = new Vector2(3, 3);

        message = messageObject.AddComponent<Text>();
        message.text = "";
        message.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        message.fontSize = 20 * (int)cam.GetCamera().aspect;
        message.color = Color.white;
        message.fontStyle = FontStyle.Bold;
        message.alignment = TextAnchor.MiddleCenter;

        // Reset anchor
        message.rectTransform.anchorMin = Vector2.zero;
        message.rectTransform.anchorMax = Vector2.zero;
        message.rectTransform.sizeDelta = new Vector2(1920, 1080);

        // Anchor the image
        message.rectTransform.anchoredPosition = new Vector3(canvas.pixelRect.width / 2, canvas.pixelRect.height / 2, 0);

        messageObject.SetActive(false);
    }

    public void SendPopUpMessage(string text, float lifeTime = 2.5f, int fontSize = 30, Color? color = null)
    {
        // Returns left value if it is not null, otherwise it returns the value to the right
        message.color = color ?? Color.white;

        message.fontSize = fontSize * (int)cam.GetCamera().aspect;
        messageLifeTime = lifeTime;
        message.text = text;
        messageObject.SetActive(true);
    }
}
