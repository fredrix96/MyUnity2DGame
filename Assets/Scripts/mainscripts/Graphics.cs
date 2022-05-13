﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graphics
{
    GameObject level, background;

    Vector4 worldLimits, levelLimits;

    // Start is called before the first frame update
    public Graphics()
    {
        float res = Mathf.CeilToInt(Screen.width / 267F);

        int tMyRes = Mathf.CeilToInt(Screen.width / 440F);
        if (tMyRes > res)
        {
            res = tMyRes;
        }

        float halfScreenWidth = Screen.width / 2 / res;
        float halfScreenHeight = Screen.height / 2 / res;

        level = new GameObject();
        level.name = "level";
        level.transform.parent = GameManager.GameManagerObject.transform;

        SpriteRenderer srLvl = level.AddComponent<SpriteRenderer>();
        srLvl.sprite = Resources.Load<Sprite>("Sprites/Level");
        srLvl.sortingLayerName = "Level";
        RectTransform transformLvl = level.AddComponent<RectTransform>();

        background = new GameObject();
        background.name = "background";
        background.transform.parent = GameManager.GameManagerObject.transform;

        SpriteRenderer srBg = background.AddComponent<SpriteRenderer>();
        srBg.sprite = Resources.Load<Sprite>("Sprites/Background2");
        srBg.sortingLayerName = "Level";
        RectTransform transformBg = background.AddComponent<RectTransform>();
        transformBg.anchoredPosition = new Vector3(transformLvl.position.x, transformLvl.position.y + transformLvl.rect.height, transformLvl.position.z);

        // X min, X max, Y min, Y max
        levelLimits = new Vector4(-transformLvl.rect.width / 2, transformLvl.rect.width / 2, -transformLvl.rect.height / 2, transformLvl.rect.height / 2);
        worldLimits = new Vector4(-transformLvl.rect.width / 2, transformLvl.rect.width / 2, -transformLvl.rect.height / 2, transformLvl.rect.height / 2 + transformBg.rect.height);
    }

    public Vector4 GetLevelLimits()
    {
        return levelLimits;
    }

    public Vector4 GetWorldLimits()
    {
        return worldLimits;
    }
}
