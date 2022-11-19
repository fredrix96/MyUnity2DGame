using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ArcheryTower : Building
{
    float shootDelay;
    int damage;
    double timer;
    Vector2 shootingOrigin;
    CircleCollider2D searchArea;
    LayerMask target;
    ProjectileManager pm;
    Sprite projSprite;

    public ArcheryTower(GameObject parent, Tile inPos, List<Building> inBuildings)
    {
        type = BuildingInformation.TYPE_OF_BUILDING.ArcheryTower;

        centerTile = inPos;
        buildings = inBuildings;

        damage = 10;
        shootDelay = 0.5f;

        go = new GameObject { name = "building_" + type.ToString() + BuildingInformation.GetCounter(type).ToString() };
        go.transform.SetParent(parent.transform);
        go.layer = LayerMask.NameToLayer("Buildings");

        go.AddComponent<CollisionManager>();

        message = go.AddComponent<PopUpMessage>();
        message.Init(go);

        sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("Sprites/" + type.ToString());

        PositionRendererSorter prs = go.AddComponent<PositionRendererSorter>();
        prs.SetIsOnlyRunOnce();
        prs.SetOffsetManually(-100);

        go.transform.position = inPos.GetWorldPos();

        MarkOrUnmarkTiles(type, inPos, true);

        collider = go.AddComponent<BoxCollider2D>();
        searchArea = go.AddComponent<CircleCollider2D>();
        searchArea.radius = 50;
        searchArea.enabled = false;

        rb = go.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.useFullKinematicContacts = true;

        CreateHealthBar(type, -0.2f);
        CreateToolBar();

        collider.size = new Vector2(sr.size.x, sr.size.y / 3.5f);
        collider.offset = new Vector2(0, -(sr.size.y / 3.0f));

        shootingOrigin = new Vector2(go.transform.position.x, go.transform.position.y + sr.bounds.size.y * 0.1f);

        selector = go.AddComponent<Selector>();
        selector.Init(toolBarObject, sr, textObject, buttonObject);
        selector.SetOutlineColor(Color.blue);
        selector.SetWidth(5);

        pm = go.AddComponent<ProjectileManager>();
        projSprite = Resources.Load<Sprite>("Sprites/Arrow");

        target = LayerMask.GetMask("Enemies");

        BuildingInformation.IncreaseCounter(type);
    }

    public override void Update()
    {
        CheckIfDestroyed();

        LookIfIgnored();

        timer += Time.deltaTime;
        if (timer > shootDelay)
        {
            Shoot();
            timer = 0;
        }
    }

    void CreateToolBar()
    {
        CreateCanvas();

        CreateToolbarObject(new Vector2(3f, 1f), 2f);

        CreateInfoText("Archery Tower", 30, TextAnchor.MiddleCenter, new Vector2(65, 75));
    }

    public void CheckIfDestroyed()
    {
        if (health.GetHealth() <= 0)
        {
            shouldBeRemoved = true;
        }
    }

    public void Shoot()
    {
        // Search for targets
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(target);

        List<Collider2D> results = new List<Collider2D>();
        searchArea.enabled = true;
        searchArea.OverlapCollider(filter, results);

        List<Vector2> hitPoints = new List<Vector2>();
        Vector3 pos;
        foreach (Collider2D collider2d in results)
        {
            pos = new Vector2(collider2d.bounds.center.x, collider2d.bounds.center.y);
            hitPoints.Add(pos);
        }

        searchArea.enabled = false;

        // Find closest target
        float distance = float.MaxValue;
        int index = -1;
        for (int i = 0; i < hitPoints.Count;i++)
        {
            float newDist = Tools.CalculateVectorDistance(shootingOrigin, hitPoints[i]);
            if (newDist < distance)
            {
                distance = newDist;
                index = i;
            }
        }

        if (hitPoints.Count > 0 && index != -1)
        {
            // Send projectile
            pm.SpawnProjectile(go, shootingOrigin, hitPoints[index], projSprite, new Vector2(0.035f, 0.05f), damage, 1.5f, target);
        }

        hitPoints.Clear();
    }
}
