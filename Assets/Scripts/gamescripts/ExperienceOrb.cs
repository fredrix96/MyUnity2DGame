using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceOrb
{
    GameObject orb;
    SpriteRenderer sr;
    float slideDist;
    Vector2 originalPos;
    Vector2 playerPos;
    Vector2 randDir;
    PositionRendererSorter prs;
    CircleCollider2D cc;
    int expPoints;
    bool collected;

    public void CreateOrb(GameObject parent, Vector2 pos, int orbCounter, int inExpPoints)
    {
        orb = new GameObject("orb" + orbCounter.ToString());
        orb.transform.SetParent(parent.transform);
        orb.transform.position = pos;
        orb.transform.localScale = new Vector2(0.05f, 0.05f);
        orb.layer = LayerMask.NameToLayer("Experience");

        sr = orb.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("Sprites/ExperienceOrb");
        sr.sortingOrder = 0; // always last

        cc = orb.AddComponent<CircleCollider2D>();

        //prs = orb.AddComponent<PositionRendererSorter>();

        originalPos = pos;
        slideDist = 0.3f;
        expPoints = inExpPoints;

        randDir = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));

        playerPos = Vector2.zero;

        collected = false;
    }

    public void Update()
    {
        if (Tools.CalculateVectorDistance(originalPos, orb.transform.position) < slideDist)
        {
            Slide();
        }

        if (collected)
        {
            Gather();
        }
    }

    public int GetExpPoints()
    {
        return expPoints;
    }

    void Slide()
    {
        orb.transform.position = new Vector2(orb.transform.position.x + randDir.x * Time.deltaTime, orb.transform.position.y + randDir.y * Time.deltaTime);
    }

    void Gather()
    {
        orb.transform.position = new Vector3(Mathf.MoveTowards(orb.transform.position.x, playerPos.x, Time.deltaTime * 5),
            Mathf.MoveTowards(orb.transform.position.y, playerPos.y, Time.deltaTime * 5), 0);

        if (playerPos.x == orb.transform.position.x && playerPos.y == orb.transform.position.y)
        {
            AudioManager.PlayAudio3D("ExpOrbSound", 0.3f, playerPos);
            orb.SetActive(false);
        }
    }

    public void AddPlayerPos(Vector2 pos)
    {
        playerPos = pos;
    }

    public void SetCollected()
    {
        collected = true;
        cc.enabled = false;
    }

    public GameObject GetGameObject()
    {
        return orb;
    }

    public void DestroyOrb()
    {
        Object.Destroy(cc);
        Object.Destroy(orb);
    }
}
