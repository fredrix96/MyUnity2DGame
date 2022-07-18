using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    GameObject go, canvasObject, imageObject, healthObject, heartObject, playerObject;
    Canvas canvas;
    CanvasScaler cs;
    Image healthBar, heart;
    Slider slider;

    int health;
    int maxHealth;

    public void Init(GameObject inGo, int inHealth)
    {
        playerObject = inGo;

        // Head object
        go = new GameObject { name = playerObject.name + "_healthbar" };
        go.transform.SetParent(playerObject.transform);

        // Canvas
        canvasObject = new GameObject { name = "canvas" };
        canvasObject.transform.parent = go.transform;

        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = CameraManager.GetCamera();
        canvas.sortingLayerName = "UI";

        cs = canvasObject.AddComponent<CanvasScaler>();
        cs.referenceResolution = new Vector2(1920, 1080);

        // Health object
        healthObject = new GameObject { name = "healthObject" };
        healthObject.transform.SetParent(canvasObject.transform);
        healthObject.transform.localScale = new Vector2(0.3f, 0.3f);

        slider = healthObject.AddComponent<Slider>();
        slider.interactable = false;
        slider.transition = Selectable.Transition.None;
        slider.navigation = Navigation.defaultNavigation;

        imageObject = new GameObject { name = "healthbarImage" };
        imageObject.transform.SetParent(canvasObject.transform);
        imageObject.transform.localScale = new Vector2(0.1f, 0.02f);

        healthBar = imageObject.AddComponent<Image>();
        healthBar.color = Color.green;

        // Reset anchor
        healthBar.rectTransform.anchorMin = Vector2.zero;
        healthBar.rectTransform.anchorMax = Vector2.zero;

        // Anchor the image
        healthBar.rectTransform.anchoredPosition = new Vector3(canvas.pixelRect.width * 0.04f, canvas.pixelRect.height * 0.455f, 0);

        healthBar.rectTransform.pivot = new Vector2(0.0f, 0.5f);
        healthBar.rectTransform.sizeDelta = new Vector2(12.5f, 10.0f) * Graphics.resolution;

        slider.fillRect = healthBar.rectTransform;

        // Heart object
        heartObject = new GameObject { name = "heart" };
        heartObject.transform.SetParent(canvasObject.transform);
        heartObject.transform.localScale = new Vector2(0.3f, 0.3f);

        heart = heartObject.AddComponent<Image>();
        heart.sprite = Resources.Load<Sprite>("Sprites/Heart");
        heart.rectTransform.sizeDelta = new Vector2(20.0f, 20.0f) * Graphics.resolution;

        // Reset anchor
        heart.rectTransform.anchorMin = Vector2.zero;
        heart.rectTransform.anchorMax = Vector2.zero;

        // Anchor the image
        heart.rectTransform.anchoredPosition = new Vector3(canvas.pixelRect.width * 0.04f, canvas.pixelRect.height - canvas.pixelRect.height / 25, 0);

        slider.value = 1;

        maxHealth = inHealth;
        health = maxHealth;

        // Move the UI interface out of the way of the game world to 
        //go.transform.position = new Vector3(200, 0, 0);
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

            //AudioManager.PlayAudio3D("Player Hit", 0.1f, playerObject.transform.position);
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
