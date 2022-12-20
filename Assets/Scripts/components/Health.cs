﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    GameObject go, parent;
    Canvas canvas;
    SpriteRenderer sr;
    DamageText dt;

    float height;

    int health;
    int maxHealth;

    public void Init(GameObject inGo, string spriteSource, int inHealth, Vector2 size, float inExtraHeight = 0, bool building = false)
    {
        go = new GameObject();
        go.name = inGo.name + "_healthbar";
        parent = inGo;
        go.transform.SetParent(parent.transform);

        canvas = go.AddComponent<Canvas>();

        sr = go.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "UI";

        sr.sprite = Resources.Load<Sprite>(spriteSource);
        sr.drawMode = SpriteDrawMode.Sliced;
        sr.size = new Vector2(sr.size.x * size.x, sr.size.y * size.y);
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.8f);

        health = inHealth;
        maxHealth = inHealth;

        if (!building)
        {
            height = sr.sprite.bounds.max.y;
        }
        else if (go.transform.parent.GetComponent<BoxCollider2D>() == null)
        {
            Debug.Log("Warning: " + inGo.name + " does not have a box collider! Could not apply the correct height...");
        }
        else
        {
            height = go.transform.parent.GetComponent<BoxCollider2D>().size.y * go.transform.parent.localScale.y * 0.65f;
        }

        height += inExtraHeight;
        go.transform.position = new Vector3(go.transform.parent.position.x, go.transform.parent.position.y + height, go.transform.parent.position.z);
    }

    void Update()
    {
        go.transform.position = new Vector3(go.transform.parent.position.x, go.transform.parent.position.y + height, go.transform.parent.position.z);
    }

    // How much health should the character lose?
    public void Damage(int damage, bool crit, bool displayDamage)
    {
        int oldHealth = health;
        health -= damage;

        if (parent.GetComponent<SpriteManager>() != null)
        {
            parent.GetComponent<SpriteManager>().StartTakingDamage();
        }

        if (displayDamage)
        {
            dt = go.AddComponent<DamageText>();
            
            Color color = Color.white;
            Vector2 size = new Vector2(1, 1);
            if (crit)
            {
                color = Color.red;
                size = new Vector2(1.25f, 1.25f);
            }

            dt.CreateDamageText(go, damage, go.transform.position, color, size);
        }

        // Health can not go below 0
        if (health <= 0)
        {
            health = 0;
            sr.size = Vector2.zero;
        }
        else
        {
            // Calculate how many percentages are left
            float percentage = (float)health / (float)maxHealth;

            // Change the scale to match the new percentage
            sr.size = new Vector2(sr.size.x * percentage, sr.size.y);
        }
    }

    public void IncreaseHealth(int increase)
    {
        int oldHealth = health;
        health += increase;

        // Calculate how many percentages are left
        float percentage = (float)health / (float)maxHealth;

        // Change the scale to match the new percentage
        sr.size = new Vector2(sr.size.x * percentage, sr.size.y);
    }

    public void IncreaseMaxHealth(int increase)
    {
        maxHealth += increase;
        health += increase;
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}
