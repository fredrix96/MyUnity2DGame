using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graphics
{
    GameObject level;

    Vector4 worldLimits;

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

        SpriteRenderer sr = level.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("Sprites/Level");
        sr.sortingLayerName = "Level";
        RectTransform transform = level.AddComponent<RectTransform>();

        // X min, X max, Y min, Y max
        worldLimits = new Vector4(-transform.rect.width / 2, transform.rect.width / 2, -transform.rect.height / 2, transform.rect.height / 2);
    }

    public Vector4 GetWorldLimits()
    {
        return worldLimits;
    }
}
