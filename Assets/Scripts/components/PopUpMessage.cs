using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpMessage : MonoBehaviour
{
    GameObject canvasObject, messageObject;
    Canvas canvas;
    CanvasScaler cs;
    Text message;

    double messageTime;
    float messageLifeTime;

    // Start is called before the first frame update
    public void Init(GameObject go)
    {
        // Canvas
        canvasObject = new GameObject { name = "canvas" };
        canvasObject.transform.parent = go.transform;

        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = CameraManager.GetCamera();
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

                if (messageTime > messageLifeTime && messageLifeTime != -1)
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
        messageObject.transform.localScale = new Vector2(1f, 1f);

        RectTransform rectTitle = messageObject.AddComponent<RectTransform>();
        rectTitle.anchorMin = Vector2.zero;
        rectTitle.anchorMax = Vector2.zero;
        rectTitle.pivot = new Vector2(0.5f, 0.5f);
        rectTitle.anchoredPosition = new Vector2(canvas.pixelRect.width / 2, canvas.pixelRect.height / 2);
        rectTitle.sizeDelta = new Vector2(0.55f * canvas.pixelRect.width, 0.3f * canvas.pixelRect.height);

        Outline outline = messageObject.AddComponent<Outline>();
        outline.effectDistance = new Vector2(3.0f, 3.0f);

        message = messageObject.AddComponent<Text>();
        message.text = "";
        message.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        message.fontSize = 2 * (int)Graphics.resolution;
        message.color = Color.white;
        message.fontStyle = FontStyle.Bold;
        message.alignment = TextAnchor.MiddleCenter;

        messageObject.SetActive(false);
    }

    /// <summary> A lifeTime of -1 means that the message wont disappear </summary> 
    public void SendPopUpMessage(string text, float lifeTime = 2.5f, int fontSize = 30, Color? color = null)
    {
        // Returns left value if it is not null, otherwise it returns the value to the right
        message.color = color ?? Color.white;

        message.fontSize = fontSize * (int)Graphics.resolution / 4;
        messageLifeTime = lifeTime;
        message.text = text;
        messageObject.SetActive(true);
    }
}
