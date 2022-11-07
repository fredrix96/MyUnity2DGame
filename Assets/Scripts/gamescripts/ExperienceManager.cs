using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ExperienceManager
{
    static int orbCounter;
    static GameObject go;
    static List<ExperienceOrb> orbList;
    static Player player;
    static double timerBar;
    static float timeDelayBar;
    static int spriteCounter;
    static Sprite[] sprites;
    static Slider slider;
    static Image expBar;
    static Image expCapsule;
    static Image expCapsuleBg;

    static public void Init(Player inPlayer)
    {
        player = inPlayer;

        orbList = new List<ExperienceOrb>();
        orbCounter = 0;

        go = new GameObject("experienceManager");
        go.transform.SetParent(GameManager.GameManagerObject.transform);

        sprites = Resources.LoadAll<Sprite>("Sprites/ExpBar_Anim2");
        spriteCounter = 0;

        float yPos = -520f;
        expCapsuleBg = UIManager.CreateImage(null, "expBarCapsuleBackground", Resources.Load<Sprite>("Sprites/ExpCapsuleBackground"), new Vector2(0, yPos), new Vector2(1020, 30)).GetComponent<Image>();
        expCapsuleBg.color = new Color(expCapsuleBg.color.r, expCapsuleBg.color.b, expCapsuleBg.color.g, 0.4f);
        slider = UIManager.CreateSlider("experienceBar_slider", "experienceBar", sprites[0], new Vector2(458, yPos), new Vector2(52.5f, 2.3f), out expBar);
        slider.fillRect.sizeDelta = new Vector2(0, slider.fillRect.sizeDelta.y);
        slider.value = 0;
        expBar.type = Image.Type.Sliced;
        expCapsule = UIManager.CreateImage(null, "expBarCapsule", Resources.Load<Sprite>("Sprites/ExpCapsule"), new Vector2(0, yPos), new Vector2(1020, 30)).GetComponent<Image>();

        timerBar = 0;
        timeDelayBar = 0.1f;
    }

    static public void UpdateXpBar()
    {
        float currExp = player.GetCurrentXp();
        float maxExp = player.GetLevelXp();

        float percentage = currExp / maxExp;

        // Change the scale to match the new percentage
        slider.value = percentage;

        if (slider.value >= 1)
        {
            player.LevelUp();
        }
    }

    static public void DropOrb(Vector2 pos, int expPoints)
    {
        ExperienceOrb orb = new ExperienceOrb();
        orb.CreateOrb(go, pos, orbCounter, expPoints);
        orbList.Add(orb);
        orbCounter++;
    }

    static public ExperienceOrb GetOrb(string name)
    {
        foreach (ExperienceOrb orb in orbList)
        {
            if (orb.GetGameObject().name == name)
            {
                return orb;
            }
        }

        return null;
    }

    static public void Update()
    {
        PlayBarAnimation();

        for (int i = 0; i < orbList.Count; i++)
        {
            orbList[i].AddPlayerPos(player.GetPosition());
            orbList[i].Update();

            if (!orbList[i].GetGameObject().activeSelf)
            {
                Object.Destroy(orbList[i].GetGameObject());
                orbList.Remove(orbList[i]);
            }
        }
    }

    static void PlayBarAnimation()
    {
        timerBar += Time.deltaTime;

        if (timerBar > timeDelayBar)
        {
            expBar.sprite = sprites[spriteCounter];
            spriteCounter++;

            if (spriteCounter == sprites.Length)
            {
                spriteCounter = 0;
            }

            timerBar = 0;
        }
    }
}
