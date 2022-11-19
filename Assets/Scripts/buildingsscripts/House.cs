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

    public House(GameObject parent, Tile inPos, List<Building> inBuildings)
    {
        type = BuildingInformation.TYPE_OF_BUILDING.House;

        nrOfHumans = 0;
        maxHumans = 5;
        HumansCounter.max += maxHumans;

        time = 0;
        timeDelay = 5f;

        centerTile = inPos;
        buildings = inBuildings;

        go = new GameObject { name = "building_" + type.ToString() + BuildingInformation.GetCounter(type).ToString() };
        go.transform.SetParent(parent.transform);
        go.layer = LayerMask.NameToLayer("Buildings");

        PositionRendererSorter prs = go.AddComponent<PositionRendererSorter>();
        prs.SetIsOnlyRunOnce();
        go.AddComponent<CollisionManager>();

        sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("Sprites/" + type.ToString());

        go.transform.position = inPos.GetWorldPos();

        MarkOrUnmarkTiles(type, inPos, true);

        collider = go.AddComponent<BoxCollider2D>();
        rb = go.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.useFullKinematicContacts = true;

        CreateHealthBar(type);
        CreateToolBar();

        collider.size = new Vector2(sr.size.x, sr.size.y / 2);
        collider.offset = new Vector2(0, -(sr.size.y / 4));

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
        CreateCanvas();

        CreateToolbarObject(new Vector2(1.5f, 0.5f), 0.8f);

        CreateInfoText(nrOfHumans + " / " + maxHumans + " Humans", 25, TextAnchor.MiddleCenter, new Vector2(70f, 120f));
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
