using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class House : Building
{
    int nrOfHumans;
    int maxHumans;

    double time;
    float timeDelay;

    public House(GameObject parent, Tile inPos, CoinManager inCoinMan, List<Building> inBuildings)
    {
        type = BuildingInformation.TYPE_OF_BUILDING.House;

        nrOfHumans = 0;
        maxHumans = 5;
        HumansCounter.max += maxHumans;

        time = 0;
        timeDelay = 5f;

        centerTile = inPos;
        coinMan = inCoinMan;
        buildings = inBuildings;

        go = new GameObject { name = "building_" + type.ToString() + BuildingInformation.GetCounter(type).ToString() };
        go.transform.SetParent(parent.transform);
        go.layer = LayerMask.NameToLayer("Buildings");

        go.AddComponent<CollisionManager>();

        sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("Sprites/" + type.ToString());
        sr.sortingLayerName = "Buildings";

        go.transform.position = inPos.GetWorldPos();

        MarkOrUnmarkTiles(type, inPos, true);

        collider = go.AddComponent<BoxCollider2D>();
        rb = go.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.useFullKinematicContacts = true;

        CreateHealthBar(type);
        CreateToolBar();

        selector = go.AddComponent<Selector>();
        selector.Init(toolBarObject, sr, textObject, null);
        selector.SetOutlineColor(Color.blue);
        selector.SetWidth(5);

        BuildingInformation.IncreaseCounter(type);
    }

    public override void Update()
    {
        time += Time.deltaTime;

        CheckIfDestroyed();

        LookIfIgnored();

        // Add humans
        if (time >= timeDelay)
        {
            if (nrOfHumans < maxHumans)
            {
                nrOfHumans++;
                HumansCounter.nrOfHumans++;
                text.text = nrOfHumans + " / " + maxHumans + " Humans";
            }

            time = 0;
        }
    }

    void CreateToolBar()
    {
        CreateToolbarObject(new Vector2(10, 10), 0.15f);

        CreateInfoText(nrOfHumans + " / " + maxHumans + " Humans", TextAnchor.MiddleCenter, new Vector2(40, 10));
    }

    public int GetNrOfHumans()
    {
        return nrOfHumans;
    }

    public void RemoveHuman()
    {
        nrOfHumans--;
        text.text = nrOfHumans + " / " + maxHumans + " Humans";
    }

    public void CheckIfDestroyed()
    {
        if (health.GetHealth() <= 0)
        {
            HumansCounter.nrOfHumans -= nrOfHumans;
            HumansCounter.max -= maxHumans;
            shouldBeRemoved = true;
        }
    }
}
