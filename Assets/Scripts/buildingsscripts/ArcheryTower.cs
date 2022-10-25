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
    Vector2 shootingDirection;

    ProjectileManager pm;
    Sprite projSprite;

    public ArcheryTower(GameObject parent, Tile inPos, CoinManager inCoinMan, List<Building> inBuildings)
    {
        type = BuildingInformation.TYPE_OF_BUILDING.ArcheryTower;

        centerTile = inPos;
        coinMan = inCoinMan;
        buildings = inBuildings;

        damage = 50;
        shootDelay = 0.5f;

        go = new GameObject { name = "building_" + type.ToString() + BuildingInformation.GetCounter(type).ToString() };
        go.transform.SetParent(parent.transform);
        go.layer = LayerMask.NameToLayer("Buildings");

        go.AddComponent<PositionRendererSorter>();
        go.AddComponent<CollisionManager>();

        message = go.AddComponent<PopUpMessage>();
        message.Init(go);

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

        collider.size = new Vector2(sr.size.x, sr.size.y / 5);
        collider.offset = new Vector2(0, -(sr.size.y / 3));

        shootingOrigin = new Vector2(go.transform.position.x, go.transform.position.y);

        selector = go.AddComponent<Selector>();
        selector.Init(toolBarObject, sr, textObject, buttonObject);
        selector.SetOutlineColor(Color.blue);
        selector.SetWidth(5);

        pm = go.AddComponent<ProjectileManager>();
        projSprite = Resources.Load<Sprite>("Sprites/Arrow");

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
        List<Vector2> hitPoints = new List<Vector2>();

        // Multithreading?
        // Search for targets
        for (int i = 0; i < 180; i++)
        {
            shootingDirection = new Vector2(Mathf.Cos(i + 1), Mathf.Cos(i));
            float searchLength = 10;
            RaycastHit2D hit = Physics2D.Raycast(shootingOrigin, shootingDirection, searchLength, LayerMask.GetMask("Enemies"));

            if (hit)
            {
                hitPoints.Add(hit.point);
            }
        }

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
            pm.SpawnProjectile(go, shootingOrigin, hitPoints[index], projSprite, new Vector2(0.035f, 0.05f), damage, 1.5f, LayerMask.GetMask("Enemies"));

#if DEBUG
            if (Tools.DebugMode)
            {
              Debug.DrawRay(shootingOrigin, hitPoints[index], Color.white, 15f);
            }
#endif
        }

        hitPoints.Clear();
    }
}
