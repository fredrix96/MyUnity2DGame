using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    Image healthBar, heart;
    Slider slider;
    GameObject parent;
    Sprite[] barSprites;

    int spriteCounter;
    int health;
    int maxHealth;
    double timerHeart, timerBar;
    float timeDelayHeart, timeDelayBar;
    float growingSpeed;
    float growingSpeedOriginal;

    private void Start()
    {
        timerHeart = 0;
        timerBar = 0;
        timeDelayHeart = 0.4f;
        timeDelayBar = 0.15f;
        growingSpeed = 0.3f;
        growingSpeedOriginal = growingSpeed;
        spriteCounter = 0;

        barSprites = Resources.LoadAll<Sprite>("Sprites/PlayerHealth_Anim");
    }

    public void Init(GameObject inParent, int inHealth)
    {
        slider = UIManager.CreateSlider("healthbarSlider", "healthbarImage", Resources.Load<Sprite>("Sprites/PlayerHealth"), new Vector2(70, 500), new Vector2(12.0f, 2.3f), out healthBar, Color.green);
        heart = UIManager.CreateImage(null, "heartImage", Resources.Load<Sprite>("Sprites/Heart"), new Vector2(-880, 505), new Vector2(60, 60)).GetComponent<Image>();

        parent = inParent;
        maxHealth = inHealth;
        health = maxHealth;
    }

    void Update()
    {
        if (health > 0)
        {
            PlayHeartAnimation();
            PlayBarAnimation();
        }
    }

    void PlayHeartAnimation()
    {
        timerHeart += Time.deltaTime;

        heart.transform.localScale = new Vector2(heart.transform.localScale.x + growingSpeed * Time.deltaTime, heart.transform.localScale.y + growingSpeed * Time.deltaTime);

        if (timerHeart > timeDelayHeart)
        {
            growingSpeed *= -1;
            timerHeart = 0;
        }
    }

    void PlayBarAnimation()
    {
        timerBar += Time.deltaTime;

        if (timerBar > timeDelayBar)
        {
            healthBar.sprite = barSprites[spriteCounter];
            spriteCounter++;
            
            if (spriteCounter == barSprites.Length)
            {
                spriteCounter = 0;
            }

            timerBar = 0;
        }
    }

    void SetGrowingSpeed(float speed = 1)
    {
        if (growingSpeed < 0)
        {
            growingSpeed = -growingSpeedOriginal * speed;
        }
        else
        {
            growingSpeed = growingSpeedOriginal * speed;
        }
    }

    // How much health should the character lose?
    public void Damage(int damage)
    {
        int oldHealth = health;
        health -= damage;

        if (health < maxHealth * 0.2f)
        {
            SetGrowingSpeed(0.3f);
        }
        else if (health < maxHealth * 0.5f)
        {
            SetGrowingSpeed(0.7f);
        }

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

            AudioManager.PlayAudio3D("Player Hit", 0.1f, parent.transform.position);
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

        if (health > maxHealth * 0.5f)
        {
            SetGrowingSpeed();
        }
    }

    public int GetHealth()
    {
        return health;
    }

    void ChangeColor(float newHealth)
    {
        float percentage = newHealth / maxHealth;

        if (percentage >= 0.33f)
        {
            healthBar.color = Color.white;
        }
        //else if (percentage < 0.66f && percentage >= 0.33f)
        //{
        //    healthBar.color = Color.yellow;
        //}
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
