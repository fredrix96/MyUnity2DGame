using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile
{
    GameObject go;
    Vector2 destination;
    int damage;
    BoxCollider2D collider;
    float timeToLive;
    double timer;
    LayerMask target;

    public void Init(GameObject source, GameObject parent, Vector2 inOrigin, Vector2 inDestination, Sprite inSprite, Vector2 inSize, int inDamage, float inTimeToLive, LayerMask inTarget)
    {
        go = new GameObject { name = source.name + "_Projectile" + ProjectileCounter.projectileCounter };
        go.transform.SetParent(parent.transform);
        go.transform.position = inOrigin;
        go.transform.localScale = inSize;
        
        destination = inDestination;
        damage = inDamage;
        timeToLive = inTimeToLive;
        timer = 0;
        target = inTarget;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = inSprite;

        Vector2 vec1 = Vector2.right;
        Vector2 vec2 = inDestination - inOrigin;

        float angle = Vector2.Angle(vec1, vec2);

        if (vec1.y > vec2.y)
        {
            angle = -angle;
        }
         
        go.transform.Rotate(go.transform.rotation.x, go.transform.rotation.y, angle);

        collider = go.AddComponent<BoxCollider2D>();
        collider.enabled = false; // To make sure that it is not in the way for other objects
        Physics2D.IgnoreCollision(collider, source.GetComponent<BoxCollider2D>());
    }

    public bool Update()
    {
        timer += Time.deltaTime;

        if (CheckCollisions())
        {
            Object.Destroy(go);
            return false;
        }
        else
        {
            go.transform.position = new Vector3(Mathf.Lerp(go.transform.position.x, destination.x, 5 * Time.deltaTime),
                Mathf.Lerp(go.transform.position.y, destination.y, 5 * Time.deltaTime), 0);

            if (timeToLive < timer)
            {
                Object.Destroy(go);
                return false;
            }
        }

        return true;
    }

    bool CheckCollisions()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(target);

        Vector3 pivotOffsetWorldSpace = go.transform.position - collider.bounds.center;

        List<Collider2D> results = new List<Collider2D>();
        collider.enabled = true;
        collider.OverlapCollider(filter, results);

        foreach (Collider2D col in results)
        {
            if (col.gameObject.GetComponent<Health>() != null)
            {
                col.gameObject.GetComponent<Health>().Damage(damage, false, false);
            }
        }

        collider.enabled = false;

        if (results.Count > 0)
        {
            return true;
        }
        return false;
    }
}
