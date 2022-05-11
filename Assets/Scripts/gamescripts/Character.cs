using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    protected GameObject go;
    protected SpriteManager sm;
    protected Graphics gfx;
    protected BoxCollider2D bc;
    protected Rigidbody2D rb;
    protected PathFinding pf;
    protected CollisionManager cm;
    protected Tile currTile;
    protected GridManager gm;
    protected Health health;

    protected int direction;
    protected int damage;
    protected float speed;
    protected bool targetFound;
    protected bool isDead;
    protected bool shouldBeRemoved;

    public Character()
    {
        targetFound = false;
        isDead = false;
        shouldBeRemoved = false;
    }


    public virtual void Update() { }

    public Vector2 GetPosition()
    {
        return go.transform.position;
    }

    public virtual void Destroy() { }

    public string GetName()
    {
        return go.name;
    }

    public bool IsDead()
    {
        return isDead;
    }
}
