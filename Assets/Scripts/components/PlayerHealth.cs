using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    GameObject go, canvasObject, imageObject, healthObject, heartObject;
    Canvas canvas;
    CanvasScaler cs;
    Image healthBar, heart;
    Slider slider;

    int health;
    int maxHealth;

    public void Init(GameObject inGo, int inHealth)
    {
        // Head object
        go = new GameObject { name = inGo.name + "_healthbar" };
        go.transform.SetParent(inGo.transform);

        // Canvas
        canvasObject = new GameObject { name = "canvas" };
        canvasObject.transform.parent = go.transform;
        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = CameraManager.GetCamera();
        canvas.sortingLayerName = "UI";

        cs = canvasObject.AddComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);
        cs.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        cs.matchWidthOrHeight = 1; // match with height (1)

        // Health object
        healthObject = new GameObject { name = "healthObject" };
        healthObject.transform.SetParent(canvasObject.transform);
        healthObject.transform.localScale = new Vector2(1.8f, 0.5f);

        RectTransform rectHO = healthObject.AddComponent<RectTransform>();
        rectHO.anchorMax = Vector2.zero;
        rectHO.anchorMin = Vector2.zero;
        rectHO.anchoredPosition = new Vector3(160, 952, 0);

        slider = healthObject.AddComponent<Slider>();
        slider.interactable = false;
        slider.transition = Selectable.Transition.None;
        slider.navigation = Navigation.defaultNavigation;

        // Image object
        imageObject = new GameObject { name = "health" };
        imageObject.transform.SetParent(healthObject.transform); 
        imageObject.transform.localScale = healthObject.transform.localScale;

        healthBar = imageObject.AddComponent<Image>();
        healthBar.color = Color.green;

        slider.fillRect = healthBar.rectTransform;

        healthBar.rectTransform.pivot = new Vector2(0.0f, 0.5f);
        healthBar.rectTransform.sizeDelta = Vector2.zero;

        // Reset anchor
        healthBar.rectTransform.anchorMin = Vector2.zero;
        healthBar.rectTransform.anchorMax = Vector2.zero;

        // Anchor the image
        healthBar.rectTransform.anchoredPosition = new Vector3(0, 150, 0);

        // Heart object
        heartObject = new GameObject { name = "heart" };
        heartObject.transform.SetParent(canvasObject.transform);
        heartObject.transform.localScale = new Vector2(0.5f, 0.5f);

        heart = heartObject.AddComponent<Image>();
        heart.sprite = Resources.Load<Sprite>("Sprites/Heart");

        // Reset anchor
        heart.rectTransform.anchorMin = Vector2.zero;
        heart.rectTransform.anchorMax = Vector2.zero;

        // Anchor the image
        heart.rectTransform.anchoredPosition = new Vector3(80, 1030, 0);

        slider.value = 1;

        maxHealth = inHealth;
        health = maxHealth;

        // Move the UI interface out of the way of the game world to 
        go.transform.position = new Vector3(200, 0, 0);
    }

    void Update()
    {
    }

    // How much health should the character lose?
    public void Damage(int damage)
    {
        int oldHealth = health;
        health -= damage;

        // Health can not go below 0
        if (health <= 0)
        {
            health = 0;
            healthBar.transform.localScale = Vector2.zero;
            heart.sprite = Resources.Load<Sprite>("Sprites/Skull");
        }
        else
        {
            // Calculate how many percentages are left
            float percentage = (float)health / (float)oldHealth;

            // Change the scale to match the new percentage
            slider.value *= percentage;

            // Set the correct color
            ChangeColor(health);
        }
    }

    public void IncreaseHealth(int increase)
    {
        int oldHealth = health;
        health += increase;

        // Calculate how many percentages are left
        float percentage = (float)health / (float)oldHealth;

        // Change the scale to match the new percentage
        slider.value *= percentage;

        // Set the correct color
        ChangeColor(health);
    }

    public int GetHealth()
    {
        return health;
    }

    void ChangeColor(float newHealth)
    {
        float percentage = newHealth / maxHealth;

        if (percentage >= 0.66f)
        {
            healthBar.color = Color.green;
        }
        else if (percentage < 0.66f && percentage >= 0.33f)
        {
            healthBar.color = Color.yellow;
        }
        else
        {
            healthBar.color = Color.red;
        }
    }

    public void Destroy()
    {
        Object.Destroy(go);
        Object.Destroy(canvasObject);
        Object.Destroy(imageObject);
        Object.Destroy(healthObject);
        Object.Destroy(heartObject);
    }
}
