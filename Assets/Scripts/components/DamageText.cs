using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class DamageTextCounter
{
    public static int counter = 0;
}

public class DamageText : MonoBehaviour
{
    GameObject textObject;
    Canvas canvas;
    float baseYPos;
    float distance;
    TextMeshPro text;

    public void CreateDamageText(GameObject go, int damage, Vector2 inPos, Color inColor, Vector2 inSize)
    {   
        textObject = new GameObject { name = "Damage tex" + DamageTextCounter.counter };
        textObject.transform.SetParent(go.transform);
        textObject.transform.position = inPos;
        textObject.layer = LayerMask.NameToLayer("UI");

        canvas = textObject.AddComponent<Canvas>();

        text = textObject.AddComponent<TextMeshPro>();
        text.text = damage.ToString();
        text.color = inColor;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = 50;
        text.transform.localScale = new Vector2(0.06f * inSize.x, 0.06f * inSize.y);

        distance = 0.5f;
        baseYPos = inPos.y;

        DamageTextCounter.counter++;
    }

    // Update is called once per frame
    void Update()
    {
        if (text != null)
        {
            if (text.transform.position.y < baseYPos + distance)
            {
                text.transform.position = new Vector2(text.transform.position.x, text.transform.position.y + Time.deltaTime);
            }
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - Time.deltaTime);
        }
    }
}
