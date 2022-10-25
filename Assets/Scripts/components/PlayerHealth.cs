using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    Image healthBar, heart;
    Slider slider;
    GameObject parent;

    int health;
    int maxHealth;

    public void Init(GameObject inParent, int inHealth)
    {
        slider = UIManager.CreateSlider("healthbarSlider", "healthbarImage", null, new Vector2(70, 500), new Vector2(10.0f, 2.0f), out healthBar, Color.green);
        heart = UIManager.CreateImage(null, "heartImage", Resources.Load<Sprite>("Sprites/Heart"), new Vector2(-880, 505), new Vector2(60, 60)).GetComponent<Image>();

        parent = inParent;
        maxHealth = inHealth;
        health = maxHealth;
    }

    void Update()
    {
    }

    // How much health should the character lose?
    public void Damage(int damage)
    {
        int oldHealth = health;
        health -= damage;

        parent.GetComponent<SpriteManager>().StartTakingDamage();

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
        Object.Destroy(GameObject.Find("heartImage"));
        Object.Destroy(GameObject.Find("healthbarSlider"));
        Object.Destroy(GameObject.Find("healthbarImage"));
    }
}
